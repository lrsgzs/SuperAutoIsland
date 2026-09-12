export type IconExpressionType = 'fluent' | 'lucide' | 'img';

export interface ParsedIconExpression {
    type: IconExpressionType;
    argument: string;
}

const ESCAPE_MAP: Record<string, string> = {
    n: '\n',
    r: '\r',
    t: '\t',
    '"': '"',
    "'": "'",
    '\\': '\\',
};

/**
 * 转义图标表达式的参数，与 ClassIsland IconExpressionEditorViewModel.FormatExpression 保持一致。
 */
export function escapeIconArgument(argument: string): string {
    return argument
        .replace(/\\/g, '\\\\')
        .replace(/"/g, '\\"')
        .replace(/\n/g, '\\n')
        .replace(/\r/g, '\\r')
        .replace(/\t/g, '\\t');
}

/**
 * 构造图标表达式，例如 `fluent("\ue9b0")`、`lucide("\ue224")`、`img("/tmp/a.png")`。
 */
export function formatIconExpression(type: IconExpressionType, argument: string): string {
    return `${type}("${escapeIconArgument(argument)}")`;
}

/**
 * 解析图标表达式。单字符会被兼容回退为 Fluent 图标。
 *
 * 参数支持带引号（`lucide("\ue0ff")`）与不带引号（`lucide(<glyph>)`，ClassIsland 默认值格式）。
 */
export function parseIconExpression(expression: string | null | undefined): ParsedIconExpression | null {
    if (!expression) return null;
    if (expression.length === 1) return { type: 'fluent', argument: expression };

    const match = expression.match(/^(fluent|lucide|img)\s*\(([\s\S]*)\)\s*$/);
    if (!match) return null;

    const raw = match[2].trim();
    if (raw.length === 0) return { type: match[1] as IconExpressionType, argument: '' };

    const quote = raw[0];
    if ((quote === '"' || quote === "'") && raw[raw.length - 1] === quote) {
        const inner = raw.slice(1, -1);
        return {
            type: match[1] as IconExpressionType,
            argument: inner.replace(/\\(.)/g, (_, char: string) => ESCAPE_MAP[char] ?? char),
        };
    }
    return { type: match[1] as IconExpressionType, argument: raw };
}

export const FLUENT_FONT_FAMILY = "'Fluent System Icons'";
export const LUCIDE_FONT_FAMILY = "'Lucide'";

/**
 * 返回表达式对应的字体族，非字体图标返回 null。
 */
export function iconFontFamily(expression: string | null | undefined): string | null {
    const parsed = parseIconExpression(expression);
    if (!parsed) return null;
    if (parsed.type === 'fluent') return FLUENT_FONT_FAMILY;
    if (parsed.type === 'lucide') return LUCIDE_FONT_FAMILY;
    return null;
}
