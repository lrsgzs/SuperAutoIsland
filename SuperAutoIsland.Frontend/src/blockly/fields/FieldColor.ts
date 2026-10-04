import * as Blockly from 'blockly/core';
import { DEFAULT_COLOR, formatColor, parseColor, toCssColor, type RgbaColor } from '../utils/colorUtils';
import { DEFAULT_PRESET_COLOR } from '../utils/colorPalette';
import { openColorPicker } from './ColorPickerDropdown';
import type { ColorPickerMode } from './ColorPickerPanel';

const SVG_NS = 'http://www.w3.org/2000/svg';

/** 字段内边距，与积木坐标一致。 */
const PADDING_X = 4;
const PADDING_Y = 2;
/** 色块尺寸。 */
const SWATCH_WIDTH = 16;
const SWATCH_HEIGHT = 11;
/** 色块与颜色代码之间的距离。 */
const SWATCH_GAP = 4;
/** 颜色代码的字号，比积木正文略小，避免颜色积木过宽。 */
const TEXT_FONT_SIZE = 10;
/** 字段最小高度。 */
const MIN_HEIGHT = 20;

/** 棋盘格的编号，用来生成不重复的 `<pattern>` id。 */
let checkerPatternCount = 0;

/** 颜色字段配置。 */
export interface FieldColorConfig extends Blockly.FieldConfig {
    /** 初始颜色，格式为「#AARRGGBB」。 */
    color?: string;
}

/** 由 JSON 创建颜色字段时的配置，同时兼容旧字段使用的 `colour` 键。 */
export interface FieldColorFromJsonConfig extends FieldColorConfig {
    colour?: string;
}

/**
 * 颜色字段。
 *
 * 字段本身显示一个色块（半透明颜色带棋盘格底）与「#AARRGGBB」颜色代码，
 * 点击后打开与 ClassIsland / FluentAvalonia 一致的颜色选择器。
 */
export class FieldColor extends Blockly.Field<string> {
    /** 色块（棋盘格底）。 */
    private checkerRect_: SVGRectElement | null = null;
    /** 色块（颜色本身）。 */
    private colorRect_: SVGRectElement | null = null;
    /** 色块描边。 */
    private outlineRect_: SVGRectElement | null = null;

    constructor(value?: string, validator?: Blockly.FieldValidator<string>, config?: FieldColorConfig) {
        super(value ?? DEFAULT_COLOR, validator, config);
        this.SERIALIZABLE = true;
        this.EDITABLE = true;
    }

    /** 选择器模式，子类可覆盖。 */
    protected get pickerMode(): ColorPickerMode {
        return 'full';
    }

    static fromJson(options: FieldColorFromJsonConfig): FieldColor {
        const value = options.color ?? options.colour;
        const color = value != null ? Blockly.utils.parsing.replaceMessageReferences(value) : undefined;
        return new this(color, undefined, options);
    }

    /**
     * 校验并规范化颜色值，「#AARRGGBB」以外的写法都会被转换过来。
     */
    protected override doClassValidation_(newValue?: unknown): string | null {
        if (typeof newValue !== 'string') return null;
        const color = parseColor(newValue);
        return color ? formatColor(color) : null;
    }

    protected override initView(): void {
        super.initView();

        const group = this.fieldGroup_;
        const textElement = this.textElement_;
        if (!group || !textElement || !this.borderRect_) return;

        // 字段整体背景，与图标字段保持一致。
        this.borderRect_.style.fill = '#ffffff';
        this.borderRect_.style.fillOpacity = '0.6';
        this.borderRect_.style.stroke = 'none';

        // 半透明颜色的棋盘格底。
        const patternId = `saiColorChecker${++checkerPatternCount}`;
        const defs = document.createElementNS(SVG_NS, 'defs');
        const pattern = document.createElementNS(SVG_NS, 'pattern');
        pattern.setAttribute('id', patternId);
        pattern.setAttribute('width', '8');
        pattern.setAttribute('height', '8');
        pattern.setAttribute('patternUnits', 'userSpaceOnUse');
        const patternBase = document.createElementNS(SVG_NS, 'rect');
        patternBase.setAttribute('width', '8');
        patternBase.setAttribute('height', '8');
        patternBase.setAttribute('fill', '#ffffff');
        pattern.appendChild(patternBase);
        for (const [x, y] of [
            [0, 0],
            [4, 4],
        ]) {
            const cell = document.createElementNS(SVG_NS, 'rect');
            cell.setAttribute('x', String(x));
            cell.setAttribute('y', String(y));
            cell.setAttribute('width', '4');
            cell.setAttribute('height', '4');
            cell.setAttribute('fill', '#c8c8c8');
            pattern.appendChild(cell);
        }
        defs.appendChild(pattern);
        group.insertBefore(defs, group.firstChild);

        this.checkerRect_ = document.createElementNS(SVG_NS, 'rect') as SVGRectElement;
        this.checkerRect_.style.fill = `url(#${patternId})`;
        this.checkerRect_.style.stroke = 'none';

        this.colorRect_ = document.createElementNS(SVG_NS, 'rect') as SVGRectElement;
        this.colorRect_.style.stroke = 'none';

        this.outlineRect_ = document.createElementNS(SVG_NS, 'rect') as SVGRectElement;
        this.outlineRect_.style.fill = 'none';
        this.outlineRect_.style.stroke = 'rgba(0, 0, 0, 0.24)';
        this.outlineRect_.style.strokeWidth = '1';

        group.insertBefore(this.checkerRect_, textElement);
        group.insertBefore(this.colorRect_, textElement);
        group.insertBefore(this.outlineRect_, textElement);
    }

    protected override render_(): void {
        if (!this.textContent_ || !this.textElement_) return;

        const color = this.parseValue_();
        this.textElement_.style.fontSize = `${TEXT_FONT_SIZE}px`;
        this.textContent_.nodeValue = formatColor(color);
        if (this.colorRect_) {
            this.colorRect_.style.fill = toCssColor(color, false);
            this.colorRect_.style.fillOpacity = String(color.a / 255);
        }
        this.updateSize_();
    }

    protected override updateSize_(): void {
        const textElement = this.textElement_;
        if (!textElement) return;

        const bbox = this.measureText_(textElement);
        const height = Math.max(bbox.height + PADDING_Y * 2, SWATCH_HEIGHT + PADDING_Y * 2, MIN_HEIGHT);
        const width = PADDING_X * 2 + SWATCH_WIDTH + SWATCH_GAP + bbox.width;
        this.size_ = new Blockly.utils.Size(width, height);

        const rtl = Boolean(this.sourceBlock_?.RTL);
        const swatchX = rtl ? width - PADDING_X - SWATCH_WIDTH : PADDING_X;
        const swatchY = (height - SWATCH_HEIGHT) / 2;
        const textX = rtl ? PADDING_X : PADDING_X + SWATCH_WIDTH + SWATCH_GAP;

        const currentX = Number(textElement.getAttribute('x') || 0);
        const currentY = Number(textElement.getAttribute('y') || 0);
        textElement.setAttribute('x', String(textX - (bbox.x - currentX)));
        textElement.setAttribute('y', String((height - bbox.height) / 2 - (bbox.y - currentY)));

        this.layoutRect_(this.borderRect_, 0, 0, width, height);
        this.layoutRect_(this.checkerRect_, swatchX, swatchY, SWATCH_WIDTH, SWATCH_HEIGHT);
        this.layoutRect_(this.colorRect_, swatchX, swatchY, SWATCH_WIDTH, SWATCH_HEIGHT);
        this.layoutRect_(this.outlineRect_, swatchX - 0.5, swatchY - 0.5, SWATCH_WIDTH + 1, SWATCH_HEIGHT + 1);
    }

    protected override showEditor_(): void {
        openColorPicker(this, {
            mode: this.pickerMode,
            color: this.value_,
            onChange: color => this.setValue(color),
        });
    }

    /**
     * 取得当前颜色，无法解析时回落到默认颜色。
     */
    private parseValue_(): RgbaColor {
        return parseColor(this.value_) ?? parseColor(DEFAULT_COLOR)!;
    }

    /**
     * 测量文本，尚未排版时按字符数估算一个尺寸，下一次渲染会自行修正。
     */
    private measureText_(element: SVGTextElement): { x: number; y: number; width: number; height: number } {
        try {
            const bbox = element.getBBox();
            if (bbox.width && bbox.height) {
                return { x: bbox.x, y: bbox.y, width: bbox.width, height: bbox.height };
            }
        } catch {
            // 元素尚未插入文档时部分浏览器会抛出异常，忽略并使用估算值。
        }
        return { x: 0, y: 0, width: formatColor(this.parseValue_()).length * 6.6, height: 12 };
    }

    private layoutRect_(rect: SVGRectElement | null, x: number, y: number, width: number, height: number): void {
        if (!rect) return;
        const radius = Math.min(3, height / 2);
        rect.setAttribute('x', String(x));
        rect.setAttribute('y', String(y));
        rect.setAttribute('width', String(width));
        rect.setAttribute('height', String(height));
        rect.setAttribute('rx', String(radius));
        rect.setAttribute('ry', String(radius));
    }
}

/**
 * 预设颜色字段：只提供 FluentAvalonia 的预设颜色分类，不支持自定义取色。
 */
export class FieldColorPreset extends FieldColor {
    constructor(value?: string, validator?: Blockly.FieldValidator<string>, config?: FieldColorConfig) {
        super(value ?? DEFAULT_PRESET_COLOR, validator, config);
    }

    protected override get pickerMode(): ColorPickerMode {
        return 'preset';
    }
}

FieldColor.prototype.DEFAULT_VALUE = DEFAULT_COLOR;
FieldColorPreset.prototype.DEFAULT_VALUE = DEFAULT_PRESET_COLOR;

Blockly.fieldRegistry.register('field_color', FieldColor);
Blockly.fieldRegistry.register('field_color_preset', FieldColorPreset);
