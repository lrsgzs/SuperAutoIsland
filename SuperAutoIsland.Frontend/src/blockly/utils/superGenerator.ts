import { addBlock, type ArgDefinition } from './blockGenerator';
import { Order } from 'blockly/javascript';

const quote_ = (text: string) => {
    // @ts-ignore
    return "'" + text.replaceAll("'", "\\'") + "'";
};

const numberInput: ArgDefinition = {
    type: 'input_value',
    blockType: 'math_number',
    check: 'Number',
    fields: {
        NUM: 0,
    },
};

const textInput: ArgDefinition = {
    type: 'input_value',
    blockType: 'text',
    check: 'String',
    fields: {
        TEXT: '',
    },
};

const booleanInput: ArgDefinition = {
    type: 'input_value',
    blockType: 'logic_boolean',
    check: 'Boolean',
    fields: {
        BOOL: 'FALSE',
    },
};

const dummyInput: ArgDefinition = {
    type: 'input_dummy',
};

export type CommonMetaArgs = [string, 'dummy' | 'text' | 'number' | 'boolean'];
export type DropDownMetaArgs = [string, 'dropdown', [string, string][]];
export type MetaArgs = CommonMetaArgs | DropDownMetaArgs;

export interface Metadata {
    id: string;
    name: string;
    args: Record<string, MetaArgs>;
    inline?: boolean;
    dropdownUseNumbers?: boolean;
    isRule?: boolean;
}

export function addMetaBlock(metadata: Metadata) {
    // @ts-ignore
    const type = metadata.id.replaceAll('.', '_');
    let message = `[${metadata.name}]`;
    const inputs: Record<string, ArgDefinition> = {};
    const args: [string, 'field_dropdown' | 'block'][] = [];

    let i = 0;
    for (const argName in metadata.args) {
        const arg = metadata.args[argName];

        i++;
        message += ` ${arg[0]} %${i}`;

        let inputDefinition: ArgDefinition = null;
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
                if (!metadata.inline) message += '\n';
                args.push([argName, 'field_dropdown']);
                break;
            default:
                inputDefinition = dummyInput;
        }
        inputs[argName] = inputDefinition;
    }

    addBlock(
        {
            type: type,
            message: message,
            inputs: inputs,
            inline: metadata.inline,
            style: 'my_blocks',
            output: metadata.isRule ? 'Boolean' : undefined,
            isReporter: metadata.isRule,
        },
        (block, generator) => {
            let argsCode = '';
            for (let [argName, argType] of args) {
                let value;
                switch (argType) {
                    case 'block':
                        value = generator.valueToCode(block, argName, Order.NONE) || "''";
                        break;
                    case 'field_dropdown':
                        value = block.getFieldValue(argName);
                        if (!metadata.dropdownUseNumbers) value = quote_(value);
                        break;
                }
                argsCode += `${value}, `;
            }

            const functionName = metadata.isRule ? 'return getRuleState' : 'callAction';
            const wrapper = generator.provideFunction_(
                type,
                `function ${generator.FUNCTION_NAME_PLACEHOLDER_}(${args.map(a => a[0]).join(', ')}) {
                const actionId = "${metadata.id}";
                const actionData = { ${args.map(a => a[0]).join(', ')} };
                ${functionName}(actionId, actionData);
            }`,
            );
            return `${wrapper}(${argsCode});\n`;
        },
    );
}
