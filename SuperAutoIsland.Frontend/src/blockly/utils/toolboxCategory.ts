import * as Blockly from 'blockly';

/**
 * @fileoverview 带图标的工具箱分类。
 *
 * Blockly 自带的分类图标只在分类被选中时显示一个箭头，这里替换成后端分类元数据里的
 * 图标（分类提供方通过 `CategoryMetadata.Icon` 提供，默认是「分类 + 齿轮」）。
 */

/** 使用分类元数据图标的分类类型名，作为工具箱 JSON 里的 `kind`。 */
export const SAI_CATEGORY_KIND = 'sai_category';

/**
 * 内置分类（Blockly 自带分类 + SAI 自己建的分类）的图标字形。
 *
 * 码位取自 Fluent 图标字体 `src/assets/fonts/fluent.json`，注释里是对应的图标名。
 */
export const CategoryGlyph = {
    /** ic_fluent_branch_fork_20_regular */
    logic: '\uE2A6',
    /** ic_fluent_arrow_repeat_all_20_regular */
    loop: '\uE125',
    /** ic_fluent_math_symbols_20_regular */
    math: '\uEB6A',
    /** ic_fluent_text_font_20_regular */
    text: '\uF26F',
    /** ic_fluent_list_20_regular */
    list: '\uEAC0',
    /** ic_fluent_braces_variable_20_regular */
    variable: '\uE29A',
    /** ic_fluent_math_formula_20_regular */
    procedure: '\uEB66',
    /** ic_fluent_calendar_ltr_20_regular */
    date: '\uE330',
    /** ic_fluent_book_database_20_regular */
    dict: '\uE223',
    /** ic_fluent_puzzle_piece_20_regular */
    misc: '\uEE29',
    /** ic_fluent_bug_20_regular */
    debug: '\uE2C8',
} as const;

/** 分类名称 -> 元数据里的图标 */
const categoryIcons = new Map<string, { glyph: string; label: string }>();

/**
 * 记录一个分类的图标（来自后端分类元数据）
 *
 * @param name 分类名称
 * @param icon `[名称, 字形]`，字形为空时清除图标
 */
export function setCategoryIcon(name: string, icon?: readonly [string, string] | null): void {
    if (!icon?.[1]) {
        categoryIcons.delete(name);
        return;
    }
    categoryIcons.set(name, { glyph: icon[1], label: icon[0] });
}

/**
 * 渲染分类元数据图标的工具箱分类
 */
export class SaiToolboxCategory extends Blockly.ToolboxCategory {
    protected override createIconDom_(): Element {
        const icon = document.createElement('span');
        icon.className = 'sai-toolbox-category-icon';

        const entry = categoryIcons.get(this.name_);
        if (entry) {
            icon.textContent = entry.glyph;
            icon.title = entry.label;
        } else {
            // 没有图标时留出同样的占位，避免分类标题左右跳动
            icon.classList.add('sai-toolbox-category-icon-empty');
        }

        return icon;
    }
}

Blockly.registry.register(Blockly.registry.Type.TOOLBOX_ITEM, SAI_CATEGORY_KIND, SaiToolboxCategory);
