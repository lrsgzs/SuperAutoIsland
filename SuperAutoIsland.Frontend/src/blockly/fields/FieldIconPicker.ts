import * as Blockly from 'blockly/core';
import {
    FLUENT_FONT_FAMILY,
    LUCIDE_FONT_FAMILY,
    formatIconExpression,
    iconFontFamily,
    parseIconExpression,
    type IconExpressionType,
} from '../utils/iconExpression';
import { ensureIconCatalog, getIconName } from '../utils/iconCatalog';
import { openIconPicker } from './IconPickerDialog';

const SVG_NS = 'http://www.w3.org/2000/svg';
const FALLBACK_FLUENT_GLYPH = '\ue9b0';
const FALLBACK_LUCIDE_GLYPH = '\ue0ff';
const LABEL_STYLE = "font-family: system-ui, -apple-system, 'Segoe UI', sans-serif; font-size: 11px;";

function defaultIconExpression(type: IconExpressionType): string {
    if (type === 'lucide') return formatIconExpression('lucide', FALLBACK_LUCIDE_GLYPH);
    if (type === 'img') return '';
    return formatIconExpression('fluent', FALLBACK_FLUENT_GLYPH);
}

function iconLabel(type: IconExpressionType, glyph: string): string {
    const prefix = type === 'lucide' ? 'Lucide' : 'Fluent';
    const typeName = type === 'lucide' ? 'lucide' : 'fluent';
    const name = getIconName(typeName, glyph);
    if (name) return `${prefix}/${name}`;
    const codePoint = glyph.codePointAt(0);
    return codePoint != null ? `${prefix}/U+${codePoint.toString(16).toUpperCase().padStart(4, '0')}` : prefix;
}

/**
 * 可选择 Fluent / Lucide / 图片的图标表达式字段，交互与 ClassIsland 的图标编辑器一致。
 */
export class FieldIconPicker extends Blockly.Field<string> {
    private iconType_: IconExpressionType;
    private labelSpan_: SVGTSpanElement | null = null;

    constructor(value?: string, validator?: Blockly.FieldValidator<string>, config?: FieldIconPickerConfig) {
        super(value ?? defaultIconExpression(config?.iconType ?? 'fluent'), validator, config);
        this.iconType_ = config?.iconType ?? 'fluent';
        this.SERIALIZABLE = true;
        this.EDITABLE = true;
    }

    static fromJson(options: FieldIconPickerFromJsonConfig): FieldIconPicker {
        const value = options.value != null ? Blockly.utils.parsing.replaceMessageReferences(options.value) : undefined;
        return new FieldIconPicker(value, undefined, { iconType: options.iconType });
    }

    protected doClassValidation_(newValue?: unknown): string | null {
        if (typeof newValue !== 'string') return null;
        return newValue;
    }

    getText(): string {
        const parsed = parseIconExpression(this.value_);
        if (!parsed) return '';
        if (parsed.type === 'img') return parsed.argument;
        return `${parsed.argument} ${iconLabel(parsed.type, parsed.argument)}`;
    }

    protected render_(): void {
        if (!this.textContent_ || !this.textElement_) return;
        const parsed = parseIconExpression(this.value_);
        const family = iconFontFamily(this.value_);

        this.labelSpan_?.remove();
        this.labelSpan_ = null;

        if (parsed && family) {
            this.textContent_.nodeValue = parsed.argument;
            this.textElement_.style.fontFamily = family;
            this.textElement_.style.fontSize = '16px';
            const span = document.createElementNS(SVG_NS, 'tspan');
            span.setAttribute('style', LABEL_STYLE);
            span.textContent = ` ${iconLabel(parsed.type, parsed.argument)}`;
            this.textElement_.appendChild(span);
            this.labelSpan_ = span;
            if (parsed.type !== 'img' && getIconName(parsed.type, parsed.argument) == null) {
                void ensureIconCatalog(parsed.type).then(() => {
                    if (this.textElement_) this.forceRerender();
                });
            }
        } else {
            this.textContent_.nodeValue = parsed ? parsed.argument : '';
            this.textElement_.style.fontFamily = '';
            this.textElement_.style.fontSize = '11px';
        }

        this._applyBackground();
        this.updateSize_();
    }

    protected updateSize_(): void {
        if (!this.textElement_ || !this.borderRect_) return;
        const bbox = this.textElement_.getBBox();
        if (!bbox.width || !bbox.height) {
            super.updateSize_();
            return;
        }
        const padX = 6;
        const padY = 2;
        const height = Math.max(bbox.height + padY * 2, 20);
        const currentX = Number(this.textElement_.getAttribute('x') || 0);
        const currentY = Number(this.textElement_.getAttribute('y') || 0);
        this.textElement_.setAttribute('x', String(padX - (bbox.x - currentX)));
        this.textElement_.setAttribute('y', String((height - bbox.height) / 2 - (bbox.y - currentY)));
        this.size_.width = bbox.width + padX * 2;
        this.size_.height = height;
        this.borderRect_.setAttribute('x', '0');
        this.borderRect_.setAttribute('y', '0');
        this.borderRect_.setAttribute('width', String(this.size_.width));
        this.borderRect_.setAttribute('height', String(this.size_.height));
    }

    protected showEditor_(): void {
        void openIconPicker({
            initialType: this.iconType_,
            expression: this.value_,
            onChange: expression => this.setValue(expression),
        });
    }

    private _applyBackground(): void {
        const rect = this.borderRect_;
        if (!rect || !this.sourceBlock_) return;
        // @ts-ignore blockly 的 BlockSvg 才带有 style
        const style = this.sourceBlock_.style;
        rect.style.fill = style.colourSecondary;
        rect.style.fillOpacity = '1';
        rect.style.stroke = style.colourTertiary;
        rect.style.strokeWidth = '1';
    }
}

Blockly.fieldRegistry.register('field_icon_picker', FieldIconPicker);

export interface FieldIconPickerConfig extends Blockly.FieldConfig {
    iconType?: IconExpressionType;
}

export interface FieldIconPickerFromJsonConfig extends FieldIconPickerConfig {
    value?: string;
}
