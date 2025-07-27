import type { BlockInfo, StaticCategoryInfo } from '../types/toolbox';
import * as Blockly from 'blockly/core';
import type { ConnectionState } from 'blockly/core/serialization/blocks.js';

export interface BlocklyArgDefinition {
    type: 'input_value';
    name: string;
    /* 一个只包含类名的字符串 */
    check?: string;
    checked?: boolean;
}

export interface BlocklyBlockDefinition {
    type: string;
    message0: string;
    args0: BlocklyArgDefinition[];
    /* 一个只包含类名的字符串 */
    output?: string;
    colour?: number;
    tooltip?: string;
    helpUrl?: string;
    style?: string;
    previousStatement?: ConnectionState;
    nextStatement?: ConnectionState;
}

interface _ImageDef {
    src: string;
    width: number;
    height: number;
    alt: string;
}

interface _UnkArgDef {
    type: string;
    data?: object;
}

interface _DummyArgDef extends _UnkArgDef {
    type: 'input_dummy';
}

interface _BlockArgDef extends _UnkArgDef {
    type: 'input_value';
    blockType: string;
    /* 一个只包含类名的字符串 */
    check: string;
    fields: Record<string, any>;
}

interface _NumberBlockArgDef extends _BlockArgDef {
    blockType: 'math_number';
    check: 'Number';
    fields: {
        TEXT: string;
    };
}

interface _TextBlockArgDef extends _BlockArgDef {
    blockType: 'text';
    check: 'String';
    fields: {
        NUM: number;
    };
}

interface _BooleanBlockArgDef extends _BlockArgDef {
    blockType: 'logic_boolean';
    check: 'Boolean';
    fields: {
        BOOL: 'TRUE' | 'FALSE';
    };
}

interface _CheckboxArgDef extends _UnkArgDef {
    type: 'field_checkbox';
    data: {
        checked?: boolean;
    };
}

interface _DropDownArgDef extends _UnkArgDef {
    type: 'field_dropdown';
    data: {
        options: ([string | _ImageDef, string] | 'separator')[];
    };
}

interface _ImageArgDef extends _UnkArgDef {
    type: 'field_image';
    data: {
        src: string;
        width: number;
        height: number;
        alt: string;
    };
}

interface _LabelArgDef extends _UnkArgDef {
    type: 'field_label';
    data: {
        text: string;
    };
}

interface _SerializableLabelArgDef extends _UnkArgDef {
    type: 'field_label_serializable';
    data: {
        text: string;
    };
}

interface _NumberArgDef extends _UnkArgDef {
    type: 'field_number';
    data: {
        value?: number;
        min?: number;
        max?: number;
        precision?: number;
    };
}

interface _InputArgDef extends _UnkArgDef {
    type: 'field_input';
    data: {
        text?: string;
        spellcheck?: boolean;
    };
}

interface _VariableArgDef extends _UnkArgDef {
    type: 'field_variable';
    data: {
        variable?: string;
    };
}

export type ArgDefinition =
    | _DummyArgDef
    | _BlockArgDef
    | _NumberBlockArgDef
    | _BooleanBlockArgDef
    | _TextBlockArgDef
    | _CheckboxArgDef
    | _DropDownArgDef
    | _ImageArgDef
    | _LabelArgDef
    | _SerializableLabelArgDef
    | _NumberArgDef
    | _InputArgDef
    | _VariableArgDef;

export interface BlockDefinition {
    type: string;
    inputs: Record<string, ArgDefinition>;
    message: string;
    /* 一个只包含类名的字符串 */
    output?: string;
    tooltip?: string;
    helpUrl?: string;
    colour?: number;
    style?: string;
    inline?: boolean;
}

export interface GeneratorOutput {
    toolbox: BlockInfo;
    definition: BlocklyBlockDefinition;
}

export type GeneratorFunction = (block: Blockly.Block, generator: Blockly.CodeGenerator) => string;

interface _DataObject {
    initialized: boolean;
    forBlock?: Record<string, GeneratorFunction>;
    category?: StaticCategoryInfo;
    blocks?: BlocklyBlockDefinition[];
}

export const data: _DataObject = {
    initialized: false,
};

export function setup(
    forBlock: Record<string, GeneratorFunction>,
    category: StaticCategoryInfo,
    blocks: BlocklyBlockDefinition[],
) {
    data.forBlock = forBlock;
    data.category = category;
    data.blocks = blocks;
    data.initialized = true;
}

export function generateBlock(block: BlockDefinition): GeneratorOutput {
    if (!data.initialized) throw new Error('未初始化 generator!');

    const toolboxInputs = {};
    const definitionArgs = [];
    for (let inputName in block.inputs) {
        if (block.inputs[inputName].type == 'input_value') {
            const toolboxFields = {};
            for (let fieldName in block.inputs[inputName].fields) {
                toolboxFields[fieldName] = block.inputs[inputName].fields[fieldName];
            }

            toolboxInputs[inputName] = {
                shadow: {
                    type: block.inputs[inputName].blockType,
                    fields: toolboxFields,
                },
            };
        }

        definitionArgs.push({
            name: inputName,
            ...block.inputs[inputName],
            ...block.inputs[inputName].data,
        });
    }

    return {
        toolbox: {
            kind: 'block',
            type: block.type,
            inputs: toolboxInputs,
            inline: block.inline,
        },
        definition: {
            type: block.type,
            message0: block.message,
            args0: definitionArgs,
            output: block.output,
            tooltip: block.tooltip,
            helpUrl: block.helpUrl,
            colour: block.colour,
            style: block.style,
            previousStatement: null,
            nextStatement: null,
        },
    };
}

export function addBlock(block: BlockDefinition, generator: GeneratorFunction) {
    if (!data.initialized) throw new Error('未初始化 generator!');

    const blockDefinition = generateBlock(block);
    data.category.contents.push(blockDefinition.toolbox);
    data.blocks.push(blockDefinition.definition);
    data.forBlock[block.type] = generator;
}
