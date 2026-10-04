import type * as Blockly from 'blockly';

/**
 * @fileoverview 分类配色。
 *
 * Blockly 的积木由一个「三色」样式来画（`colourPrimary` / `colourSecondary` / `colourTertiary`），
 * 插件可以在分类元数据里指定这三个颜色，这里负责把它们归一化后注册成 Blockly 主题里的积木样式。
 *
 * 色调要求：三个颜色必须取自同一色相，只是明度不同（跟 Blockly 官方 modern 主题一致）
 * - 主色 primary：积木填充色，同时用作分类行的颜色
 * - 阴影色 secondary：同色相的浅色，默认取「主色与白色混 60%」
 * - 描边色 tertiary：同色相的深色，默认取「主色 × 0.8」（暗 20%）
 *
 * 颜色写法为 `#AARRGGBB` 或 `#RRGGBB`；Blockly 只认不带 Alpha 的写法，而且 CSS 会把
 * `#AARRGGBB` 当成 `#RRGGBBAA` 解析，所以这里统一转成 `#RRGGBB`（Alpha 忽略，积木颜色不透明）。
 */

/** 插件指定的三个颜色，后两个留空时按主色推导 */
export interface CategoryColorsInput {
    /** 主色：积木填充色，同时用作分类行的颜色 */
    primary?: string | null;
    /** 阴影色：同色相的浅色，留空时取「主色与白色混 60%」 */
    secondary?: string | null;
    /** 描边色：同色相的深色，留空时取「主色 × 0.8」 */
    tertiary?: string | null;
}

/** 归一化之后的三个颜色，都是 `#RRGGBB` */
export interface CategoryColors {
    primary: string;
    secondary: string;
    tertiary: string;
}

/** 不指定颜色时的主色（与历史版本一致） */
export const DEFAULT_CATEGORY_PRIMARY = '#00AAFF';

/** 阴影色：主色与白色混合的比例 */
const SECONDARY_WHITE_MIX = 0.6;

/** 描边色：主色明度缩放的比例 */
const TERTIARY_SCALE = 0.8;

type Rgb = [number, number, number];

/**
 * 解析 `#AARRGGBB` / `#RRGGBB` / `#ARGB` / `#RGB`（Alpha 忽略）
 *
 * @param input 颜色字符串
 * @returns RGB 三元组，解析失败时返回 null
 */
function parseHex(input?: string | null): Rgb | null {
    if (!input) return null;

    const hex = input.trim().replace(/^#/, '');
    if (!/^[0-9a-fA-F]+$/.test(hex)) return null;

    const expand = (value: string): Rgb => [
        parseInt(value[0] + value[0], 16),
        parseInt(value[1] + value[1], 16),
        parseInt(value[2] + value[2], 16),
    ];

    switch (hex.length) {
        // #AARRGGBB：头两位是 Alpha，丢掉
        case 8:
            return [
                parseInt(hex.slice(2, 4), 16),
                parseInt(hex.slice(4, 6), 16),
                parseInt(hex.slice(6, 8), 16),
            ];
        case 6:
            return [
                parseInt(hex.slice(0, 2), 16),
                parseInt(hex.slice(2, 4), 16),
                parseInt(hex.slice(4, 6), 16),
            ];
        // #ARGB：头一位是 Alpha，丢掉
        case 4:
            return expand(hex.slice(1));
        case 3:
            return expand(hex);
        default:
            return null;
    }
}

/** RGB 三元组转 `#RRGGBB` */
function toHex(rgb: Rgb): string {
    const channel = (value: number) =>
        Math.max(0, Math.min(255, Math.round(value)))
            .toString(16)
            .padStart(2, '0');
    return `#${channel(rgb[0])}${channel(rgb[1])}${channel(rgb[2])}`.toUpperCase();
}

/**
 * 归一化成 Blockly 认的 `#RRGGBB`
 *
 * @param input 颜色字符串（`#AARRGGBB` 或 `#RRGGBB`）
 * @param fallback 解析失败时的返回值
 */
export function toBlocklyColor(input?: string | null, fallback?: string): string | undefined {
    const rgb = parseHex(input);
    return rgb ? toHex(rgb) : fallback;
}

/** 主色与白色按比例混合（`t` = 0 保持原色，1 变纯白） */
function mixWithWhite(rgb: Rgb, t: number): Rgb {
    return [
        rgb[0] + (255 - rgb[0]) * t,
        rgb[1] + (255 - rgb[1]) * t,
        rgb[2] + (255 - rgb[2]) * t,
    ];
}

/** 各个通道按比例缩放（`k` < 1 变暗） */
function scaleRgb(rgb: Rgb, k: number): Rgb {
    return [rgb[0] * k, rgb[1] * k, rgb[2] * k];
}

/**
 * 把插件给的三个颜色补全并归一化
 *
 * @param input 插件指定的颜色，可以只给主色
 * @returns 三个 `#RRGGBB` 颜色
 */
export function resolveCategoryColors(input?: CategoryColorsInput | null): CategoryColors {
    const primary = parseHex(input?.primary) ?? parseHex(DEFAULT_CATEGORY_PRIMARY)!;
    const secondary = parseHex(input?.secondary) ?? mixWithWhite(primary, SECONDARY_WHITE_MIX);
    const tertiary = parseHex(input?.tertiary) ?? scaleRgb(primary, TERTIARY_SCALE);

    return {
        primary: toHex(primary),
        secondary: toHex(secondary),
        tertiary: toHex(tertiary),
    };
}

/** 收集到的积木样式，最后并进 Blockly 主题 */
export const categoryBlockStyles: Record<string, Blockly.Theme.BlockStyle> = {};

/** 自动生成的积木样式名序号 */
let autoStyleCounter = 0;

/**
 * 注册一个分类的配色，返回该分类下积木应该使用的样式名
 *
 * @param input 插件指定的颜色
 * @param styleName 样式名，不给就自动生成一个
 * @returns 积木样式名（同时也是分类行的颜色来源）
 */
export function registerCategoryColors(
    input?: CategoryColorsInput | null,
    styleName?: string,
): { styleName: string; colors: CategoryColors } {
    const name = styleName ?? `sai_blocks_${++autoStyleCounter}`;
    const colors = resolveCategoryColors(input);

    categoryBlockStyles[name] = {
        colourPrimary: colors.primary,
        colourSecondary: colors.secondary,
        colourTertiary: colors.tertiary,
        hat: '',
    };

    return { styleName: name, colors };
}
