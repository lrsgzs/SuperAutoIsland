import * as Blockly from 'blockly';
import { Order } from 'blockly/javascript';
import { addBlock, data, type BlocklyArgDefinition, type BlocklyBlockDefinition } from '../utils/blockGenerator';

/**
 * 字典积木（突变器版）
 *
 * 参考：https://docs.blockly.com/guides/create-custom-blocks/mutators/
 *
 * 主块形状：
 *   字典
 *   1 [key1] = [value1]
 *   2 [key2] = [value2]
 *   ...
 *
 * 点击齿轮图标打开突变器气泡，可以增删键值对。
 * 键是 String 类型的输入，值是任意类型的输入。
 * 生成器把键值对直接解析成 JSON 对象（Object.fromEntries）。
 *
 * 只有工具箱里初始的两个键（key1/key2）带 text shadow 作提示；
 * 新增的键值对（第 3 对起）不带 shadow，避免 shadowDom 在操作中
 * respawn/裸断开产生孤儿块。
 */

/**
 * 移除一个输入上连接的子块：
 * - shadow block 直接销毁（默认 text shadow 被刨出来只会变成无法交互的孤儿块）
 * - 真实块只断开连接，保留在画布上（用户可继续拖走使用）
 */
const detachChild = (connection: Blockly.Connection) => {
    const block = connection.getSourceBlock();
    if (block.isShadow()) {
        block.dispose(true);
    } else {
        connection.disconnect();
    }
};

/**
 * 重连前清空目标输入上「不是本次连接」的子块：
 * - shadow 直接销毁（reconnect 内部会裸 disconnect 输入上已有的 shadow，导致孤儿块）
 * - 真实块断开；若断开后输入上又 respawn 出 shadow（key 输入带 shadowDom），继续清掉
 */
const clearInputForReconnect = (input: Blockly.Input | null | undefined, keep: Blockly.Connection | null) => {
    let target = input?.connection?.targetConnection ?? null;
    if (!target || target === keep) return;
    while (target) {
        const child = target.getSourceBlock();
        if (child.isShadow()) {
            child.dispose(true);
            return;
        }
        target.disconnect();
        target = input?.connection?.targetConnection ?? null;
    }
};

/**
 * 注册字典突变器（mutator）。
 *
 * - saveExtraState / loadExtraState：JSON 序列化钩子（项目用 Blockly.serialization，首选）
 * - decompose / compose / saveConnections：默认突变器 UI（齿轮气泡）
 * - updateShape_：按 itemCount_ 增删键值对输入
 * - helper 函数：积木创建时把默认键值对数设为 2
 */
Blockly.Extensions.registerMutator(
    'dictionary_mutator',
    {
        // ---- JSON 序列化钩子 ----
        saveExtraState(this: any) {
            // 默认 2 项时返回 null，不写入 extraState，减小存档体积
            return this.itemCount_ === 2 ? null : { keyCount: this.itemCount_ };
        },
        loadExtraState(this: any, state: { keyCount: number }) {
            this.itemCount_ = state.keyCount;
            this.updateShape_();
        },

        // ---- 突变器 UI 钩子 ----
        decompose(this: any, workspace: Blockly.Workspace) {
            const topBlock = workspace.newBlock('dictionary_mutator_container');
            // 突变器工作区里的块都是 BlockSvg，initSvg 是 SVG 块才有的方法
            (topBlock as any).initSvg();
            let connection = topBlock.getInput('STACK')!.connection!;
            for (let i = 1; i <= this.itemCount_; i++) {
                const itemBlock = workspace.newBlock('dictionary_mutator_item');
                (itemBlock as any).initSvg();
                connection.connect(itemBlock.previousConnection!);
                connection = itemBlock.nextConnection!;
            }
            return topBlock;
        },
        compose(this: any, topBlock: Blockly.Block) {
            // 1. 收集仍在突变器里的项所对应的主块连接（键 + 值）
            const keyConnections: (Blockly.Connection | null)[] = [];
            const valueConnections: (Blockly.Connection | null)[] = [];
            let itemBlock = topBlock.getInputTargetBlock('STACK');
            while (itemBlock && !itemBlock.isInsertionMarker()) {
                keyConnections.push((itemBlock as any).keyConnection_ ?? null);
                valueConnections.push((itemBlock as any).valueConnection_ ?? null);
                itemBlock = itemBlock.nextConnection && itemBlock.nextConnection.targetBlock();
            }

            // 2. 清理被删除的项：shadow 自动销毁，真实块断开保留在画布
            for (let i = 1; i <= this.itemCount_; i++) {
                const keyTarget = this.getInput('key' + i)?.connection.targetConnection;
                const valueTarget = this.getInput('value' + i)?.connection.targetConnection;
                if (keyTarget && keyConnections.indexOf(keyTarget) === -1) detachChild(keyTarget);
                if (valueTarget && valueConnections.indexOf(valueTarget) === -1) detachChild(valueTarget);
            }

            // 3. 更新形状
            this.itemCount_ = keyConnections.length;
            this.updateShape_();

            // 4. 重连子积木（先清掉目标输入上非本次连接的子块，避免裸断开 shadow 产生孤儿）
            for (let i = 0; i < this.itemCount_; i++) {
                if (keyConnections[i]) {
                    clearInputForReconnect(this.getInput('key' + (i + 1)), keyConnections[i]);
                    keyConnections[i]!.reconnect(this, 'key' + (i + 1));
                }
                if (valueConnections[i]) {
                    clearInputForReconnect(this.getInput('value' + (i + 1)), valueConnections[i]);
                    valueConnections[i]!.reconnect(this, 'value' + (i + 1));
                }
            }
        },
        saveConnections(this: any, topBlock: Blockly.Block) {
            let itemBlock = topBlock.getInputTargetBlock('STACK');
            let i = 1;
            while (itemBlock) {
                const keyInput = this.getInput('key' + i);
                const valueInput = this.getInput('value' + i);
                (itemBlock as any).keyConnection_ = keyInput && keyInput.connection.targetConnection;
                (itemBlock as any).valueConnection_ = valueInput && valueInput.connection.targetConnection;
                i++;
                itemBlock = itemBlock.nextConnection && itemBlock.nextConnection.targetBlock();
            }
        },

        // ---- 形状更新：按 itemCount_ 增删键值对 ----
        updateShape_(this: any) {
            // 标题：空字典时只显示「空字典」标签
            // （标题是 message0 里「字典\n」生成的未命名 endrow，位于 inputList[0]，字段是 field_label）
            const titleField = this.inputList[0]?.fieldRow[0];
            if (titleField) {
                titleField.setValue(this.itemCount_ === 0 ? '创建空字典' : '创建字典');
            }

            for (let i = 1; i <= this.itemCount_; i++) {
                if (!this.getInput('key' + i)) {
                    // 先补一个换行输入，保证每个键值对单独一行
                    this.appendEndRowInput('row' + i);
                    this.appendValueInput('key' + i)
                        .setCheck('String')
                        .appendField(String(i));
                    this.appendValueInput('value' + i).appendField('=');
                    // 新增的键不带 shadow（只有工具箱里初始的两个键有）
                }
            }
            for (let i = this.itemCount_ + 1; this.getInput('key' + i); i++) {
                // 移除前先清理输入上的子块：shadow 自动销毁，真实块断开保留
                const keyTarget = this.getInput('key' + i)?.connection.targetConnection;
                const valueTarget = this.getInput('value' + i)?.connection.targetConnection;
                if (keyTarget) detachChild(keyTarget);
                if (valueTarget) detachChild(valueTarget);
                this.removeInput('key' + i);
                this.removeInput('value' + i);
                // 初始形状的换行输入没有名字，静默删除
                this.removeInput('row' + i, true);
            }
        },
    },
    function (this: any) {
        this.itemCount_ = 2;
        this.updateShape_();
    },
    ['dictionary_mutator_item'],
);

/**
 * 突变器气泡里的容器块和项块（只注册定义，不放进工具箱）。
 */
data.blocks?.push(
    {
        type: 'dictionary_mutator_container',
        message0: '字典项 %1',
        args0: [{ type: 'input_statement', name: 'STACK' } as unknown as BlocklyArgDefinition],
        colour: 255,
    } as BlocklyBlockDefinition,
    {
        type: 'dictionary_mutator_item',
        message0: '项目',
        args0: [],
        previousStatement: null,
        nextStatement: null,
        colour: 255,
    } as BlocklyBlockDefinition,
);

/**
 * 主块：字典
 *
 * message 中的 \n 会被 Blockly 解析成 input_end_row（换行），
 * 所以初始形状与积木定义一致：
 *   字典
 *   1 [key1] = [value1]
 *   2 [key2] = [value2]
 */
addBlock(
    {
        type: 'dictionary',
        message: '创建字典\n1 %1 = %2\n2 %3 = %4',
        inputs: {
            key1: { type: 'input_value', blockType: 'text', check: 'String', fields: { TEXT: '' } },
            value1: { type: 'input_value' },
            key2: { type: 'input_value', blockType: 'text', check: 'String', fields: { TEXT: '' } },
            value2: { type: 'input_value' },
        },
        inline: true,
        output: 'Dictionary',
        colour: 255,
        tooltip: '创建一个字典（键值对）。点齿轮图标可以增删键值对。',
        helpUrl: '',
        isReporter: true,
        mutator: 'dictionary_mutator',
    },
    (block, generator) => {
        const entries: string[] = [];
        for (let i = 1; block.getInput('key' + i); i++) {
            const key = generator.valueToCode(block, 'key' + i, Order.NONE) || "''";
            const value = generator.valueToCode(block, 'value' + i, Order.NONE) || 'null';
            entries.push(`[${key}, ${value}]`);
        }
        // 直接把键值对解析成 JSON 对象
        return [`Object.fromEntries([${entries.join(', ')}])`, Order.FUNCTION_CALL];
    },
);

/**
 * 字典操作积木：键总数 / 键列表 / 指定键的值 / 添加键值对。
 * 输入都用 check = 'Dictionary'，与主块的 output 类型检查匹配。
 */

// 键总数
addBlock(
    {
        type: 'dictionary_size',
        message: '获取字典键总数 %1',
        inputs: {
            DICT: { type: 'input_value', check: 'Dictionary' },
        },
        inline: false,
        output: 'Number',
        colour: 255,
        tooltip: '返回字典中键的总数',
        isReporter: true,
    },
    (block, generator) => {
        const dict = generator.valueToCode(block, 'DICT', Order.NONE) || '{}';
        return [`Object.keys(${dict}).length`, Order.MEMBER];
    },
);

// 键列表
addBlock(
    {
        type: 'dictionary_keys',
        message: '获取字典键列表 %1',
        inputs: {
            DICT: { type: 'input_value', check: 'Dictionary' },
        },
        inline: false,
        output: 'Array',
        colour: 255,
        tooltip: '返回字典中所有键组成的列表',
        isReporter: true,
    },
    (block, generator) => {
        const dict = generator.valueToCode(block, 'DICT', Order.NONE) || '{}';
        return [`Object.keys(${dict})`, Order.FUNCTION_CALL];
    },
);

// 指定键的值
addBlock(
    {
        type: 'dictionary_get',
        message: '获取字典 %1 中指定键的值 %2',
        inputs: {
            DICT: { type: 'input_value', check: 'Dictionary' },
            KEY: { type: 'input_value', blockType: 'text', check: 'String', fields: { TEXT: '' } },
        },
        inline: false,
        output: '',
        colour: 255,
        tooltip: '返回字典中指定键对应的值',
        isReporter: true,
    },
    (block, generator) => {
        const dict = generator.valueToCode(block, 'DICT', Order.NONE) || '{}';
        const key = generator.valueToCode(block, 'KEY', Order.NONE) || "''";
        return [`(${dict})[${key}]`, Order.MEMBER];
    },
);

// 添加键值对
addBlock(
    {
        type: 'dictionary_set',
        message: '字典 %1 添加键 %2 等于值 %3',
        inputs: {
            DICT: { type: 'input_value', check: 'Dictionary' },
            KEY: { type: 'input_value', blockType: 'text', check: 'String', fields: { TEXT: '' } },
            VALUE: { type: 'input_value' },
        },
        inline: false,
        output: 'Dictionary',
        colour: 255,
        tooltip: '向字典添加（或覆盖）一个键值对，返回新字典',
        isReporter: true,
    },
    (block, generator) => {
        const dict = generator.valueToCode(block, 'DICT', Order.NONE) || '{}';
        const key = generator.valueToCode(block, 'KEY', Order.NONE) || "''";
        const value = generator.valueToCode(block, 'VALUE', Order.NONE) || 'null';
        // 对象字面量自带括号并视为原子：任何上下文（包括语句位置）都不会被误解析
        return [`({...(${dict}), [${key}]: ${value}})`, Order.ATOMIC];
    },
);
