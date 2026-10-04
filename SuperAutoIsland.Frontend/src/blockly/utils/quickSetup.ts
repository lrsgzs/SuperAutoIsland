import { BlocklyBlockDefinition, GeneratorFunction, setup } from './blockGenerator';
import type { StaticCategoryInfo } from '../types/toolbox';
import { SAI_CATEGORY_KIND, setCategoryIcon } from './toolboxCategory';
import { type CategoryColorsInput, registerCategoryColors } from './categoryColors';
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

/** 建立一个分类时可以指定的东西 */
export interface CategorySetupOptions {
    /**
     * 分类下积木用的样式名（内部会按这个名字注册三个颜色）。
     * 不给就自动生成一个；自己写死样式名的积木模块需要跟这里对上。
     */
    style?: string;
    /** 分类图标 `[名称, 字形]`，见 utils/toolboxCategory.ts */
    icon?: readonly [string, string] | null;
    /** 分类配色（积木的三个颜色），见 utils/categoryColors.ts */
    colors?: CategoryColorsInput | null;
}

export const settingUpCategory: CategoryData = {
    initialized: false,
};

/**
 * 初始化当前设置的类别
 * @param name 类别名称
 * @param options 图标、配色、样式名
 */
export function preSetupCategory(name: string, options: CategorySetupOptions = {}) {
    const { styleName, colors } = registerCategoryColors(options.colors, options.style);

    setCategoryIcon(name, options.icon);
    settingUpCategory.blocks = [];
    settingUpCategory.forBlocks = {};
    settingUpCategory.category = {
        kind: SAI_CATEGORY_KIND,
        name: name,
        // 分类行的颜色直接用主色（分类样式只能给一个颜色，没必要再注册一份）
        colour: colors.primary,
        contents: [],
    };
    settingUpCategory.initialized = true;
    setup(settingUpCategory.forBlocks, settingUpCategory.category, settingUpCategory.blocks, styleName);
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
