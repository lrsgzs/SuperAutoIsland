import * as Blockly from 'blockly';

type IconData = {
    icon: string;
    text: string;
};

/**
 * icon 字段
 */
export class FieldIcon extends Blockly.Field<IconData> {
    private displayValue_: IconData | undefined;
    private isValueValid_: boolean | undefined;

    constructor(value: IconData, validator?: Blockly.FieldValidator<IconData>) {
        super(value, validator);

        this.SERIALIZABLE = false;
        this.EDITABLE = false;
    }

    static fromJson(_options: Blockly.FieldConfig & IconData) {
        const icon = Blockly.utils.parsing.replaceMessageReferences(_options.icon);
        const text = Blockly.utils.parsing.replaceMessageReferences(_options.text);
        return new FieldIcon({ icon, text });
    }

    doClassValidation_(newValue: IconData) {
        if (typeof newValue.icon != 'string' || typeof newValue.text != 'string') {
            return null;
        }
        return newValue;
    }

    doValueUpdate_(newValue: IconData) {
        super.doValueUpdate_(newValue);
        this.displayValue_ = newValue;
        this.isValueValid_ = true;
    }

    doValueInvalid_(newValue: IconData) {
        this.displayValue_ = newValue;
        this.isDirty_ = true;
        this.isValueValid_ = false;
    }

    getText() {
        return `(${this.displayValue_?.text})`;
    }

    render_() {
        this.textContent_!.nodeValue = this.value_!.icon;
        this.textElement_!.style.fontFamily = "'Fluent System Icons'";
        this.textElement_!.style.fontSize = '16px';
        this._changeColor();
        this.updateSize_();
    }

    private _changeColor() {
        const sourceBlock = this.sourceBlock_;
        if (sourceBlock!.isShadow()) {
            this.textElement_!.parentElement!
                .querySelector('rect')!
                // @ts-ignore
                .setAttribute('fill', sourceBlock.style.colourSecondary);
        } else {
            this.textElement_!.parentElement!
                .querySelector('rect')!
                // @ts-ignore
                .setAttribute('fill', sourceBlock.style.colourPrimary);
        }
    }

    updateSize_() {
        const bbox = this.textElement_!.getBBox();
        let width = bbox.width;
        let height = bbox.height;
        this.size_.width = width - 17;
        this.size_.height = height - 12;
        this.borderRect_!.setAttribute('width', '0');
        this.borderRect_!.setAttribute('height', '0');
        this.textElement_!.style.transform = `translate(-9px, 19px)`;
    }
}
