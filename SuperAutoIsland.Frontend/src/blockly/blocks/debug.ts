import { addBlock } from '../utils/blockGenerator';
import { Order } from 'blockly/javascript';

addBlock(
    {
        type: 'throw_data',
        message: '丢弃值 %1',
        inputs: {
            VALUE: {
                type: 'input_value',
            },
        },
        inline: false,
        style: 'debug_blocks',
    },
    (block, generator) => {
        const value = generator.valueToCode(block, 'VALUE', Order.NONE) || "''";
        return `${value};\n`;
    },
);

addBlock(
    {
        type: 'console_debug',
        message: '[%1 控制台] debug %2',
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
        style: 'debug_blocks',
    },
    (block, generator) => {
        const value = generator.valueToCode(block, 'VALUE', Order.NONE) || "''";
        return `console.debug(${value});\n`;
    },
);

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
        style: 'debug_blocks',
    },
    (block, generator) => {
        const value = generator.valueToCode(block, 'VALUE', Order.NONE) || "''";
        return `console.log(${value});\n`;
    },
);

addBlock(
    {
        type: 'console_warn',
        message: '[%1 控制台] warn %2',
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
        style: 'debug_blocks',
    },
    (block, generator) => {
        const value = generator.valueToCode(block, 'VALUE', Order.NONE) || "''";
        return `console.warn(${value});\n`;
    },
);

addBlock(
    {
        type: 'console_error',
        message: '[%1 控制台] error %2',
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
        style: 'debug_blocks',
    },
    (block, generator) => {
        const value = generator.valueToCode(block, 'VALUE', Order.NONE) || "''";
        return `console.error(${value});\n`;
    },
);
