import { addBlock } from '../utils/blockGenerator';
import { Order } from 'blockly/javascript';

const date = new Date();
const year = date.getFullYear();
const month = String(date.getMonth() + 1).padStart(2, '0');
const day = String(date.getDate()).padStart(2, '0');
const today = `${year}-${month}-${day}`;

addBlock(
    {
        type: 'date_today',
        message: '今天的日期',
        tooltip: '返回格式类似于 2026-08-21',
        inputs: {},
        inline: false,
        style: 'date_blocks',
        output: 'String',
        isReporter: true,
    },
    (block, generator) => {
        const wrapper = generator.provideFunction_(
            'date_today',
            `function ${generator.FUNCTION_NAME_PLACEHOLDER_}() {
                const date = new Date();
                const year = date.getFullYear();
                const month = String(date.getMonth() + 1).padStart(2, '0');
                const day = String(date.getDate()).padStart(2, '0');
                return \`$\{year}-$\{month}-$\{day}\`;
            }`,
        );
        return [`${wrapper}()`, Order.MEMBER];
    },
);

addBlock(
    {
        type: 'date_block',
        message: '%1',
        tooltip: '返回格式类似于 2026-08-21',
        inputs: {
            DATE: {
                type: 'field_date',
                data: {
                    date: today,
                },
            },
        },
        inline: false,
        style: 'date_blocks',
        output: 'String',
        isReporter: true,
    },
    (block, generator) => {
        const date = block.getFieldValue('DATE') || '2026-08-21';
        return [`"${date}"`, Order.MEMBER];
    },
);

addBlock(
    {
        type: 'date_generator',
        message: '%1 年 %2 月 %3 日',
        tooltip: '返回格式类似于 2026-08-21',
        inputs: {
            YEAR: {
                type: 'input_value',
                blockType: 'math_number',
                check: 'Number',
                fields: {
                    NUM: new Date().getFullYear(),
                },
            },
            MONTH: {
                type: 'input_value',
                blockType: 'math_number',
                check: 'Number',
                fields: {
                    NUM: new Date().getMonth() + 1,
                },
            },
            DAY: {
                type: 'input_value',
                blockType: 'math_number',
                check: 'Number',
                fields: {
                    NUM: new Date().getDate(),
                },
            },
        },
        inline: true,
        style: 'date_blocks',
        output: 'String',
        isReporter: true,
    },
    (block, generator) => {
        const year = generator.valueToCode(block, 'YEAR', Order.NONE) || '2017';
        const month = generator.valueToCode(block, 'MONTH', Order.NONE) || '7';
        const day = generator.valueToCode(block, 'DAY', Order.NONE) || '28';
        const wrapper = generator.provideFunction_(
            'date_generator',
            `function ${generator.FUNCTION_NAME_PLACEHOLDER_}(year, month, day) {
                const date = new Date(year, month - 1, day);
                const yearD = date.getFullYear();
                const monthD = String(date.getMonth() + 1).padStart(2, '0');
                const dayD = String(date.getDate()).padStart(2, '0');
                return \`$\{yearD}-$\{monthD}-$\{dayD}\`;
            }`,
        );
        return [`${wrapper}(${year}, ${month}, ${day})`, Order.MEMBER];
    },
);

addBlock(
    {
        type: 'date_year',
        message: '%1 的年',
        inputs: {
            DATE: {
                type: 'input_value' as const,
                blockType: 'date_block',
                check: 'String',
                fields: {},
            },
        },
        inline: true,
        style: 'date_blocks',
        output: 'Number',
        isReporter: true,
    },
    (block, generator) => {
        const value = generator.valueToCode(block, 'DATE', Order.NONE) || '\"2026-08-21\"';
        const wrapper = generator.provideFunction_(
            'date_year',
            `function ${generator.FUNCTION_NAME_PLACEHOLDER_}(value) {
                return new Date(value + 'T00:00:00').getFullYear();
            }`,
        );
        return [`${wrapper}(${value})`, Order.MEMBER];
    },
);

addBlock(
    {
        type: 'date_month',
        message: '%1 的月',
        inputs: {
            DATE: {
                type: 'input_value' as const,
                blockType: 'date_block',
                check: 'String',
                fields: {},
            },
        },
        inline: true,
        style: 'date_blocks',
        output: 'Number',
        isReporter: true,
    },
    (block, generator) => {
        const value = generator.valueToCode(block, 'DATE', Order.NONE) || '\"2026-08-21\"';
        const wrapper = generator.provideFunction_(
            'date_month',
            `function ${generator.FUNCTION_NAME_PLACEHOLDER_}(value) {
                return new Date(value + 'T00:00:00').getMonth() + 1;
            }`,
        );
        return [`${wrapper}(${value})`, Order.MEMBER];
    },
);

addBlock(
    {
        type: 'date_day',
        message: '%1 的日',
        inputs: {
            DATE: {
                type: 'input_value' as const,
                blockType: 'date_block',
                check: 'String',
                fields: {},
            },
        },
        inline: true,
        style: 'date_blocks',
        output: 'Number',
        isReporter: true,
    },
    (block, generator) => {
        const value = generator.valueToCode(block, 'DATE', Order.NONE) || '\"2026-08-21\"';
        const wrapper = generator.provideFunction_(
            'date_day',
            `function ${generator.FUNCTION_NAME_PLACEHOLDER_}(value) {
                return new Date(value + 'T00:00:00').getDate();
            }`,
        );
        return [`${wrapper}(${value})`, Order.MEMBER];
    },
);

addBlock(
    {
        type: 'date_add_days',
        message: '%1 后的第 %2 天',
        inputs: {
            DATE: {
                type: 'input_value' as const,
                blockType: 'date_block',
                check: 'String',
                fields: {},
            },
            DAYS: {
                type: 'input_value' as const,
                blockType: 'math_number',
                check: 'Number',
                fields: { NUM: 1 },
            },
        },
        inline: true,
        style: 'date_blocks',
        output: 'String',
        isReporter: true,
    },
    (block, generator) => {
        const value = generator.valueToCode(block, 'DATE', Order.NONE) || '\"2026-08-21\"';
        const days = generator.valueToCode(block, 'DAYS', Order.NONE) || '1';
        const wrapper = generator.provideFunction_(
            'date_add_days',
            `function ${generator.FUNCTION_NAME_PLACEHOLDER_}(value, days) {
                const date = new Date(value + 'T00:00:00');
                date.setDate(date.getDate() + days);
                const year = date.getFullYear();
                const month = String(date.getMonth() + 1).padStart(2, '0');
                const day = String(date.getDate()).padStart(2, '0');
                return \`$\{year\}-$\{month\}-$\{day\}\`;
            }`,
        );
        return [`${wrapper}(${value}, ${days})`, Order.MEMBER];
    },
);
