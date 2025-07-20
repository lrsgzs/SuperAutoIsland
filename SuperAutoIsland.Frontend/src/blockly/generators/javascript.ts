import { Order } from 'blockly/javascript';
import * as Blockly from 'blockly/core';

export const forBlock: { [block: string]: (block: Blockly.Block, generator: Blockly.CodeGenerator) => string } =
    Object.create(null);

forBlock['show_alert'] = function (block: Blockly.Block, generator: Blockly.CodeGenerator) {
    const text = generator.valueToCode(block, 'TEXT', Order.NONE) || "''";
    const addText = generator.provideFunction_(
        'showAlert',
        `function ${generator.FUNCTION_NAME_PLACEHOLDER_}(text) {
                   alert(text);
               }`,
    );
    return `${addText}(${text});\n`;
};
