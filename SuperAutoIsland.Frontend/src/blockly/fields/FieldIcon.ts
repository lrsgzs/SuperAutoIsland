import * as Blockly from 'blockly';

type IconData = {
    icon: string;
    text: string;
};

/** 图标字号，与积木正文相近但略大一点，方便辨认。 */
const ICON_FONT_SIZE = 16;
/** 量不到字形（尚未排版）时的兜底尺寸。 */
const FALLBACK_SIZE = 12;
/** 图标字体族名。 */
const ICON_FONT_FAMILY = 'Fluent System Icons';
/** 取不到渲染器常量时，退回到 thrasos 的 MEDIUM_PADDING。 */
const DEFAULT_FIELD_SPACING = 5;

/**
 * 积木图标字段：只显示一个 Fluent 图标字形，不可编辑、不参与序列化。
 *
 * 字段尺寸按字形的实际墨迹（ink box）计算，并把墨迹居中放进字段里，
 * 这样图标与前后文字能对齐，也不会互相挤压。
 */
export class FieldIcon extends Blockly.Field<IconData> {
    /** 图标字体的加载请求，全局只发起一次。 */
    private static fontPromise: Promise<void> | null = null;

    /** 字体加载完成后是否已经重新排版过。 */
    private fontApplied = false;

    constructor(value: IconData, validator?: Blockly.FieldValidator<IconData>) {
        super(value, validator);

        this.SERIALIZABLE = false;
        this.EDITABLE = false;
    }

    static fromJson(options: Blockly.FieldConfig & IconData) {
        const icon = Blockly.utils.parsing.replaceMessageReferences(options.icon);
        const text = Blockly.utils.parsing.replaceMessageReferences(options.text);
        return new FieldIcon({ icon, text });
    }

    protected override initView(): void {
        super.initView();
        // 图标字段不需要背景与边框，直接隐藏（保留元素以免影响 Blockly 的排版逻辑）
        if (this.borderRect_) {
            this.borderRect_.style.display = 'none';
        }
        this.fieldGroup_?.classList.add('saiBlockIcon');
    }

    protected override doClassValidation_(newValue: IconData): IconData | null {
        if (typeof newValue?.icon != 'string' || typeof newValue?.text != 'string') {
            return null;
        }
        return newValue;
    }

    protected override doValueUpdate_(newValue: IconData) {
        super.doValueUpdate_(newValue);
        this.value_ = newValue;
    }

    getText(): string {
        return `(${this.value_?.text ?? ''})`;
    }

    protected override render_(): void {
        if (!this.textContent_ || !this.textElement_) return;

        this.textContent_.nodeValue = this.value_?.icon ?? '';
        this.textElement_.style.fontFamily = `'${ICON_FONT_FAMILY}'`;
        this.textElement_.style.fontSize = `${ICON_FONT_SIZE}px`;
        this.updateSize_();

        // 首次排版时图标字体可能还没下载完，此时量到的是回退字形的尺寸，
        // 会让图标产生偏移；字体就绪后重新量一次。
        this.scheduleRemeasureOnFontReady();
    }

    protected override updateSize_(): void {
        const textElement = this.textElement_;
        if (!textElement) return;

        const glyph = this.value_?.icon ?? '';
        if (!glyph) {
            this.size_ = new Blockly.utils.Size(0, 0);
            return;
        }

        const bbox = this.measure_(textElement);
        const spacing = this.getFieldSpacing_();
        const height = Math.max(bbox.height, FALLBACK_SIZE);
        // Blockly 会在字段两侧各留出一份字段间距（MEDIUM_PADDING）。这里让字段框比
        // 字形窄两份间距，再让字形居中溢出到这两份间距里，图标就能紧贴前后文字。
        const width = Math.max(0, bbox.width - spacing * 2);
        this.size_ = new Blockly.utils.Size(width, height);

        // getBBox 返回的是相对于元素自身坐标系的墨迹框，这里把它挪到字段框的正中间
        const currentX = Number(textElement.getAttribute('x') || 0);
        const currentY = Number(textElement.getAttribute('y') || 0);
        textElement.setAttribute('x', String((width - bbox.width) / 2 - (bbox.x - currentX)));
        textElement.setAttribute('y', String((height - bbox.height) / 2 - (bbox.y - currentY)));
    }

    /**
     * 取当前渲染器的字段间距（thrasos / geras / zelos 都是 MEDIUM_PADDING）。
     */
    private getFieldSpacing_(): number {
        try {
            const workspace = this.sourceBlock_?.workspace as Blockly.WorkspaceSvg | undefined;
            const spacing = workspace?.getRenderer?.().getConstants?.().MEDIUM_PADDING;
            if (typeof spacing === 'number' && spacing > 0) return spacing;
        } catch {
            // 工作区或渲染器不可用时使用默认值
        }
        return DEFAULT_FIELD_SPACING;
    }

    /**
     * 图标字体就绪后重新排版一次。
     */
    private scheduleRemeasureOnFontReady(): void {
        if (this.fontApplied) return;

        const fonts = document.fonts;
        if (!fonts?.load) {
            this.fontApplied = true;
            return;
        }

        FieldIcon.fontPromise ??= fonts
            .load(`${ICON_FONT_SIZE}px '${ICON_FONT_FAMILY}'`)
            .then(() => undefined)
            .catch(() => undefined);

        void FieldIcon.fontPromise.then(() => {
            if (this.fontApplied || !this.textElement_) return;
            this.fontApplied = true;
            this.forceRerender();
        });
    }

    /**
     * 量取字形墨迹，尚未排版时按字号估算，下一次渲染会自行修正。
     */
    private measure_(element: SVGTextElement): { x: number; y: number; width: number; height: number } {
        try {
            const bbox = element.getBBox();
            if (bbox.width && bbox.height) {
                return { x: bbox.x, y: bbox.y, width: bbox.width, height: bbox.height };
            }
        } catch {
            // 元素尚未插入文档时部分浏览器会抛出异常，忽略并使用估算值。
        }
        return { x: 0, y: 0, width: FALLBACK_SIZE, height: FALLBACK_SIZE };
    }
}
