import {
    DEFAULT_COLOR,
    formatColor,
    hsvToRgb,
    parseColor,
    rgbToHsv,
    toCssColor,
    type HsvColor,
    type RgbaColor,
} from '../utils/colorUtils';import { FLUENT_PALETTE } from '../utils/colorPalette';

/**
 * @fileoverview 仿照 FluentAvalonia `ColorView` 的颜色选择面板。
 *
 * 布局与默认值都按 FluentAvalonia（Avalonia 的 ColorView）来：
 * - 三个分类页：光谱 / 调色板 / 组件；
 * - `ColorSpectrumComponents = HueSaturation`：二维色块横轴是色相、纵轴是饱和度，
 *   左侧竖条是第三个分量（明度），右侧竖条是透明度；
 * - `ColorModel = Rgba`：组件页默认是 RGB 模型；
 * - 底部 `ColorPreviewer` 的四个色阶可以点击，明度按 ±10% / ±20% 变化。
 */

/** 选择器模式：`preset` 只显示调色板分类，`full` 显示完整的三个分类。 */
export type ColorPickerMode = 'preset' | 'full';

/** 三个分类页，与 FluentAvalonia ColorView 的 TabControl 一一对应。 */
export type ColorSection = 'spectrum' | 'palette' | 'components';

/** 组件页的颜色模型，与 ColorView 的 RGB / HSV 切换一致。 */
export type ColorModel = 'rgba' | 'hsva';

/** 三个分类页的顺序与图标，字形取自 Fluent System Icons（与 WinUI/FA 的三个页面一一对应）。 */
const SECTIONS: { id: ColorSection; glyph: string; label: string }[] = [
    // ic_fluent_highlight_20_regular：笔，对应 WinUI 光谱页的「Highlight」(E76D)
    { id: 'spectrum', glyph: '\uE989', label: '光谱' },
    // ic_fluent_color_20_regular：调色盘
    { id: 'palette', glyph: '\uE51E', label: '调色板' },
    // ic_fluent_options_20_regular：三条滑块，对应组件页
    { id: 'components', glyph: '\uEC34', label: '组件' },
];

/** 组件页里的一个通道。 */
type ChannelId = 'r' | 'g' | 'b' | 'h' | 's' | 'v' | 'a';

const CHANNEL_NAMES: Record<ChannelId, string> = {
    r: 'R',
    g: 'G',
    b: 'B',
    h: 'H',
    s: 'S',
    v: 'V',
    a: 'A',
};

const CHANNEL_TITLES: Partial<Record<ChannelId, string>> = {
    h: '色相',
    s: '饱和度',
    v: '明度',
    a: '透明度',
};

/** 预览条上四个色阶的相对明度偏移，与 Avalonia 的 AccentColorConverter 一致。 */
const PREVIEW_SHADES: { delta: number; label: string }[] = [
    { delta: -2, label: '更暗' },
    { delta: -1, label: '较暗' },
    { delta: 1, label: '较亮' },
    { delta: 2, label: '更亮' },
];

export interface ColorPickerPanelOptions {
    /** 选择器模式，默认为 `full`。 */
    mode?: ColorPickerMode;
    /** 初始颜色。 */
    color?: string | null;
    /** 颜色变化时回调，返回「#AARRGGBB」格式的颜色。 */
    onChange?: (color: string) => void;
    /** 按下 Esc 等要求关闭面板时回调。 */
    onRequestClose?: () => void;
}

export interface ColorPickerPanelHandle {
    /** 面板根元素。 */
    root: HTMLElement;
    /** 当前颜色的「#AARRGGBB」表示。 */
    getColor(): string;
    /** 卸载面板，解除全部事件监听。 */
    dispose(): void;
}

function clamp(value: number, min: number, max: number): number {
    return Math.min(Math.max(value, min), max);
}

function element<K extends keyof HTMLElementTagNameMap>(
    tag: K,
    className: string,
    parent?: HTMLElement,
): HTMLElementTagNameMap[K] {
    const node = document.createElement(tag);
    node.className = className;
    parent?.appendChild(node);
    return node;
}

/** 通道的取值范围。 */
function channelRange(id: ChannelId): { min: number; max: number } {
    if (id === 'h') return { min: 0, max: 360 };
    if (id === 's' || id === 'v') return { min: 0, max: 100 };
    return { min: 0, max: 255 };
}

/** 当前颜色模型下的四个通道，与 FluentAvalonia 的组件页一致（RGB:RGBA / HSV:HSVA）。 */
function modelChannels(model: ColorModel): ChannelId[] {
    return model === 'rgba' ? ['r', 'g', 'b', 'a'] : ['h', 's', 'v', 'a'];
}

/**
 * 创建颜色选择面板。
 */
export function createColorPickerPanel(options: ColorPickerPanelOptions = {}): ColorPickerPanelHandle {
    const mode = options.mode ?? 'full';
    const initial = parseColor(options.color) ?? parseColor(DEFAULT_COLOR)!;

    /** 以 HSV 为权威状态，这样在灰色/黑色时色相与饱和度不会丢失。 */
    let hsv: HsvColor = rgbToHsv(initial);
    let alpha = initial.a;
    let model: ColorModel = 'rgba';
    let section: ColorSection = mode === 'preset' ? 'palette' : 'spectrum';

    const cleanups: (() => void)[] = [];
    const on = (target: EventTarget, type: string, handler: (event: never) => void, opts?: AddEventListenerOptions) => {
        target.addEventListener(type, handler as EventListener, opts);
        cleanups.push(() => target.removeEventListener(type, handler as EventListener, opts));
    };

    const currentColor = (): RgbaColor => hsvToRgb(hsv, alpha);
    const currentHex = (): string => formatColor(currentColor());

    /** 由 RGB 反推 HSV，同时保留灰色/黑色下无意义的色相与饱和度。 */
    const applyRgb = (color: RgbaColor): void => {
        const next = rgbToHsv(color);
        hsv = {
            h: next.s === 0 || next.v === 0 ? hsv.h : next.h,
            s: next.v === 0 ? hsv.s : next.s,
            v: next.v,
        };
        alpha = color.a;
    };

    /** 修改单个通道。 */
    const applyChannel = (id: ChannelId, raw: number): void => {
        const { min, max } = channelRange(id);
        const value = clamp(Math.round(raw), min, max);
        if (id === 'a') {
            alpha = value;
        } else if (id === 'h') {
            hsv = { ...hsv, h: value };
        } else if (id === 's') {
            hsv = { ...hsv, s: value };
        } else if (id === 'v') {
            hsv = { ...hsv, v: value };
        } else {
            const color = currentColor();
            applyRgb({ ...color, [id]: value } as RgbaColor);
        }
    };

    // ---------------------------------------------------------------- 提交
    /**
     * 颜色变化统一通过 rAF 合并后上报：拖动滑块时一帧只触发一次字段更新，
     * 避免每移动一格都让 Blockly 重新渲染积木。
     */
    let lastCommitted: string | null = null;
    let commitPending = false;
    let commitTimer: number | undefined;

    const flushCommit = () => {
        if (commitTimer !== undefined) {
            clearTimeout(commitTimer);
            commitTimer = undefined;
        }
        if (!commitPending) return;
        commitPending = false;
        const hex = currentHex();
        if (hex === lastCommitted) return;
        lastCommitted = hex;
        options.onChange?.(hex);
    };

    const commit = () => {
        if (commitPending) return;
        commitPending = true;
        // 用动画帧合并：拖动滑块时一帧只上报一次，避免每移动一格都让 Blockly 重渲染积木。
        if (typeof requestAnimationFrame === 'function') requestAnimationFrame(flushCommit);
        // 兜底：页面不可见时浏览器不会派发动画帧，这里保证改动仍然会提交。
        commitTimer = window.setTimeout(flushCommit, 120);
    };

    // ---------------------------------------------------------------- 根节点
    const root = element('div', 'sai-cp');
    const body = element('div', 'sai-cp-body', root);

    // ------------------------------------------------------------ 光谱分类
    // ColorSpectrumComponents = HueSaturation：横轴色相、纵轴饱和度，
    // 左侧竖条是第三个分量（明度），右侧竖条是透明度。
    const spectrumPanel = element('div', 'sai-cp-panel sai-cp-spectrum', body);
    const valueSlider = element('div', 'sai-cp-vslider', spectrumPanel);
    valueSlider.dataset.channel = 'v';
    valueSlider.title = '明度';
    valueSlider.setAttribute('aria-label', '明度');
    element('div', 'sai-cp-vtrack', valueSlider);
    const valueThumb = element('div', 'sai-cp-vthumb', valueSlider);

    const spectrumBox = element('div', 'sai-cp-spectrum-box', spectrumPanel);
    const spectrumThumb = element('div', 'sai-cp-spectrum-thumb', spectrumBox);

    const alphaSlider = element('div', 'sai-cp-vslider', spectrumPanel);
    alphaSlider.dataset.channel = 'a';
    alphaSlider.title = '透明度';
    alphaSlider.setAttribute('aria-label', '透明度');
    element('div', 'sai-cp-vtrack sai-cp-checker', alphaSlider);
    const alphaThumb = element('div', 'sai-cp-vthumb', alphaSlider);

    /** 拖动一个元素，回调收到 0-1 的相对坐标。 */
    const bindDrag = (target: HTMLElement, onDrag: (x: number, y: number) => void) => {
        const move = (event: PointerEvent) => {
            const rect = target.getBoundingClientRect();
            if (!rect.width || !rect.height) return;
            onDrag(
                clamp((event.clientX - rect.left) / rect.width, 0, 1),
                clamp((event.clientY - rect.top) / rect.height, 0, 1),
            );
        };
        on(target, 'pointerdown', (event: PointerEvent) => {
            event.preventDefault();
            move(event);
            const up = () => {
                document.removeEventListener('pointermove', move);
                document.removeEventListener('pointerup', up);
                document.removeEventListener('pointercancel', up);
            };
            document.addEventListener('pointermove', move);
            document.addEventListener('pointerup', up);
            document.addEventListener('pointercancel', up);
            // 面板可能在拖动过程中被关闭，这里保证 document 上的监听一并解除。
            cleanups.push(up);
        });
    };

    bindDrag(spectrumBox, (x, y) => {
        hsv = { ...hsv, h: x * 360, s: (1 - y) * 100 };
        sync();
        commit();
    });
    // 竖条与 FluentAvalonia 一致：底部为最小值，顶部为最大值。
    bindDrag(valueSlider, (_x, y) => {
        hsv = { ...hsv, v: (1 - y) * 100 };
        sync();
        commit();
    });
    bindDrag(alphaSlider, (_x, y) => {
        alpha = (1 - y) * 255;
        sync();
        commit();
    });

    // ---------------------------------------------------------- 调色板分类
    const palettePanel = element('div', 'sai-cp-panel sai-cp-palette', body);
    const paletteGrid = element('div', 'sai-cp-palette-grid', palettePanel);
    const swatchButtons = new Map<HTMLButtonElement, string>();
    for (const preset of FLUENT_PALETTE) {
        const button = element('button', 'sai-cp-swatch', paletteGrid);
        button.type = 'button';
        button.title = preset;
        button.setAttribute('aria-label', preset);
        const parsed = parseColor(preset)!;
        // 注意：CSS 会把「#AARRGGBB」当成「#RRGGBBAA」，这里必须转成 rgb()。
        button.style.background = toCssColor(parsed);
        // 选中/悬停的描边取对比色，与 FluentAvalonia 的 ContrastBrushConverter 一致。
        const light = (0.299 * parsed.r + 0.587 * parsed.g + 0.114 * parsed.b) / 255 > 0.55;
        button.style.setProperty('--sai-cp-swatch-ring', light ? 'rgba(0, 0, 0, 0.9)' : 'rgba(255, 255, 255, 0.95)');
        button.style.setProperty('--sai-cp-swatch-ring-soft', light ? 'rgba(0, 0, 0, 0.45)' : 'rgba(255, 255, 255, 0.55)');
        on(button, 'click', () => {
            applyRgb(parsed);
            sync();
            commit();
        });
        swatchButtons.set(button, preset);
    }

    // ------------------------------------------------------------ 组件分类
    const componentsPanel = element('div', 'sai-cp-panel sai-cp-components', body);

    const componentsHead = element('div', 'sai-cp-components-head', componentsPanel);
    const segmented = element('div', 'sai-cp-segmented', componentsHead);
    const modelButtons = new Map<HTMLButtonElement, ColorModel>();
    (['rgba', 'hsva'] as ColorModel[]).forEach(kind => {
        const button = element('button', 'sai-cp-segment', segmented);
        button.type = 'button';
        button.textContent = kind === 'rgba' ? 'RGB' : 'HSV';
        on(button, 'click', () => {
            if (model === kind) return;
            model = kind;
            sync();
        });
        modelButtons.set(button, kind);
    });

    const hexBox = element('div', 'sai-cp-hex', componentsHead);
    const hexPrefix = element('span', 'sai-cp-hex-prefix', hexBox);
    hexPrefix.textContent = '#';
    const hexInput = element('input', 'sai-cp-hex-input', hexBox);
    hexInput.type = 'text';
    hexInput.maxLength = 8;
    hexInput.autocomplete = 'off';
    hexInput.spellcheck = false;
    hexInput.placeholder = 'AARRGGBB';
    hexInput.setAttribute('aria-label', '颜色代码');

    /** 输入框内容合法时立即应用，非法时保持沉默（失焦时才回退）。 */
    const applyHexInput = (): boolean => {
        const parsed = parseColor(hexInput.value);
        if (!parsed) return false;
        applyRgb(parsed);
        sync();
        commit();
        return true;
    };
    on(hexInput, 'input', () => {
        const text = hexInput.value.replace(/[^0-9a-fA-F]/g, '').slice(0, 8);
        if (text !== hexInput.value) hexInput.value = text;
        applyHexInput();
    });
    on(hexInput, 'blur', () => {
        if (!applyHexInput()) sync();
    });

    /** 组件页的四行：标签 + 数值输入 + 滑块。 */
    const rows: { label: HTMLSpanElement; number: HTMLInputElement; slider: HTMLInputElement }[] = [];
    for (let index = 0; index < 4; index++) {
        const row = element('div', 'sai-cp-component-row', componentsPanel);
        const label = element('span', 'sai-cp-component-label', row);
        const number = element('input', 'sai-cp-component-number', row);
        number.type = 'text';
        number.autocomplete = 'off';
        number.spellcheck = false;
        const slider = element('input', 'sai-cp-component-slider', row);
        slider.type = 'range';
        slider.step = '1';

        const applyValue = (raw: number) => {
            applyChannel(modelChannels(model)[index], raw);
            sync();
            commit();
        };

        on(slider, 'input', () => applyValue(Number(slider.value)));
        on(number, 'change', () => applyValue(Number(number.value)));
        on(number, 'keydown', (event: KeyboardEvent) => {
            if (event.key !== 'Enter') return;
            event.preventDefault();
            applyValue(Number(number.value));
            number.blur();
        });

        rows.push({ label, number, slider });
    }

    // --------------------------------------------------------------- 预览条
    // ColorPreviewer：中间是当前颜色，两侧各两级明度色阶，四个色阶都可以点击。
    const shadeColor = (delta: number): RgbaColor =>
        hsvToRgb({ ...hsv, v: clamp(hsv.v + delta * 10, 0, 100) }, alpha);

    const previewer = element('div', 'sai-cp-previewer', root);
    const shadeSlots: { slot: HTMLElement; delta: number; label: string }[] = [];
    const createShade = (definition: { delta: number; label: string }) => {
        const slot = element('div', 'sai-cp-accent', previewer);
        slot.setAttribute('role', 'button');
        slot.tabIndex = 0;
        const pick = () => {
            applyRgb(shadeColor(definition.delta));
            sync();
            commit();
        };
        on(slot, 'click', pick);
        on(slot, 'keydown', (event: KeyboardEvent) => {
            if (event.key !== 'Enter' && event.key !== ' ') return;
            event.preventDefault();
            pick();
        });
        shadeSlots.push({ slot, delta: definition.delta, label: definition.label });
    };
    // 排列为「-2 -1 当前色 +1 +2」，与 FluentAvalonia 的 ColorPreviewer 一致。
    createShade(PREVIEW_SHADES[0]);
    createShade(PREVIEW_SHADES[1]);
    const previewMain = element('div', 'sai-cp-preview-main sai-cp-checker', previewer);
    const previewMainColour = element('div', 'sai-cp-preview-colour', previewMain);
    createShade(PREVIEW_SHADES[2]);
    createShade(PREVIEW_SHADES[3]);

    // -------------------------------------------------------------- 同步
    /** 组件滑块与竖向滑条的轨道渐变。 */
    const channelGradient = (id: ChannelId): string => {
        const color = currentColor();
        switch (id) {
            case 'r':
                return `linear-gradient(to right, ${toCssColor({ ...color, r: 0 }, false)}, ${toCssColor(
                    { ...color, r: 255 },
                    false,
                )})`;
            case 'g':
                return `linear-gradient(to right, ${toCssColor({ ...color, g: 0 }, false)}, ${toCssColor(
                    { ...color, g: 255 },
                    false,
                )})`;
            case 'b':
                return `linear-gradient(to right, ${toCssColor({ ...color, b: 0 }, false)}, ${toCssColor(
                    { ...color, b: 255 },
                    false,
                )})`;
            case 'h':
                return `linear-gradient(to right, ${[0, 60, 120, 180, 240, 300, 360]
                    .map(h => toCssColor(hsvToRgb({ h, s: 100, v: 100 }), false))
                    .join(', ')})`;
            case 's':
                return `linear-gradient(to right, ${toCssColor(
                    hsvToRgb({ h: hsv.h, s: 0, v: hsv.v }),
                    false,
                )}, ${toCssColor(hsvToRgb({ h: hsv.h, s: 100, v: hsv.v }), false)})`;
            case 'v':
                return `linear-gradient(to right, ${toCssColor(
                    hsvToRgb({ h: hsv.h, s: hsv.s, v: 0 }),
                    false,
                )}, ${toCssColor(hsvToRgb({ h: hsv.h, s: hsv.s, v: 100 }), false)})`;
            default:
                return `linear-gradient(to right, ${toCssColor(hsvToRgb(hsv, 0))}, ${toCssColor(hsvToRgb(hsv, 255))})`;
        }
    };

    const sync = () => {
        const color = currentColor();
        const hex = currentHex();

        if (document.activeElement !== hexInput) hexInput.value = hex.slice(1);

        // 光谱页：横轴色相、纵轴饱和度，整块色域按当前明度绘制
        const hueStops = [0, 60, 120, 180, 240, 300, 360]
            .map(h => toCssColor(hsvToRgb({ h, s: 100, v: hsv.v }), false))
            .join(', ');
        const gray = hsvToRgb({ h: hsv.h, s: 0, v: hsv.v });
        spectrumBox.style.backgroundImage = `linear-gradient(to bottom, ${toCssColor(
            { ...gray, a: 0 },
        )}, ${toCssColor(gray, false)}), linear-gradient(to right, ${hueStops})`;
        spectrumThumb.style.left = `${clamp(hsv.h / 3.6, 0, 100)}%`;
        spectrumThumb.style.top = `${clamp(100 - hsv.s, 0, 100)}%`;
        spectrumThumb.style.background = toCssColor(color, false);
        valueThumb.style.top = `${clamp(100 - hsv.v, 0, 100)}%`;
        valueSlider.style.setProperty(
            '--sai-cp-value-gradient',
            `linear-gradient(to top, #000, ${toCssColor(hsvToRgb({ h: hsv.h, s: hsv.s, v: 100 }), false)})`,
        );
        alphaThumb.style.top = `${clamp((1 - alpha / 255) * 100, 0, 100)}%`;
        alphaSlider.style.setProperty(
            '--sai-cp-alpha-gradient',
            `linear-gradient(to top, ${toCssColor(hsvToRgb(hsv, 0))}, ${toCssColor(hsvToRgb(hsv, 255))})`,
        );

        // 调色板页：预设色都是规范化的「#AARRGGBB」，直接比较字符串即可
        if (section === 'palette') {
            for (const [button, preset] of swatchButtons) {
                button.classList.toggle('active', preset === hex);
            }
        }

        // 组件页
        if (section === 'components') {
            for (const [button, kind] of modelButtons) {
                button.classList.toggle('active', kind === model);
            }
            const channels = modelChannels(model);
            rows.forEach((row, index) => {
                const id = channels[index];
                const { min, max } = channelRange(id);
                const value =
                    id === 'a'
                        ? alpha
                        : id === 'h'
                          ? hsv.h
                          : id === 's'
                            ? hsv.s
                            : id === 'v'
                              ? hsv.v
                              : id === 'r'
                                ? color.r
                                : id === 'g'
                                  ? color.g
                                  : color.b;
                const name = CHANNEL_NAMES[id];
                const title = CHANNEL_TITLES[id] ?? name;
                row.label.textContent = name;
                row.label.title = title;
                if (document.activeElement !== row.number) row.number.value = String(Math.round(value));
                row.number.setAttribute('aria-label', title);
                row.slider.min = String(min);
                row.slider.max = String(max);
                row.slider.setAttribute('aria-label', title);
                if (document.activeElement !== row.slider) row.slider.value = String(Math.round(value));
                row.slider.style.setProperty('--sai-range-track', channelGradient(id));
            });
        }

        // 预览条
        previewMainColour.style.background = toCssColor(color);
        for (const { slot, delta, label } of shadeSlots) {
            slot.style.background = toCssColor(shadeColor(delta));
            slot.title = `${label}（明度 ${Math.round(clamp(hsv.v + delta * 10, 0, 100))}%）`;
            slot.setAttribute('aria-label', slot.title);
        }
    };

    // ------------------------------------------------------------ 分类切换
    const tabButtons = new Map<HTMLButtonElement, ColorSection>();
    const applySection = () => {
        for (const [button, kind] of tabButtons) {
            button.classList.toggle('active', kind === section);
        }
        spectrumPanel.style.display = section === 'spectrum' ? '' : 'none';
        palettePanel.style.display = section === 'palette' ? '' : 'none';
        componentsPanel.style.display = section === 'components' ? '' : 'none';
        // 组件页与调色板页只在显示时才更新，切回来时补一次同步
        if (section === 'components' || section === 'palette') sync();
    };

    if (mode === 'full') {
        const tabStrip = element('div', 'sai-cp-tabs', root);
        root.insertBefore(tabStrip, body);
        for (const definition of SECTIONS) {
            const button = element('button', 'sai-cp-tab', tabStrip);
            button.type = 'button';
            button.title = definition.label;
            button.setAttribute('aria-label', definition.label);
            const icon = element('span', 'sai-cp-tab-icon', button);
            icon.textContent = definition.glyph;
            element('span', 'sai-cp-tab-pipe', button);
            on(button, 'click', () => {
                if (section === definition.id) return;
                section = definition.id;
                applySection();
            });
            tabButtons.set(button, definition.id);
        }
    }

    on(document, 'keydown', (event: KeyboardEvent) => {
        if (event.key !== 'Escape') return;
        event.preventDefault();
        options.onRequestClose?.();
    });

    applySection();
    sync();
    lastCommitted = currentHex();

    return {
        root,
        getColor: currentHex,
        dispose: () => {
            // 关闭前把还没上报的改动补上，避免最后一帧的改动丢失
            flushCommit();
            for (const cleanup of cleanups.splice(0)) cleanup();
            root.remove();
        },
    };
}
