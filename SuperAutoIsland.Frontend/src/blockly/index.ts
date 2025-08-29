import * as Blockly from 'blockly';
import { javascriptGenerator } from 'blockly/javascript';
import { toolbox } from './toolbox';
import blocklyLangZhHans from './langs/zh-hans';
import { preSetupCategory, postSetupCategory } from './utils/quickSetup';
import type { Metadata } from './utils/superGenerator';
import { wsWaitMessage } from './utils/wsUtils';
import { v4 as uuid } from 'uuid';
import './types/extraData.d.ts';

import * as prettier from 'prettier';
import * as prettierEstreePlugin from 'prettier/plugins/estree';
import * as prettierBabelPlugin from 'prettier/plugins/babel';

import { FieldIcon } from './fields/FieldIcon';
Blockly.fieldRegistry.register('field_icon', FieldIcon);

const ws = new WebSocket('/');
ws.addEventListener('message', ev => console.log(ev));
await new Promise(resolve => {
    setTimeout(resolve, 500);
});

const data = await wsWaitMessage<{ blocksString: string }>(ws, { type: 'getExtraBlocks' });
window.extraBlocks = JSON.parse(data.blocksString) as Record<string, Record<'rules' | 'actions', Metadata[]>>;
window.saiWS = ws;
window.saiWaitMessage = wsWaitMessage;

preSetupCategory('规则');
// @ts-ignore
await import('./blocks/rules');
postSetupCategory();

preSetupCategory('行动');
// @ts-ignore
await import('./blocks/actions');
postSetupCategory();

preSetupCategory('调试');
// @ts-ignore
await import('./blocks/debug');
postSetupCategory();

Blockly.setLocale(blocklyLangZhHans);
Blockly.ContextMenuItems.registerCommentOptions();

let projectUuid = new URLSearchParams(location.search).get("id") || "";
if (projectUuid === "") {
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

const defaultTheme = Blockly.Theme.defineTheme('default', {
    base: Blockly.Themes.Classic,
    name: 'default',
    blockStyles: {
        my_blocks: {
            colourPrimary: '#00AAFF',
            colourSecondary: '#00C2FF',
            colourTertiary: '#007cb8',
        },
    },
    categoryStyles: {
        my_category: {
            colour: '#00AAFF',
        },
    },
});

/**
 * 运行代码
 * @param workspace 工作区实例
 */
export const runCode = async (workspace: Blockly.Workspace = window.workspace) => {
    console.log(workspace);
    let code = javascriptGenerator.workspaceToCode(workspace);
    code = `${callActionDefinition}\n${getRuleStateDefinition}\n\n` + code;
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

    await wsWaitMessage(ws, { type: "save",
        data: {
            type: "blocklyAction",
            guid: projectUuid,
            workspace: JSON.stringify(Blockly.serialization.workspaces.save(workspace)),
            code: code
        }
    })
}
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
    }) as Blockly.Workspace;

    window.workspace = workspace;
    Reflect.set(window, 'javascriptGenerator', javascriptGenerator);
    Reflect.set(window, 'Blockly', Blockly);

    if (workspace) {
        let data: { type: string; workspace: string; guid: string; };
        try {
            data = await wsWaitMessage<{ workspace: string; guid: string; }>(ws, { type: "load", guid: projectUuid });
            if (data.guid !== projectUuid) {
                location.href = `/?id=${data.guid}`;
            }

            Blockly.Events.disable();
            Blockly.serialization.workspaces.load(JSON.parse(data.workspace), workspace, undefined);
            Blockly.Events.enable();
        } catch {
            console.log("Load Failed!")
        }

        return workspace;
    }
    return null;
};

// @ts-ignore
export { Blockly };
// @ts-ignore
// nothing
