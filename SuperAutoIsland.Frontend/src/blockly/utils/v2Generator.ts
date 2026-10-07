import { addBlock, type ArgDefinition, data } from './blockGenerator';
import type { CategoryColorsInput } from './categoryColors';
import { Order } from 'blockly/javascript';
import { wsWaitMessage } from './wsUtils';

// 注意：拼字符串一律用 generator.quote_（见下面的生成器），别在这儿自己手搓转义。
// 以前这里有一份只转义单引号的 quote_，反斜杠 / 换行 / 回车全会漏，
// 插件积木里填个 C:\Users\... 就能把生成的代码搞崩（\x 这种还是非法转义）。
// generator.quote_ 在 index.ts 里被 utils/textQuoting.ts 覆盖过，补上了 \r。

export interface Field {
    name: string;
    type: string;
    options: Record<string, any>;
}

export interface InputField extends Field {
    type: 'input_value';
    check: string;
    shadowBlockType: string | null;
}

export type BlockKind = 'action' | 'rule' | 'data' | 'label';

export interface BlockMetadata {
    id: string;
    kind: BlockKind;
    name: string;
    icon: [name: string, icon: string];
    tooltip: string;
    fields: Record<string, Field>;
    inlineBlock: boolean;
    inlineField: boolean;
    dataOutput: string;
}

export interface CategoryMetadata {
    name: string;
    icon: [name: string, glyph: string];
    /** 分类配色（三个颜色），见 utils/categoryColors.ts；不填用默认蓝 */
    colors?: CategoryColorsInput | null;
}

export interface CategoryContent {
    metadata: CategoryMetadata;
    blocks: BlockMetadata[];
}

const generateField = (field: Field) => {
    return {
        type: field.type,
        data: field.options,
    };
};

const generateInputField = (field: InputField) => {
    let output = {
        type: field.type,
        check: field.check,
        fields: field.options,
        blockType: undefined as string | undefined,
    };

    if (field.shadowBlockType != null) {
        output.blockType = field.shadowBlockType;
    }
    return output;
};

export async function addV2Block(metadata: BlockMetadata) {
    if (metadata.kind == 'label') {
        data.category?.contents.push({
            kind: 'label',
            text: metadata.name,
        });
        return;
    }

    const id = metadata.id.replaceAll('.', '_');
    let message = `[%1 ${metadata.name}]`;
    const inputs: Record<string, ArgDefinition> = {};
    const fields: [string, 'field' | 'block' | 'dropdown' | 'dropdown-number', string][] = [];

    inputs['ICON'] = {
        type: 'field_icon',
        data: {
            text: metadata.icon[0],
            icon: metadata.icon[1],
        },
    };

    let i = 1;
    for (const fieldId in metadata.fields) {
        const field = metadata.fields[fieldId];

        i++;
        message += ` ${field.name} %${i}`;

        let inputDefinition: ArgDefinition | null = null;
        if (field.type === 'internal_dynamic_dropdown') {
            let data = await wsWaitMessage<{ options: [string, string][] }>(window.saiWS, {
                type: 'getDynamicDropdownContent',
                id: field.options.id,
            });
            inputDefinition = {
                type: 'field_dropdown',
                data: {
                    options: data.options,
                },
            };
            fields.push([fieldId, field.options.useNumbers ? 'dropdown-number' : 'dropdown', field.type]);
            if (!metadata.inlineField) message += '\n';
        } else if (field.type === 'field_dropdown') {
            inputDefinition = generateField(field) as ArgDefinition;
            fields.push([fieldId, field.options.useNumbers ? 'dropdown-number' : 'dropdown', field.type]);
            if (!metadata.inlineField) message += '\n';
        } else if (field.type === 'input_value') {
            inputDefinition = generateInputField(field as InputField) as ArgDefinition;
            fields.push([fieldId, 'block', field.type]);
        } else if (field.type === 'input_dummy') {
            inputDefinition = generateField(field) as ArgDefinition;
        } else {
            inputDefinition = generateField(field) as ArgDefinition;
            fields.push([fieldId, 'field', field.type]);
            if (!metadata.inlineField) message += '\n';
        }

        inputs[fieldId] = inputDefinition;
    }

    addBlock(
        {
            type: id,
            message: message,
            inputs: inputs,
            inline: metadata.inlineBlock,
            tooltip: metadata.tooltip,
            // 样式由所属分类的配色决定（preSetupCategory 注册），这里不写死
            output: metadata.kind == 'rule' ? 'Boolean' : metadata.kind == 'data' ? metadata.dataOutput : undefined,
            isReporter: metadata.kind == 'rule' || metadata.kind == 'data',
        },
        (block, generator) => {
            let argsCode = 'await (async () => { let a = {};';
            for (let [fieldId, fieldType, actualType] of fields) {
                let value;
                switch (fieldType) {
                    case 'block':
                        value = generator.valueToCode(block, fieldId, Order.NONE) || "''";
                        break;
                    case 'field':
                        value = block.getFieldValue(fieldId);
                        if (actualType == 'field_checkbox') {
                            value = { TRUE: true, FALSE: false }[value as 'TRUE' | 'FALSE'];
                        }
                        break;
                    case 'dropdown':
                        value = generator.quote_(block.getFieldValue(fieldId));
                        break;
                    case 'dropdown-number':
                        value = block.getFieldValue(fieldId);
                }

                if (
                    fieldType == 'field' &&
                    [
                        'field_input',
                        'field_variable',
                        'field_date',
                        'field_label',
                        'field_label_serializable',
                        'field_color',
                        'field_color_preset',
                        'field_colour',
                        'field_colour_hsv_sliders',
                    ].includes(actualType)
                ) {
                    value = generator.quote_(value);
                }

                if (fieldId.includes('.')) {
                    const parts = fieldId.split('.');
                    for (let i = 1; i < parts.length; i++) {
                        const path = parts.slice(0, i).join('.');
                        argsCode += `if (!a.${path}) a.${path} = {};\n`;
                    }
                }
                argsCode += `a.${fieldId} = ${value};`;
            }
            argsCode += 'return a; })()';

            if (metadata.kind == 'rule') {
                return [`await getRuleState("${metadata.id}", ${argsCode})\n`, Order.MEMBER];
            } else if (metadata.kind == 'data') {
                return [`await getData("${metadata.id}", ${argsCode})\n`, Order.MEMBER];
            } else {
                return `await callAction("${metadata.id}", ${argsCode});\n`;
            }
        },
    );
}
