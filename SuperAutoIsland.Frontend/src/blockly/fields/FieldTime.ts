/**
 * @license
 * SPDX-License-Identifier: Apache-2.0
 */

/**
 * @fileoverview 一个使用浏览器原生时间选择器的 Blockly 时间字段，参考 @blockly/field-date。
 */
import * as Blockly from 'blockly/core';

/**
 * Class for a time input field.
 */
export class FieldTime extends Blockly.FieldTextInput {
    /**
     * Serializable fields are saved by the XML renderer, non-serializable fields
     * are not. Editable fields should also be serializable.
     */
    SERIALIZABLE = true;

    /**
     * Class for a time input field. Derived from the Closure library time
     * picker.
     *
     * @param value The initial value of the field. Should be in
     *    'HH:mm:ss' format. Defaults to the current time.
     * @param validator A function that is called to validate
     *    changes to the field's value. Takes in a time string & returns a
     *    validated time string ('HH:mm:ss' format), or null to abort the
     *    change.
     * @param config A map of options used to configure the field.
     */
    constructor(
        value?: string,
        validator?: FieldTimeValidator,
        config?: FieldTimeConfig,
    ) {
        super(value, validator, config);
    }

    /**
     * Constructs a FieldTime from a JSON arg object.
     *
     * @param options A JSON object with options (time).
     * @returns The new field instance.
     * @package
     * @nocollapse
     */
    static fromJson(options: FieldTimeFromJsonConfig): FieldTime {
        const { time, ...fieldTimeConfig } = options;
        // `this` might be a subclass of FieldTime if that class doesn't
        // override the static fromJson method.
        return new this(time, undefined, fieldTimeConfig);
    }

    /* eslint-disable @typescript-eslint/naming-convention */
    /**
     * Ensures that the input value is a valid time.
     *
     * @param newValue The input value. Ex: '08:30:00' / '08:30'
     * @returns A valid time string ('HH:mm:ss'), or null if invalid.
     * @override
     */
    protected doClassValidation_(newValue?: string): string | null {
        if (!newValue) return null;
        return normalizeTime(newValue);
    }

    /**
     * Get the text to display on the block when the input hasn't spawned in.
     *
     * @returns The text to display on the block.
     * @override
     */
    protected getText_(): string | null {
        const value = this.getValue();
        if (!value) return null;
        return value;
    }

    /**
     * Renders the field. If the picker is shown make sure it has the current
     * time selected.
     */
    protected render_() {
        super.render_();
    }

    /**
     * Shows the inline free-text editor on top of the text along with the time
     * editor.
     *
     * @param e Optional mouse event that triggered the field to
     *     open, or undefined if triggered programmatically.
     * @override
     */
    protected showEditor_(e?: Event) {
        // Pass in `true` for `quietInput` to disable modal inputs for the time
        // block without setting `this.sourceBlock_.workspace.options.modalInputs`,
        // which would impact the entire workspace.
        super.showEditor_(e, true);

        // Even though `quietInput` was set true, focus on the element.
        this.htmlInput_?.focus({
            preventScroll: true,
        });
        this.htmlInput_?.select();
        this.showDropdown();
    }

    /**
     * Updates the size of the field based on the text.
     *
     * @param margin margin to use when positioning the text element.
     * @override
     */
    protected updateSize_(margin?: number) {
        // Add margin so that the time input's clock icon doesn't clip with
        // the text when sized for the time.
        super.updateSize_((margin ?? 0) + 20);
    }

    /**
     * Shows the time picker.
     */
    private showDropdown(): void {
        if (!this.htmlInput_) return;
        Blockly.utils.dom.addClass(this.htmlInput_, 'blocklyTimeInput');

        // Delay showing the picker until the editor has a chance to position
        window.requestAnimationFrame(() => {
            // NOTE: HTMLInputElement.showPicker() is not available in earlier
            // TypeScript versions (like 4.7.4), so casting to `any` to be
            // compatible with dev scripts. Additionally, it's not available for
            // time inputs for Safari. For browser compatibility of showPicker,
            // see:
            // https://developer.mozilla.org/en-US/docs/Web/API/HTMLInputElement/showPicker
            /* eslint-disable @typescript-eslint/no-explicit-any */
            (this.htmlInput_ as any).showPicker();
            /* eslint-enable @typescript-eslint/no-explicit-any */
        });
    }

    /**
     * Create the html input and set it to type time.
     *
     * @returns The newly created time input editor.
     */
    protected widgetCreate_(): HTMLInputElement {
        // NOTE: field_input should return HTMLInputElement for this.
        const htmlInput = super.widgetCreate_() as HTMLInputElement;
        htmlInput.type = 'time';
        // 允许秒，这样浏览器输入框的值格式为 'HH:mm:ss'。
        htmlInput.step = '1';

        return htmlInput;
    }
    /* eslint-enable @typescript-eslint/naming-convention */
}

if (Blockly.utils.userAgent.MAC) {
    // NOTE: By default, 4 px padding total are added within the User Agent
    // Shadow Content on Safari on MAC. Remove the padding so the inner input
    // matches the outer input's height and, by extension, the height of the text
    // node.
    Blockly.Css.register(`
input.blocklyTimeInput::-webkit-datetime-edit,
input.blocklyTimeInput::-webkit-datetime-edit-hours-field,
input.blocklyTimeInput::-webkit-datetime-edit-minutes-field,
input.blocklyTimeInput::-webkit-datetime-edit-seconds-field,
input.blocklyTimeInput::-webkit-datetime-edit-ampm-field {
  padding: 0;
}
`);
}

Blockly.fieldRegistry.register('field_time', FieldTime);

/**
 * A config object for defining a field time.
 */
export interface FieldTimeConfig extends Blockly.FieldTextInputConfig {
    // NOTE: spellcheck is defined for FieldInput though irrelevant for FieldTime.
    spellcheck?: never;
}

/**
 * Options used to define a field time from JSON.
 */
export interface FieldTimeFromJsonConfig extends FieldTimeConfig {
    time?: string;
}

export type FieldTimeValidator = Blockly.FieldTextInputValidator;

/**
 * 校验并规范化时间字符串为 'HH:mm:ss' 格式。
 *
 * @param value 要校验的值，支持 'HH:mm' 或 'HH:mm:ss'。
 * @returns 规范化后的 'HH:mm:ss' 字符串；如果无效则返回 null。
 */
export function normalizeTime(value: string): string | null {
    const match = value.match(/^(\d{1,2}):(\d{1,2})(?::(\d{1,2}))?$/);
    if (!match) return null;
    const h = Number(match[1]);
    const m = Number(match[2]);
    const s = match[3] !== undefined ? Number(match[3]) : 0;
    if (h > 23 || m > 59 || s > 59) return null;
    return `${String(h).padStart(2, '0')}:${String(m).padStart(2, '0')}:${String(s).padStart(2, '0')}`;
}

/**
 * 获取当前时间的 'HH:mm:ss' 字符串。
 */
export function getCurrentTimeString(): string {
    const date = new Date();
    const h = String(date.getHours()).padStart(2, '0');
    const m = String(date.getMinutes()).padStart(2, '0');
    const s = String(date.getSeconds()).padStart(2, '0');
    return `${h}:${m}:${s}`;
}

// NOTE: Set default here instead of in class so it's available at Field.
FieldTime.prototype.DEFAULT_VALUE = getCurrentTimeString();
