import { addBlock } from '../utils/blockGenerator';
import { Order } from 'blockly/javascript';

addBlock(
    {
        type: 'console_log',
        message: '[控制台] log %1',
        inputs: {
            VALUE: {
                type: 'input_value',
                check: null,
            },
        },
        inline: false,
        style: 'my_blocks',
    },
    (block, generator) => {
        const value = generator.valueToCode(block, 'VALUE', Order.NONE) || "''";
        return `console.log(${value});\n`;
    },
);
