import { addBlock } from '../utils/blockGenerator';
import { Order } from 'blockly/javascript';
import { formatIconExpression } from '../utils/iconExpression';

const DEFAULT_FLUENT = formatIconExpression('fluent', '\ue9b0');

addBlock(
    {
        type: 'icon',
        message: '图标 %1',
        tooltip: '选择一个 Fluent / Lucide 图标，或填写图片路径，返回图标表达式。',
        inputs: {
            ICON: {
                type: 'field_icon_picker',
                data: {
                    iconType: 'fluent',
                    value: DEFAULT_FLUENT,
                },
            },
        },
        inline: false,
        style: 'my_blocks',
        output: 'SAI_Icon',
        isReporter: true,
    },
    block => {
        const expression = block.getFieldValue('ICON') || DEFAULT_FLUENT;
        return [JSON.stringify(expression), Order.ATOMIC];
    },
);
