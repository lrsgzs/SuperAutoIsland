import { addBlock } from '../blockGenerator';
import { Order } from 'blockly/javascript';

addBlock(
    {
        type: 'show_alert',
        message: 'Show Alert %1',
        inputs: {
            TEXT: {
                type: 'input_value',
                blockType: 'text',
                check: 'String',
                fields: {
                    TEXT: 'Hello',
                },
            },
        },
        tooltip: 'Show a alert to user.',
        style: 'my_blocks',
    },
    (block, generator) => {
        const text = generator.valueToCode(block, 'TEXT', Order.NONE) || "''";
        const addText = generator.provideFunction_(
            'showAlert',
            `function ${generator.FUNCTION_NAME_PLACEHOLDER_}(text) {
                alert(text);
            }`,
        );
        return `${addText}(${text});\n`;
    },
);
