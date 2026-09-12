import { addBlock, addLabel } from '../utils/blockGenerator';
import { Order } from 'blockly/javascript';

addLabel('颜色');

addBlock(
    {
        type: 'colour_slider',
        message: '%1',
        inputs: {
            COLOUR: {
                type: 'field_colour_hsv_sliders',
                data: {
                    colour: '#FF0000',
                },
            },
        },
        inline: false,
        style: 'my_blocks',
        output: 'SAI_Color',
        isReporter: true,
    },
    (block, generator) => {
        const colour = block.getFieldValue('COLOUR') || '#FF0000';
        return [`"${colour}"`, Order.MEMBER];
    },
);

addBlock(
    {
        type: 'colour_preset',
        message: '预设颜色 %1',
        inputs: {
            COLOUR: {
                type: 'field_colour',
                data: {
                    colour: '#FF0000',
                },
            },
        },
        inline: false,
        style: 'my_blocks',
        output: 'SAI_Color',
        isReporter: true,
    },
    (block, generator) => {
        const colour = block.getFieldValue('COLOUR') || '#FF0000';
        return [`"${colour}"`, Order.MEMBER];
    },
);
