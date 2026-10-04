import type { Block } from 'blockly';
import * as Blockly from 'blockly';
import { type JavascriptGenerator, javascriptGenerator, Order } from 'blockly/javascript';
import { toolbox } from './toolbox';
import blocklyLangZhHans from './langs/zh-hans';

import { textMultiline } from '@blockly/field-multilineinput';
import { shadowBlockConversionChangeListener } from '@blockly/shadow-block-converter';
import Theme from '@blockly/theme-modern';
import '@blockly/field-date';
import { installBackpack } from './utils/backpackStorage';
import './fields/FieldTime';
import { FieldColor } from './fields/FieldColor';
import { FieldIcon } from './fields/FieldIcon';
import { FieldIconPicker } from './fields/FieldIconPicker';
import { postSetupCategory, preSetupCategory } from './utils/quickSetup';
import { CategoryGlyph } from './utils/toolboxCategory';
import { categoryBlockStyles } from './utils/categoryColors';
import { addLabel } from './utils/blockGenerator';
import { wsWaitMessage } from './utils/wsUtils';
import { v4 as uuid } from 'uuid';
import './types/extraData.d.ts';

import * as prettier from 'prettier/standalone';
import * as prettierEstreePlugin from 'prettier/plugins/estree';
import * as prettierBabelPlugin from 'prettier/plugins/babel';
import { addV2Block, type CategoryContent } from './utils/v2Generator';

Blockly.fieldRegistry.register('field_icon', FieldIcon);
Blockly.fieldRegistry.register('field_icon_picker', FieldIconPicker);
Blockly.fieldRegistry.register('field_colour', FieldColor);
Blockly.fieldRegistry.register('field_colour_hsv_sliders', FieldColor);

const ws = new WebSocket('/');
await new Promise(resolve => {
    setTimeout(resolve, 500);
});

const data = await wsWaitMessage<{ blocksString: string }>(ws, { type: 'getCategories' });
window.extraBlocks = JSON.parse(data.blocksString) as CategoryContent[];
window.saiWS = ws;
window.saiWaitMessage = wsWaitMessage;

// modify procedures interaction
javascriptGenerator.forBlock['procedures_defreturn'] = function (block: Block, generator: JavascriptGenerator) {
    // Define a procedure with a return value.
    const funcName = generator.getProcedureName(block.getFieldValue('NAME'));
    let xfix1 = '';
    if (generator.STATEMENT_PREFIX) {
        xfix1 += generator.injectId(generator.STATEMENT_PREFIX, block);
    }
    if (generator.STATEMENT_SUFFIX) {
        xfix1 += generator.injectId(generator.STATEMENT_SUFFIX, block);
    }
    if (xfix1) {
        xfix1 = generator.prefixLines(xfix1, generator.INDENT);
    }
    let loopTrap = '';
    if (generator.INFINITE_LOOP_TRAP) {
        loopTrap = generator.prefixLines(generator.injectId(generator.INFINITE_LOOP_TRAP, block), generator.INDENT);
    }
    let branch = '';
    if (block.getInput('STACK')) {
        // The 'procedures_defreturn' block might not have a STACK input.
        branch = generator.statementToCode(block, 'STACK');
    }
    let returnValue = '';
    if (block.getInput('RETURN')) {
        // The 'procedures_defnoreturn' block (which shares this code)
        // does not have a RETURN input.
        returnValue = generator.valueToCode(block, 'RETURN', Order.NONE) || '';
    }
    let xfix2 = '';
    if (branch && returnValue) {
        // After executing the function body, revisit this block for the return.
        xfix2 = xfix1;
    }
    if (returnValue) {
        returnValue = generator.INDENT + 'return ' + returnValue + ';\n';
    }
    const args = [];
    const variables = block.getVarModels();
    for (let i = 0; i < variables.length; i++) {
        args[i] = generator.getVariableName(variables[i].getId());
    }
    let code =
        'async function ' +
        funcName +
        '(' +
        args.join(', ') +
        ') {\n' +
        xfix1 +
        loopTrap +
        branch +
        xfix2 +
        returnValue +
        '}';
    code = generator.scrub_(block, code);
    // Add % so as not to collide with helper functions in definitions list.
    // CodeGenerator declaring .definitions protected.
    (generator as any).definitions_['%' + funcName] = code;
    return null;
};

javascriptGenerator.forBlock['procedures_defnoreturn'] = javascriptGenerator.forBlock['procedures_defreturn'];

javascriptGenerator.forBlock['procedures_callreturn'] = function (
    block: Block,
    generator: JavascriptGenerator,
): [string, Order] {
    // Call a procedure with a return value.
    const funcName = generator.getProcedureName(block.getFieldValue('NAME'));
    const args = [];
    const variables = block.getVarModels();
    for (let i = 0; i < variables.length; i++) {
        args[i] = generator.valueToCode(block, 'ARG' + i, Order.NONE) || 'null';
    }
    const code = 'await ' + funcName + '(' + args.join(', ') + ')';
    return [code, Order.FUNCTION_CALL];
};

preSetupCategory('日期与时间', {
    style: 'date_blocks',
    icon: ['日期与时间', CategoryGlyph.date],
    colors: { primary: '#A6A65B', secondary: '#C8C87D', tertiary: '#848448' },
});
// @ts-ignore
await import('./blocks/date');
postSetupCategory();

preSetupCategory('字典', {
    style: 'dict_blocks',
    icon: ['字典', CategoryGlyph.dict],
    colors: { primary: '#5B6EA6' },
});
// @ts-ignore
await import('./blocks/dict');
postSetupCategory();

preSetupCategory('杂项', {
    style: 'my_blocks',
    icon: ['杂项', CategoryGlyph.misc],
    // 只给主色，secondary / tertiary 按规则推导
    colors: { primary: '#0078D7' },
});
// @ts-ignore
await import('./blocks/colour');
// @ts-ignore
await import('./blocks/icon');
postSetupCategory();

preSetupCategory('调试', {
    style: 'debug_blocks',
    icon: ['调试', CategoryGlyph.debug],
    colors: { primary: '#666666', secondary: '#888888', tertiary: '#444444' },
});
// @ts-ignore
await import('./blocks/debug');
postSetupCategory();

toolbox.contents.push({
    kind: 'sep',
});

// 后端送来的分类：使用完整分类元数据（名称 + 图标 + 配色）建立 Blockly 分类
for (const category of window.extraBlocks) {
    preSetupCategory(category.metadata.name, {
        icon: category.metadata.icon,
        colors: category.metadata.colors,
    });

    for (const block of category.blocks) {
        await addV2Block(block);
    }

    if (category.blocks.length == 0) {
        addLabel('滚木分类？');
    }

    postSetupCategory();
}

textMultiline.installBlock({
    javascript: javascriptGenerator,
});
Blockly.setLocale(blocklyLangZhHans);
Blockly.ContextMenuItems.registerCommentOptions();

// 积木样式由各个分类的配色决定（preSetupCategory 里注册，见 utils/categoryColors.ts），
// 分类行的颜色直接用主色，所以这里不再需要 categoryStyles
const defaultTheme = Blockly.Theme.defineTheme('default', {
    base: Theme,
    name: 'default',
    blockStyles: categoryBlockStyles,
});

let projectUuid = new URLSearchParams(location.search).get('id') || '';
if (projectUuid === '') {
    location.href = `/?id=${uuid()}`;
}

const callActionDefinition = `
async function callAction(id, data) {
    console.log("Calling Action:", id, data);
    await window.saiWaitMessage(window.saiWS, {
        type: "runAction",
        id: id, 
        settings: data,
    });
}`;

const getRuleStateDefinition = `
async function getRuleState(id, data) {
    console.log("Getting Rule State:", id, data);
    const result = await window.saiWaitMessage(window.saiWS, {
        type: "runRule",
        id: id,
        settings: data,
    });
    return result.result;
}`;

const getDataDefinition = `
async function getData(id, data) {
    console.log("Getting Data:", id, data);
    const result = await window.saiWaitMessage(window.saiWS, {
        type: "runData",
        id: id,
        settings: data,
    });
    return result.data;
}`;

/**
 * 运行代码
 * @param workspace 工作区实例
 */
export const runCode = async (workspace: Blockly.Workspace = window.workspace) => {
    console.log(workspace);
    let code = javascriptGenerator.workspaceToCode(workspace);
    code = `${callActionDefinition}\n${getRuleStateDefinition}\n${getDataDefinition}\n\n` + code;
    code = `(async () => {\n${code}\n})();\n`;
    code = await prettier.format(code, {
        semi: true,
        singleQuote: true,
        trailingComma: 'all',
        parser: 'babel',
        plugins: [prettierEstreePlugin, prettierBabelPlugin],
    });
    console.log(code);
    eval(code);
};
window.runCode = runCode;

/**
 * 保存代码
 * @param workspace 工作区实例
 */
export const saveCode = async (workspace: Blockly.Workspace = window.workspace) => {
    console.log(workspace);
    let code = javascriptGenerator.workspaceToCode(workspace);
    code = `(async () => {\n${code}\n})();\n`;
    code = await prettier.format(code, {
        semi: true,
        singleQuote: true,
        trailingComma: 'all',
        parser: 'babel',
        plugins: [prettierEstreePlugin, prettierBabelPlugin],
    });
    console.log(code);

    await wsWaitMessage(ws, {
        type: 'save',
        data: {
            type: 'blocklyAction',
            guid: projectUuid,
            workspace: JSON.stringify(Blockly.serialization.workspaces.save(workspace)),
            code: code,
        },
    });
};
window.saveCode = saveCode;

/**
 * 注入 blockly
 * @param dom 要注入的 dom 元素
 */
export const injectBlockly = async (dom: HTMLElement) => {
    const workspace = Blockly.inject(dom, {
        toolbox,
        zoom: { controls: true },
        media: './media/',
        theme: defaultTheme,
    }) as Blockly.WorkspaceSvg;

    // 背包内容存在 localStorage 里，不写进项目存档
    const backpack = installBackpack(workspace);
    workspace.addChangeListener(shadowBlockConversionChangeListener);

    window.workspace = workspace;
    Reflect.set(window, 'javascriptGenerator', javascriptGenerator);
    Reflect.set(window, 'Blockly', Blockly);

    if (workspace) {
        let data: { type: string; workspace: string; guid: string };
        try {
            data = await wsWaitMessage<{ workspace: string; guid: string }>(ws, { type: 'load', guid: projectUuid });
            if (data.guid !== projectUuid) {
                location.href = `/?id=${data.guid}`;
            }

            Blockly.Events.disable();
            Blockly.serialization.workspaces.load(JSON.parse(data.workspace), workspace, undefined);
            Blockly.Events.enable();
        } catch (error) {
            console.error('Load Failed!', error);
        }

        return workspace;
    }
    return null;
};

// @ts-ignore
export { Blockly };
// @ts-ignore
// nothing
