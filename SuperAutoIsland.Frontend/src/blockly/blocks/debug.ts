import { addBlock } from '../utils/blockGenerator';
import { Order } from 'blockly/javascript';

addBlock(
    {
        type: 'console_log',
        message: '[%1 控制台] log %2',
        inputs: {
            ICON: {
                type: 'field_icon',
                data: {
                    text: '控制台',
                    icon: '\uF498',
                },
            },
            VALUE: {
                type: 'input_value',
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
