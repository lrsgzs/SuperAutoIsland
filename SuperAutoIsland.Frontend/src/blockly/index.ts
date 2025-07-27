import * as Blockly from 'blockly';
import { javascriptGenerator } from 'blockly/javascript';
import { save, load } from './serialization';
import { toolbox } from './toolbox';
import blocklyLangZhHans from './langs/zh-hans';
import { type BlocklyBlockDefinition, data, type GeneratorFunction, setup } from './utils/blockGenerator';
import type { StaticCategoryInfo } from './types/toolbox';

import * as prettier from 'prettier';
import * as prettierEstreePlugin from 'prettier/plugins/estree';
import * as prettierBabelPlugin from 'prettier/plugins/babel';

const actionBlocks: BlocklyBlockDefinition[] = [];
const actionForBlocks: Record<string, GeneratorFunction> = {};
const actionCategory: StaticCategoryInfo = {
    kind: 'category',
    name: '行动',
    categorystyle: 'my_category',
    contents: [],
};
setup(actionForBlocks, actionCategory, actionBlocks);

// @ts-ignore
await import('./blocks/action');

toolbox.contents.push(actionCategory);
Blockly.common.defineBlocks(Blockly.common.createBlockDefinitionsFromJsonArray(actionBlocks));
Object.assign(javascriptGenerator.forBlock, actionForBlocks);

Blockly.setLocale(blocklyLangZhHans);
Blockly.ContextMenuItems.registerCommentOptions();

console.log('Blockly 初始化完成！blockGenerator data:');
console.log(data);

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
    code = `function callAction(id, data) { console.log("Calling Action:", id, data) }\n` + code;
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
