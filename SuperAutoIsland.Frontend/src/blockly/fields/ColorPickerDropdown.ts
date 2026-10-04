import * as Blockly from 'blockly/core';
import { createColorPickerPanel, type ColorPickerMode } from './ColorPickerPanel';

/** 下拉框（Blockly DropDownDiv）上添加的类名，用于套用 Fluent 的浮出层样式。 */
const DROPDOWN_CLASS = 'sai-color-dropdown';

const PANEL_BACKGROUND = '#f9f9f9';
const PANEL_BORDER = 'rgba(0, 0, 0, 0.14)';

export interface OpenColorPickerOptions {
    /** 选择器模式，默认为 `full`。 */
    mode?: ColorPickerMode;
    /** 当前颜色。 */
    color?: string | null;
    /** 颜色变化时回调，返回「#AARRGGBB」格式的颜色。 */
    onChange?: (color: string) => void;
}

/**
 * 在字段下方以 Blockly 下拉框的形式打开颜色选择器。
 *
 * 下拉框本身沿用 FluentAvalonia 的浮出层外观（无内边距、圆角、投影、无箭头）。
 */
export function openColorPicker(field: Blockly.Field<string>, options: OpenColorPickerOptions = {}): void {
    // 已经由本字段展开时直接返回，这样再次点击字段就等于收起下拉框。
    if (Blockly.DropDownDiv.getOwner() === field) return;

    // 先无动画关掉上一个下拉框：DropDownDiv 会重建内容和容器节点，必须先关再取。
    closeColorPicker();

    const content = Blockly.DropDownDiv.getContentDiv();
    const outer = content.parentElement as HTMLElement | null;

    const panel = createColorPickerPanel({
        mode: options.mode,
        color: options.color,
        onChange: options.onChange,
        onRequestClose: () => {
            Blockly.DropDownDiv.hideIfOwner(field);
        },
    });

    content.style.padding = '0';
    content.style.borderRadius = '8px';
    content.style.overflow = 'hidden';
    content.appendChild(panel.root);

    if (outer) {
        outer.classList.add(DROPDOWN_CLASS);
        outer.querySelector<HTMLElement>('.blocklyDropDownArrow')?.style.setProperty('display', 'none');
    }
    Blockly.DropDownDiv.setColour(PANEL_BACKGROUND, PANEL_BORDER);

    Blockly.DropDownDiv.showPositionedByField(field, () => {
        panel.dispose();
        content.style.padding = '';
        content.style.borderRadius = '';
        content.style.overflow = '';
        outer?.classList.remove(DROPDOWN_CLASS);
    });
}

/** 关闭当前颜色下拉框。 */
export function closeColorPicker(): void {
    // 用无动画关闭，保证上一次的内容立刻清空，避免新旧面板同时存在。
    if (Blockly.DropDownDiv.isVisible()) Blockly.DropDownDiv.hideWithoutAnimation();
}
