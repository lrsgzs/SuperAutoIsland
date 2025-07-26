import { BlockInfo, StaticCategoryInfo } from './types/toolbox';
import * as Blockly from 'blockly/core';
import type { ConnectionState } from 'blockly/core/serialization/blocks.js';

export interface BlocklyArgDefinition {
    type: 'input_value';
    name: string;
    /* 一个只包含类名的字符串 */
    check: string;
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

interface _NumArgDef {
    type: 'input_value' | string;
    blockType: 'math_number';
    check: 'Number';
    fields?: {
        NUM?: number;
    };
}

interface _TextArgDef {
    type: 'input_value' | string;
    blockType: 'text';
    check: 'String';
    fields?: {
        TEXT?: string;
    };
}

interface _UnkArgDef {
    type: 'input_value' | string;
    blockType: string;
    /* 一个只包含类名的字符串 */
    check: string;
    fields?: Record<string, object>;
}

export type ArgDefinition = _NumArgDef | _TextArgDef | _UnkArgDef;

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
    if (data.initialized) throw new Error('无法二次初始化！');
    data.forBlock = forBlock;
    data.category = category;
    data.blocks = blocks;
    data.initialized = true;
}

export function generateBlock(block: BlockDefinition): GeneratorOutput {
    const toolboxInputs = {};
    const definitionArgs = [];
    for (let inputName in block.inputs) {
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

        definitionArgs.push({
            type: 'input_value',
            name: inputName,
            check: block.inputs[inputName].check,
        });
    }

    return {
        toolbox: {
            kind: 'block',
            type: block.type,
            inputs: toolboxInputs,
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
    const blockDefinition = generateBlock(block);
    data.category.contents.push(blockDefinition.toolbox);
    data.blocks.push(blockDefinition.definition);
    data.forBlock[block.type] = generator;
}
