import { addBlock, addLabel } from '../utils/blockGenerator';
import { Order } from 'blockly/javascript';

/**
 * 「类型」积木：类型转换与类型判断。
 *
 * 全部用 JS 代码生成实现（脚本最终跑在后端 Jint 上），不经过 C# 后端接口，
 * 对应的转换 / 判断规则都写在下面的辅助函数里，积木的 tooltip 里也说明了。
 *
 * 都设了 inline: true（inputsInline）：消息是「%1 转成数字」这种「输入在前、文字在后」的形式时，
 * Blockly 会把后面的文字单独做成一个 dummy input，而 blockGenerator 又会把 inline 写成显式的
 * inputsInline: false，结果输入槽和文字各占一行 —— 工具箱里看就是文字上面多一个空行。
 * 设成 true 之后渲染和 Blockly 内置的「%1 is empty」完全一致（一行）。
 */

addLabel('类型');

// 转数字
addBlock(
    {
        type: 'type_to_number',
        message: '%1 转成数字',
        inputs: {
            VALUE: {
                type: 'input_value',
            },
        },
        inline: true,
        style: 'my_blocks',
        output: 'Number',
        tooltip:
            '按 JavaScript 的 Number 规则把值转成数字：文本会整体解析（"12abc" 这种得到 NaN），空文本、null 得到 0，true / false 得到 1 / 0。',
        isReporter: true,
    },
    (block, generator) => {
        const value = generator.valueToCode(block, 'VALUE', Order.NONE) || "''";
        const wrapper = generator.provideFunction_(
            'type_to_number',
            `function ${generator.FUNCTION_NAME_PLACEHOLDER_}(value) {
                return Number(value);
            }`,
        );
        return [`${wrapper}(${value})`, Order.FUNCTION_CALL];
    },
);

// 转布尔值
addBlock(
    {
        type: 'type_to_boolean',
        message: '%1 转成布尔值',
        inputs: {
            VALUE: {
                type: 'input_value',
            },
        },
        inline: true,
        style: 'my_blocks',
        output: 'Boolean',
        tooltip:
            '把值转成 true / false：文本为空、0、false、no、null、undefined、nan 时是 false，其余文本都是 true；文本以外的值按 JavaScript 的 Boolean 规则。',
        isReporter: true,
    },
    (block, generator) => {
        const value = generator.valueToCode(block, 'VALUE', Order.NONE) || "''";
        const wrapper = generator.provideFunction_(
            'type_to_boolean',
            `function ${generator.FUNCTION_NAME_PLACEHOLDER_}(value) {
                if (typeof value === 'string') {
                    const text = value.trim().toLowerCase();
                    if (
                        text === '' ||
                        text === '0' ||
                        text === 'false' ||
                        text === 'no' ||
                        text === 'null' ||
                        text === 'undefined' ||
                        text === 'nan'
                    ) {
                        return false;
                    }
                    return true;
                }
                return Boolean(value);
            }`,
        );
        return [`${wrapper}(${value})`, Order.FUNCTION_CALL];
    },
);

// 是数字？
addBlock(
    {
        type: 'type_is_number',
        message: '%1 是数字？',
        inputs: {
            VALUE: {
                type: 'input_value',
            },
        },
        inline: true,
        style: 'my_blocks',
        output: 'Boolean',
        tooltip: '判断值是不是数字，NaN 不算；文本形式的数字（"123"）也不算。',
        isReporter: true,
    },
    (block, generator) => {
        const value = generator.valueToCode(block, 'VALUE', Order.NONE) || "''";
        const wrapper = generator.provideFunction_(
            'type_is_number',
            `function ${generator.FUNCTION_NAME_PLACEHOLDER_}(value) {
                return typeof value === 'number' && !isNaN(value);
            }`,
        );
        return [`${wrapper}(${value})`, Order.FUNCTION_CALL];
    },
);

// 是布尔值？
addBlock(
    {
        type: 'type_is_boolean',
        message: '%1 是布尔值？',
        inputs: {
            VALUE: {
                type: 'input_value',
            },
        },
        inline: true,
        style: 'my_blocks',
        output: 'Boolean',
        tooltip: '判断值是不是 true / false 这种布尔值。',
        isReporter: true,
    },
    (block, generator) => {
        const value = generator.valueToCode(block, 'VALUE', Order.NONE) || "''";
        const wrapper = generator.provideFunction_(
            'type_is_boolean',
            `function ${generator.FUNCTION_NAME_PLACEHOLDER_}(value) {
                return typeof value === 'boolean';
            }`,
        );
        return [`${wrapper}(${value})`, Order.FUNCTION_CALL];
    },
);

// 是文本？
addBlock(
    {
        type: 'type_is_string',
        message: '%1 是文本？',
        inputs: {
            VALUE: {
                type: 'input_value',
            },
        },
        inline: true,
        style: 'my_blocks',
        output: 'Boolean',
        tooltip: '判断值是不是文本（字符串）。',
        isReporter: true,
    },
    (block, generator) => {
        const value = generator.valueToCode(block, 'VALUE', Order.NONE) || "''";
        const wrapper = generator.provideFunction_(
            'type_is_string',
            `function ${generator.FUNCTION_NAME_PLACEHOLDER_}(value) {
                return typeof value === 'string';
            }`,
        );
        return [`${wrapper}(${value})`, Order.FUNCTION_CALL];
    },
);

// 是列表？
addBlock(
    {
        type: 'type_is_array',
        message: '%1 是列表？',
        inputs: {
            VALUE: {
                type: 'input_value',
            },
        },
        inline: true,
        style: 'my_blocks',
        output: 'Boolean',
        tooltip: '判断值是不是列表（数组）。',
        isReporter: true,
    },
    (block, generator) => {
        const value = generator.valueToCode(block, 'VALUE', Order.NONE) || "''";
        const wrapper = generator.provideFunction_(
            'type_is_array',
            `function ${generator.FUNCTION_NAME_PLACEHOLDER_}(value) {
                return Array.isArray(value);
            }`,
        );
        return [`${wrapper}(${value})`, Order.FUNCTION_CALL];
    },
);
