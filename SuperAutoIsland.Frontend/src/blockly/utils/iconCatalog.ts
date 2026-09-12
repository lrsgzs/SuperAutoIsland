export type IconCatalogType = 'fluent' | 'lucide';

export interface IconCatalogEntry {
    /** 与 ClassIsland 枚举名一致的显示名称，例如 `AccessTimeFilled`。 */
    name: string;
    /** 图标字形字符。 */
    glyph: string;
    codePoint: number;
    /** 去除空白、`-`、`_` 后的名称，用于搜索。 */
    searchName: string;
}

function toPascalCase(key: string, isFluent: boolean): string {
    let name = key;
    if (isFluent) {
        name = name.replace('ic_fluent_', '').replace('_20_', '_');
    }
    name = name.replace(/-/g, '_');
    return name
        .split('_')
        .filter(s => s.length > 0)
        .map(s => s.charAt(0).toUpperCase() + s.slice(1))
        .join('');
}

function normalizeName(name: string): string {
    return [...name].filter(c => !/\s/.test(c) && c !== '-' && c !== '_').join('');
}

function createCatalog(mapping: Record<string, number>, isFluent: boolean): IconCatalogEntry[] {
    return Object.entries(mapping)
        .map(([key, codePoint]) => {
            const name = toPascalCase(key, isFluent);
            return {
                name,
                glyph: String.fromCharCode(codePoint),
                codePoint,
                searchName: normalizeName(name),
            };
        })
        .sort((a, b) => {
            const x = a.name.toLowerCase();
            const y = b.name.toLowerCase();
            return x < y ? -1 : x > y ? 1 : 0;
        });
}

let fluentCatalog: IconCatalogEntry[] | null = null;
let lucideCatalog: IconCatalogEntry[] | null = null;
let fluentNameMap: Map<string, string> | null = null;
let lucideNameMap: Map<string, string> | null = null;
let fluentLoading: Promise<IconCatalogEntry[]> | null = null;
let lucideLoading: Promise<IconCatalogEntry[]> | null = null;

async function loadFluent(): Promise<IconCatalogEntry[]> {
    const mapping = (await import('../../assets/fonts/fluent.json')).default as Record<string, number>;
    fluentCatalog = createCatalog(mapping, true);
    fluentNameMap = new Map(fluentCatalog.map(entry => [entry.glyph, entry.name]));
    return fluentCatalog;
}

async function loadLucide(): Promise<IconCatalogEntry[]> {
    const mapping = (await import('../../assets/fonts/lucide.json')).default as Record<string, number>;
    lucideCatalog = createCatalog(mapping, false);
    lucideNameMap = new Map(lucideCatalog.map(entry => [entry.glyph, entry.name]));
    return lucideCatalog;
}

function loadCatalog(type: IconCatalogType): Promise<IconCatalogEntry[]> {
    if (type === 'fluent') {
        if (fluentCatalog) return Promise.resolve(fluentCatalog);
        return (fluentLoading ??= loadFluent());
    }
    if (lucideCatalog) return Promise.resolve(lucideCatalog);
    return (lucideLoading ??= loadLucide());
}

/**
 * 获取字形对应的图标名称。目录尚未加载时返回 null，可通过 {@link ensureIconCatalog} 预加载。
 */
export function getIconName(type: IconCatalogType, glyph: string): string | null {
    const map = type === 'fluent' ? fluentNameMap : lucideNameMap;
    return map?.get(glyph) ?? null;
}

/**
 * 预加载指定图标目录。
 */
export async function ensureIconCatalog(type: IconCatalogType): Promise<void> {
    await loadCatalog(type);
}

/**
 * 按 ClassIsland 的规则筛选图标：名称（忽略大小写与分隔符）、Unicode 码或精确字形。
 */
export async function searchIcons(type: IconCatalogType, query: string): Promise<IconCatalogEntry[]> {
    const catalog = await loadCatalog(type);
    const trimmed = query.trim();
    if (trimmed.length === 0) return catalog;

    const normalized = normalizeName(trimmed);
    let code = trimmed;
    if (
        code.startsWith('U+') ||
        code.startsWith('u+') ||
        code.startsWith('0x') ||
        code.startsWith('0X') ||
        code.startsWith('\\u') ||
        code.startsWith('\\U')
    ) {
        code = code.slice(2);
    }
    const parsed = parseInt(code, 16);
    const hasCode = /^[0-9a-fA-F]+$/.test(code) && Number.isFinite(parsed);

    return catalog.filter(
        entry =>
            entry.searchName.toLowerCase().includes(normalized.toLowerCase()) ||
            (hasCode && entry.codePoint === parsed) ||
            entry.glyph === trimmed,
    );
}
