import * as Blockly from 'blockly';
import { javascriptGenerator } from 'blockly/javascript';
import { save, load } from './serialization';
import { toolbox } from './toolbox';
import blocklyLangZhHans from './langs/zh-hans';
import { type BlocklyBlockDefinition, data, type GeneratorFunction, setup } from './blockGenerator';
import type { StaticCategoryInfo } from './types/toolbox';

const exampleBlocks: BlocklyBlockDefinition[] = [];
const exampleForBlocks: Record<string, GeneratorFunction> = {};
const exampleCategory: StaticCategoryInfo = {
    kind: 'category',
    name: 'Example',
    categorystyle: 'my_category',
    contents: [],
};
setup(exampleForBlocks, exampleCategory, exampleBlocks);

// @ts-ignore
await import('./blocks/example');

toolbox.contents.push(exampleCategory);
Blockly.common.defineBlocks(Blockly.common.createBlockDefinitionsFromJsonArray(exampleBlocks));
Object.assign(javascriptGenerator.forBlock, exampleForBlocks);

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

export const runCode = (workspace: Blockly.Workspace) => {
    console.log(workspace);
    const code = javascriptGenerator.workspaceToCode(workspace);
    console.log(code);
    // eval(code);
};

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
        runCode(workspace);

        workspace.addChangeListener((e: Blockly.Events.Abstract) => {
            if (e.isUiEvent) return;
            save(workspace);
        });
        return workspace;
    }
    return null;
};

export { Blockly };
