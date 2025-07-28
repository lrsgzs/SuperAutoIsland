import * as Blockly from 'blockly';
import { javascriptGenerator } from 'blockly/javascript';
import { save, load } from './serialization';
import { toolbox } from './toolbox';
import blocklyLangZhHans from './langs/zh-hans';
import { preSetupCategory, postSetupCategory, settingUpCategory } from './utils/quickSetup';

import * as prettier from 'prettier';
import * as prettierEstreePlugin from 'prettier/plugins/estree';
import * as prettierBabelPlugin from 'prettier/plugins/babel';

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

const callActionDefinition = `function callAction(id, data) { console.log("Calling Action:", id, data) }`;
const getRuleStateDefinition = `function getRuleState(id, data) { console.log("Getting Rule State:", id, data); return false; }`;

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

export const runCode = async (workspace: Blockly.Workspace) => {
    console.log(workspace);
    let code = javascriptGenerator.workspaceToCode(workspace);
    code = `${callActionDefinition}\n${getRuleStateDefinition}\n` + code;
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

Reflect.set(window, 'runCode', runCode);

export const injectBlockly = (dom: HTMLElement) => {
    const workspace = Blockly.inject(dom, {
        toolbox,
        zoom: { controls: true },
        media: './media/',
        theme: defaultTheme,
    }) as Blockly.Workspace;

    Reflect.set(window, 'workspace', workspace);
    Reflect.set(window, 'javascriptGenerator', javascriptGenerator);
    Reflect.set(window, 'Blockly', Blockly);

    if (workspace) {
        load(workspace);
        // runCode(workspace);

        workspace.addChangeListener((e: Blockly.Events.Abstract) => {
            if (e.isUiEvent) return;
            save(workspace);
        });
        return workspace;
    }
    return null;
};

export { Blockly };
