/**
 * @fileoverview FluentAvalonia 预设颜色。
 *
 * 数据取自 FluentAvalonia 中 `ColorView` 的默认调色板——Avalonia 的
 * `FluentColorPalette`（`ColorCount = 6`、`ShadeCount = 8`），即 WinUI
 * 颜色选择器「调色板」标签页里的那 48 种颜色。
 *
 * 与 FluentAvalonia 一致，网格按每行 8 列排布：每一行是一组同色系的颜色，
 * 每一列是同色系由亮到暗的不同深浅。
 */

/** 调色板的列数，与 FluentAvalonia 的 `ColorView.PaletteColumnCount` 一致。 */
export const FLUENT_PALETTE_COLUMNS = 8;

/**
 * 预设颜色（6 组 × 8 种），颜色格式为「#AARRGGBB」。
 */
export const FLUENT_PALETTE_SHADES: readonly (readonly string[])[] = [
    ['#FFFF4343', '#FFD13438', '#FFEF6950', '#FFDA3B01', '#FFCA5010', '#FFF7630C', '#FFFF8C00', '#FFFFB900'],
    ['#FFE74856', '#FFE81123', '#FFEA005E', '#FFC30052', '#FFE3008C', '#FFBF0077', '#FFC239B3', '#FF9A0089'],
    ['#FF0078D7', '#FF0063B1', '#FF8E8CD8', '#FF6B69D6', '#FF8764B8', '#FF744DA9', '#FFB146C2', '#FF881798'],
    ['#FF0099BC', '#FF2D7D9A', '#FF00B7C3', '#FF038387', '#FF00B294', '#FF018574', '#FF00CC6A', '#FF10893E'],
    ['#FF7A7574', '#FF5D5A50', '#FF68768A', '#FF515C6B', '#FF567C73', '#FF486860', '#FF498205', '#FF107C10'],
    ['#FF767676', '#FF4C4A48', '#FF69797E', '#FF4A5459', '#FF647C64', '#FF525E54', '#FF847545', '#FF7E735F'],
];

/** 展平后的预设颜色列表，按行优先顺序排列，共 48 种。 */
export const FLUENT_PALETTE: readonly string[] = FLUENT_PALETTE_SHADES.flat();

/** 预设颜色的默认值，与 FluentAvalonia 调色板的第一种颜色一致。 */
export const DEFAULT_PRESET_COLOR = FLUENT_PALETTE[0];
