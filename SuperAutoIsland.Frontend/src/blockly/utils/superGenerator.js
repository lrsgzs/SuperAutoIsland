import { addBlock, data } from './blockGenerator';
import { Order } from 'blockly/javascript';
import { wsWaitMessage } from '../utils/wsUtils';
const quote_ = (text) => {
    // @ts-ignore
    return "'" + text.replaceAll("'", "\\'") + "'";
};
const numberInput = {
    type: 'input_value',
    blockType: 'math_number',
    check: 'Number',
    fields: {
        NUM: 0,
    },
};
const textInput = {
    type: 'input_value',
    blockType: 'text',
    check: 'String',
    fields: {
        TEXT: '',
    },
};
const booleanInput = {
    type: 'input_value',
    blockType: 'logic_boolean',
    check: 'Boolean',
    fields: {
        BOOL: 'FALSE',
    },
};
const dummyInput = {
    type: 'input_dummy',
};
/**
 * 添加积木
 * @param blockType 积木类型
 * @param metadata 积木元数据
 */
export async function addMetaBlock(blockType, metadata) {
    // @ts-ignore
    const id = metadata.id.replaceAll('.', '_');
    let message = `[%1 ${metadata.name}]`;
    const inputs = {};
    const args = [];
    inputs['ICON'] = {
        type: 'field_icon',
        data: {
            text: metadata.icon[0],
            icon: metadata.icon[1],
        },
    };
    let i = 1;
    for (const argName in metadata.args) {
        const arg = metadata.args[argName];
        i++;
        message += ` ${arg[0]} %${i}`;
        let inputDefinition = null;
        switch (arg[1]) {
            case 'dummy':
                inputDefinition = dummyInput;
                break;
            case 'text':
                inputDefinition = textInput;
                args.push([argName, 'block']);
                break;
            case 'number':
                inputDefinition = numberInput;
                args.push([argName, 'block']);
                break;
            case 'boolean':
                inputDefinition = booleanInput;
                args.push([argName, 'block']);
                break;
            case 'dropdown':
                inputDefinition = {
                    type: 'field_dropdown',
                    data: {
                        options: arg[2],
                    },
                };
                if (!metadata.inlineField)
                    message += '\n';
                args.push([argName, 'field_dropdown']);
                break;
            case 'dynamic_dropdown':
                let data = await wsWaitMessage(window.saiWS, { type: 'getDynamicDropdownContent', id: arg[2] });
                inputDefinition = {
                    type: 'field_dropdown',
                    data: {
                        options: data.options,
                    },
                };
                if (!metadata.inlineField)
                    message += '\n';
                args.push([argName, 'field_dropdown']);
                break;
            case 'checkbox':
                inputDefinition = {
                    type: 'field_checkbox',
                    data: {
                        checked: arg[2],
                    },
                };
                if (!metadata.inlineField)
                    message += '\n';
                args.push([argName, 'field_checkbox']);
                break;
            default:
                inputDefinition = dummyInput;
        }
        inputs[argName] = inputDefinition;
    }
    addBlock({
        type: id,
        message: message,
        inputs: inputs,
        inline: metadata.inlineBlock,
        style: 'my_blocks',
        output: blockType == 'rule' ? 'Boolean' : undefined,
        isReporter: blockType == 'rule',
    }, (block, generator) => {
        let argsCode = '(() => { let a = {};';
        for (let [argName, argType] of args) {
            let value;
            switch (argType) {
                case 'block':
                    value = generator.valueToCode(block, argName, Order.NONE) || "''";
                    break;
                case 'field_dropdown':
                    value = block.getFieldValue(argName);
                    if (!metadata.dropdownUseNumbers)
                        value = quote_(value);
                    break;
                case 'field_checkbox':
                    value = block.getFieldValue(argName);
                    value = { TRUE: true, FALSE: false }[value];
                    break;
            }
            if (argName.includes('.')) {
                const parts = argName.split('.');
                for (let i = 1; i < parts.length; i++) {
                    const path = parts.slice(0, i).join('.');
                    argsCode += `if (!a.${path}) a.${path} = {};\n`;
                }
            }
            argsCode += `a.${argName} = ${value};`;
        }
        argsCode += 'return a; })()';
        if (blockType == 'rule') {
            return [`await getRuleState("${metadata.id}", ${argsCode})\n`, Order.MEMBER];
        }
        else {
            return `await callAction("${metadata.id}", ${argsCode});\n`;
        }
    });
}
export function addLabel(label) {
    data.category?.contents.push({
        kind: 'label',
        text: label,
    });
}
