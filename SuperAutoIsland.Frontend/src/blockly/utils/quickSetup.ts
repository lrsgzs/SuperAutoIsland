import { BlocklyBlockDefinition, GeneratorFunction, setup } from './blockGenerator';
import type { StaticCategoryInfo } from '../types/toolbox';
import { SAI_CATEGORY_KIND, setCategoryIcon } from './toolboxCategory';
import { toolbox } from '../toolbox';
import * as Blockly from 'blockly';
import { javascriptGenerator } from 'blockly/javascript';

/**
 * 类别数据接口
 */
interface CategoryData {
    blocks?: BlocklyBlockDefinition[];
    forBlocks?: Record<string, GeneratorFunction>;
    category?: StaticCategoryInfo;
    initialized: boolean;
}

export const settingUpCategory: CategoryData = {
    initialized: false,
};

/**
 * 初始化当前设置的类别
 * @param name 类别名称
 * @param style 类别样式
 * @param icon 分类图标 `[名称, 字形]`，见 utils/toolboxCategory.ts
 */
export function preSetupCategory(
    name: string,
    style: string = 'my_category',
    icon?: readonly [string, string] | null,
) {
    setCategoryIcon(name, icon);
    settingUpCategory.blocks = [];
    settingUpCategory.forBlocks = {};
    settingUpCategory.category = {
        kind: SAI_CATEGORY_KIND,
        name: name,
        categorystyle: style,
        contents: [],
    };
    settingUpCategory.initialized = true;
    setup(settingUpCategory.forBlocks, settingUpCategory.category, settingUpCategory.blocks);
}

/**
 * 结束定义当前设置的类别
 */
export function postSetupCategory() {
    if (!settingUpCategory.initialized) throw new Error('还没初始化呢！你先别急');

    toolbox.contents.push(settingUpCategory.category!);
    Blockly.common.defineBlocks(Blockly.common.createBlockDefinitionsFromJsonArray(settingUpCategory.blocks!));
    Object.assign(javascriptGenerator.forBlock, settingUpCategory.forBlocks);
    console.log(`${settingUpCategory.category!.name} 初始化完成！当前分类:`, { ...settingUpCategory });
    delete settingUpCategory.category;
    delete settingUpCategory.blocks;
    delete settingUpCategory.forBlocks;
    settingUpCategory.initialized = false;
}
