import { setup } from './blockGenerator';
import { toolbox } from '../toolbox';
import * as Blockly from 'blockly';
import { javascriptGenerator } from 'blockly/javascript';
export const settingUpCategory = {
    initialized: false,
};
/**
 * 初始化当前设置的类别
 * @param name 类别名称
 */
export function preSetupCategory(name) {
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
/**
 * 结束定义当前设置的类别
 */
export function postSetupCategory() {
    if (!settingUpCategory.initialized)
        throw new Error('还没初始化呢！你先别急');
    toolbox.contents.push(settingUpCategory.category);
    Blockly.common.defineBlocks(Blockly.common.createBlockDefinitionsFromJsonArray(settingUpCategory.blocks));
    Object.assign(javascriptGenerator.forBlock, settingUpCategory.forBlocks);
    console.log(`${settingUpCategory.category.name} 初始化完成！settingUpCategory:`, { ...settingUpCategory });
    delete settingUpCategory.category;
    delete settingUpCategory.blocks;
    delete settingUpCategory.forBlocks;
    settingUpCategory.initialized = false;
}
