/**
 * @fileoverview 颜色工具。
 *
 * SAI 的颜色统一使用「#AARRGGBB」格式（Alpha 在前），与 Avalonia 的
 * `Color.TryParse` / `Color.ToString()`、以及 ClassIsland 中
 * `SetSubjectColorBlock` 等积木的解析行为保持一致。
 */

/** RGBA 颜色，各分量取值范围均为 0-255。 */
export interface RgbaColor {
    /** 透明度 */
    a: number;
    /** 红色 */
    r: number;
    /** 绿色 */
    g: number;
    /** 蓝色 */
    b: number;
}

/** HSV 颜色。 */
export interface HsvColor {
    /** 色相，0-360 */
    h: number;
    /** 饱和度，0-100 */
    s: number;
    /** 明度，0-100 */
    v: number;
}

/** 默认颜色（不透明的红色），与 `BasicFields.Color` 的默认值保持一致。 */
export const DEFAULT_COLOR = '#FFFF0000';

const HEX_PATTERN = /^[0-9a-fA-F]+$/;

function clamp(value: number, min: number, max: number): number {
    return Math.min(Math.max(value, min), max);
}

function clampByte(value: number): number {
    if (!Number.isFinite(value)) return 0;
    return clamp(Math.round(value), 0, 255);
}

function byteToHex(value: number): string {
    return clampByte(value).toString(16).padStart(2, '0').toUpperCase();
}

/**
 * 解析颜色文本。
 *
 * 支持 `#RGB`、`#ARGB`、`#RRGGBB`、`#AARRGGBB`，以及省略 `#` 的写法，
 * 与 Avalonia 的 `Color.TryParse` 一致（Alpha 位于最前）。
 *
 * @param value 待解析的值，非字符串或格式不合法时返回 `null`。
 */
export function parseColor(value: unknown): RgbaColor | null {
    if (typeof value !== 'string') return null;
    let text = value.trim();
    if (text.startsWith('#')) text = text.slice(1);
    if (!text || !HEX_PATTERN.test(text)) return null;

    switch (text.length) {
        case 3:
            return {
                a: 255,
                r: parseInt(text[0] + text[0], 16),
                g: parseInt(text[1] + text[1], 16),
                b: parseInt(text[2] + text[2], 16),
            };
        case 4:
            return {
                a: parseInt(text[0] + text[0], 16),
                r: parseInt(text[1] + text[1], 16),
                g: parseInt(text[2] + text[2], 16),
                b: parseInt(text[3] + text[3], 16),
            };
        case 6:
            return {
                a: 255,
                r: parseInt(text.slice(0, 2), 16),
                g: parseInt(text.slice(2, 4), 16),
                b: parseInt(text.slice(4, 6), 16),
            };
        case 8:
            return {
                a: parseInt(text.slice(0, 2), 16),
                r: parseInt(text.slice(2, 4), 16),
                g: parseInt(text.slice(4, 6), 16),
                b: parseInt(text.slice(6, 8), 16),
            };
        default:
            return null;
    }
}

/**
 * 将颜色格式化为「#AARRGGBB」。
 */
export function formatColor(color: RgbaColor): string {
    return `#${byteToHex(color.a)}${byteToHex(color.r)}${byteToHex(color.g)}${byteToHex(color.b)}`;
}

/**
 * 将任意颜色文本规范化为「#AARRGGBB」，无法解析时返回 `null`。
 */
export function normalizeColor(value: unknown): string | null {
    const color = parseColor(value);
    return color ? formatColor(color) : null;
}

/**
 * 判断两个颜色文本是否表示同一种颜色（忽略大小写与书写形式）。
 */
export function colorsEqual(a: unknown, b: unknown): boolean {
    const left = parseColor(a);
    const right = parseColor(b);
    if (!left || !right) return left === right;
    return left.a === right.a && left.r === right.r && left.g === right.g && left.b === right.b;
}

/**
 * 转换为 CSS 颜色。
 *
 * 注意 CSS 会把「#AARRGGBB」解析成「#RRGGBBAA」，所以颜色一律通过本函数
 * 转成 `rgb()` / `rgba()` 后再交给浏览器。
 *
 * @param color 颜色
 * @param withAlpha 是否保留透明度，为 `false` 时输出不透明的 `rgb()`。
 */
export function toCssColor(color: RgbaColor, withAlpha = true): string {
    const r = clampByte(color.r);
    const g = clampByte(color.g);
    const b = clampByte(color.b);
    if (!withAlpha) return `rgb(${r}, ${g}, ${b})`;
    return `rgba(${r}, ${g}, ${b}, ${(clampByte(color.a) / 255).toFixed(3)})`;
}

/**
 * 判断颜色是否完全不透明。
 */
export function isOpaque(color: RgbaColor): boolean {
    return clampByte(color.a) === 255;
}

/**
 * RGB 转 HSV（色相 0-360，饱和度与明度 0-100）。
 */
export function rgbToHsv(color: RgbaColor): HsvColor {
    const r = clampByte(color.r) / 255;
    const g = clampByte(color.g) / 255;
    const b = clampByte(color.b) / 255;
    const max = Math.max(r, g, b);
    const min = Math.min(r, g, b);
    const delta = max - min;

    let h = 0;
    if (delta !== 0) {
        if (max === r) {
            h = ((g - b) / delta) % 6;
        } else if (max === g) {
            h = (b - r) / delta + 2;
        } else {
            h = (r - g) / delta + 4;
        }
        h *= 60;
        if (h < 0) h += 360;
    }

    return {
        h,
        s: max === 0 ? 0 : (delta / max) * 100,
        v: max * 100,
    };
}

/**
 * HSV 转 RGB。
 *
 * @param hsv 色相 0-360，饱和度与明度 0-100。
 * @param alpha 透明度 0-255。
 */
export function hsvToRgb(hsv: HsvColor, alpha = 255): RgbaColor {
    const h = ((hsv.h % 360) + 360) % 360;
    const s = clamp(hsv.s, 0, 100) / 100;
    const v = clamp(hsv.v, 0, 100) / 100;

    const c = v * s;
    const x = c * (1 - Math.abs(((h / 60) % 2) - 1));
    const m = v - c;

    let r = 0;
    let g = 0;
    let b = 0;
    if (h < 60) {
        [r, g, b] = [c, x, 0];
    } else if (h < 120) {
        [r, g, b] = [x, c, 0];
    } else if (h < 180) {
        [r, g, b] = [0, c, x];
    } else if (h < 240) {
        [r, g, b] = [0, x, c];
    } else if (h < 300) {
        [r, g, b] = [x, 0, c];
    } else {
        [r, g, b] = [c, 0, x];
    }

    return {
        a: clampByte(alpha),
        r: clampByte((r + m) * 255),
        g: clampByte((g + m) * 255),
        b: clampByte((b + m) * 255),
    };
}

/**
 * 以当前颜色为基准，替换 HSV 分量后返回新的颜色。
 */
export function withHsv(color: RgbaColor, patch: Partial<HsvColor>): RgbaColor {
    const hsv = rgbToHsv(color);
    return hsvToRgb(
        {
            h: patch.h ?? hsv.h,
            s: patch.s ?? hsv.s,
            v: patch.v ?? hsv.v,
        },
        color.a,
    );
}
