import { addBlock } from '../utils/blockGenerator';
import { Order } from 'blockly/javascript';
import { DEFAULT_COLOR } from '../utils/colorUtils';
import { DEFAULT_PRESET_COLOR } from '../utils/colorPalette';

// 注意：积木类型（colour_slider / colour_preset）与字段名（COLOUR）会写进存档，
// Interface 里的 BasicFields.Color 也依赖它们，所以保持不变。

addBlock(
    {
        type: 'colour_slider',
        message: '%1',
        inputs: {
            COLOUR: {
                // 与 ClassIsland / FluentAvalonia 一致的颜色选择器：预设颜色 + 自定义取色。
                type: 'field_color',
                data: {
                    color: DEFAULT_COLOR,
                },
            },
        },
        inline: false,
        style: 'my_blocks',
        output: 'SAI_Color',
        isReporter: true,
    },
    (block, generator) => {
        const color = block.getFieldValue('COLOUR') || DEFAULT_COLOR;
        return [`"${color}"`, Order.MEMBER];
    },
);

addBlock(
    {
        type: 'colour_preset',
        message: '预设颜色 %1',
        inputs: {
            COLOUR: {
                // 只提供 FluentAvalonia 的预设颜色分类。
                type: 'field_color_preset',
                data: {
                    color: DEFAULT_PRESET_COLOR,
                },
            },
        },
        inline: false,
        style: 'my_blocks',
        output: 'SAI_Color',
        isReporter: true,
    },
    (block, generator) => {
        const color = block.getFieldValue('COLOUR') || DEFAULT_PRESET_COLOR;
        return [`"${color}"`, Order.MEMBER];
    },
);
