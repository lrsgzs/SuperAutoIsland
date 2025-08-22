import { BlocklyBlockDefinition, GeneratorFunction, setup } from './blockGenerator';
import type { StaticCategoryInfo } from '../types/toolbox';
import { toolbox } from '../toolbox';
import * as Blockly from 'blockly';
import { javascriptGenerator } from 'blockly/javascript';

interface CategoryData {
    blocks?: BlocklyBlockDefinition[];
    forBlocks?: Record<string, GeneratorFunction>;
    category?: StaticCategoryInfo;
    initialized: boolean;
}

export const settingUpCategory: CategoryData = {
    initialized: false,
};

export function preSetupCategory(name: string) {
    settingUpCategory.blocks = [];
    settingUpCategory.forBlocks = {};
    settingUpCategory.category = {
        kind: 'category',
        name: name,
        categorystyle: 'my_category',
        contents: [],
    };
    settingUpCategory.initialized = true;
    setup(settingUpCategory.forBlocks, settingUpCategory.category, settingUpCategory.blocks);
}

export function postSetupCategory() {
    if (!settingUpCategory.initialized) throw new Error('还没初始化呢！你先别急');

    toolbox.contents.push(settingUpCategory.category!);
    Blockly.common.defineBlocks(Blockly.common.createBlockDefinitionsFromJsonArray(settingUpCategory.blocks!));
    Object.assign(javascriptGenerator.forBlock, settingUpCategory.forBlocks);
    console.log(`${settingUpCategory.category!.name} 初始化完成！settingUpCategory:`, { ...settingUpCategory });
    delete settingUpCategory.category;
    delete settingUpCategory.blocks;
    delete settingUpCategory.forBlocks;
    settingUpCategory.initialized = false;
}
