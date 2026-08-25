import { addBlock, addLabel } from '../utils/blockGenerator';
import { Order } from 'blockly/javascript';

const date = new Date();
const year = date.getFullYear();
const month = String(date.getMonth() + 1).padStart(2, '0');
const day = String(date.getDate()).padStart(2, '0');
const today = `${year}-${month}-${day}`;
const timeNow = `${String(date.getHours()).padStart(2, '0')}:${String(date.getMinutes()).padStart(2, '0')}:${String(date.getSeconds()).padStart(2, '0')}`;

addLabel("日期")

addBlock(
    {
        type: 'date_today',
        message: '今天的日期',
        tooltip: '返回格式类似于 2026-08-21',
        inputs: {},
        inline: false,
        style: 'date_blocks',
        output: 'SAI_Date',
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
        output: 'SAI_Date',
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
        output: 'SAI_Date',
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
                check: 'SAI_Date',
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
                check: 'SAI_Date',
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
                check: 'SAI_Date',
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
                check: 'SAI_Date',
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
        output: 'SAI_Date',
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

addLabel('时间');

addBlock(
    {
        type: 'time_now',
        message: '现在的时间',
        tooltip: '返回当前时间，格式类似于 08:30:00',
        inputs: {},
        inline: false,
        style: 'date_blocks',
        output: 'SAI_Time',
        isReporter: true,
    },
    (block, generator) => {
        const wrapper = generator.provideFunction_(
            'time_now',
            `function ${generator.FUNCTION_NAME_PLACEHOLDER_}() {
                const date = new Date();
                const h = String(date.getHours()).padStart(2, '0');
                const m = String(date.getMinutes()).padStart(2, '0');
                const s = String(date.getSeconds()).padStart(2, '0');
                return \`$\{h}:$\{m}:$\{s}\`;
            }`,
        );
        return [`${wrapper}()`, Order.MEMBER];
    },
);

addBlock(
    {
        type: 'time_block',
        message: '%1',
        tooltip: '返回格式类似于 08:30:00',
        inputs: {
            TIME: {
                type: 'field_time',
                data: {
                    time: timeNow,
                },
            },
        },
        inline: false,
        style: 'date_blocks',
        output: 'SAI_Time',
        isReporter: true,
    },
    (block, generator) => {
        const time = block.getFieldValue('TIME') || timeNow;
        return [`"${time}"`, Order.MEMBER];
    },
);

addBlock(
    {
        type: 'time_generator',
        message: '%1 时 %2 分 %3 秒',
        tooltip: '构造一个时间，返回格式类似于 08:30:00',
        inputs: {
            HOUR: {
                type: 'input_value',
                blockType: 'math_number',
                check: 'Number',
                fields: { NUM: 8 },
            },
            MINUTE: {
                type: 'input_value',
                blockType: 'math_number',
                check: 'Number',
                fields: { NUM: 30 },
            },
            SECOND: {
                type: 'input_value',
                blockType: 'math_number',
                check: 'Number',
                fields: { NUM: 0 },
            },
        },
        inline: true,
        style: 'date_blocks',
        output: 'SAI_Time',
        isReporter: true,
    },
    (block, generator) => {
        const hour = generator.valueToCode(block, 'HOUR', Order.NONE) || '8';
        const minute = generator.valueToCode(block, 'MINUTE', Order.NONE) || '30';
        const second = generator.valueToCode(block, 'SECOND', Order.NONE) || '0';
        const wrapper = generator.provideFunction_(
            'time_generator',
            `function ${generator.FUNCTION_NAME_PLACEHOLDER_}(hour, minute, second) {
                const h = String(Math.trunc(hour)).padStart(2, '0');
                const m = String(Math.trunc(minute)).padStart(2, '0');
                const s = String(Math.trunc(second)).padStart(2, '0');
                return \`$\{h}:$\{m}:$\{s}\`;
            }`,
        );
        return [`${wrapper}(${hour}, ${minute}, ${second})`, Order.MEMBER];
    },
);

addBlock(
    {
        type: 'time_hour',
        message: '%1 的时',
        tooltip: '获取时间的小时',
        inputs: {
            TIME: {
                type: 'input_value' as const,
                blockType: 'time_block',
                check: 'SAI_Time',
                fields: {},
            },
        },
        inline: true,
        style: 'date_blocks',
        output: 'Number',
        isReporter: true,
    },
    (block, generator) => {
        const value = generator.valueToCode(block, 'TIME', Order.NONE) || '\"08:30:00\"';
        const wrapper = generator.provideFunction_(
            'time_hour',
            `function ${generator.FUNCTION_NAME_PLACEHOLDER_}(value) {
                const parts = String(value).split(':');
                return parseInt(parts[0], 10) || 0;
            }`,
        );
        return [`${wrapper}(${value})`, Order.MEMBER];
    },
);

addBlock(
    {
        type: 'time_minute',
        message: '%1 的分',
        tooltip: '获取时间的分钟',
        inputs: {
            TIME: {
                type: 'input_value' as const,
                blockType: 'time_block',
                check: 'SAI_Time',
                fields: {},
            },
        },
        inline: true,
        style: 'date_blocks',
        output: 'Number',
        isReporter: true,
    },
    (block, generator) => {
        const value = generator.valueToCode(block, 'TIME', Order.NONE) || '\"08:30:00\"';
        const wrapper = generator.provideFunction_(
            'time_minute',
            `function ${generator.FUNCTION_NAME_PLACEHOLDER_}(value) {
                const parts = String(value).split(':');
                return parseInt(parts[1], 10) || 0;
            }`,
        );
        return [`${wrapper}(${value})`, Order.MEMBER];
    },
);

addBlock(
    {
        type: 'time_second',
        message: '%1 的秒',
        tooltip: '获取时间的秒',
        inputs: {
            TIME: {
                type: 'input_value' as const,
                blockType: 'time_block',
                check: 'SAI_Time',
                fields: {},
            },
        },
        inline: true,
        style: 'date_blocks',
        output: 'Number',
        isReporter: true,
    },
    (block, generator) => {
        const value = generator.valueToCode(block, 'TIME', Order.NONE) || '\"08:30:00\"';
        const wrapper = generator.provideFunction_(
            'time_second',
            `function ${generator.FUNCTION_NAME_PLACEHOLDER_}(value) {
                const parts = String(value).split(':');
                return parseInt(parts[2], 10) || 0;
            }`,
        );
        return [`${wrapper}(${value})`, Order.MEMBER];
    },
);

addBlock(
    {
        type: 'time_to_minutes',
        message: '%1 转成分钟数',
        tooltip: '把时间转换成当天的总分钟数，例如 08:30:00 → 510。秒会换算成小数分钟。',
        inputs: {
            TIME: {
                type: 'input_value' as const,
                blockType: 'time_block',
                check: 'SAI_Time',
                fields: {},
            },
        },
        inline: true,
        style: 'date_blocks',
        output: 'Number',
        isReporter: true,
    },
    (block, generator) => {
        const value = generator.valueToCode(block, 'TIME', Order.NONE) || '\"08:30:00\"';
        const wrapper = generator.provideFunction_(
            'time_to_minutes',
            `function ${generator.FUNCTION_NAME_PLACEHOLDER_}(value) {
                const parts = String(value).split(':');
                const h = parseInt(parts[0], 10) || 0;
                const m = parseInt(parts[1], 10) || 0;
                const s = parseInt(parts[2], 10) || 0;
                return h * 60 + m + s / 60;
            }`,
        );
        return [`${wrapper}(${value})`, Order.MEMBER];
    },
);

addBlock(
    {
        type: 'time_add_minutes',
        message: '%1 后的第 %2 分钟',
        tooltip: '返回时间加上指定分钟数后的时间，格式类似于 08:30:00。跨天会自动回绕。',
        inputs: {
            TIME: {
                type: 'input_value' as const,
                blockType: 'time_block',
                check: 'SAI_Time',
                fields: {},
            },
            MINUTES: {
                type: 'input_value' as const,
                blockType: 'math_number',
                check: 'Number',
                fields: { NUM: 1 },
            },
        },
        inline: true,
        style: 'date_blocks',
        output: 'SAI_Time',
        isReporter: true,
    },
    (block, generator) => {
        const time = generator.valueToCode(block, 'TIME', Order.NONE) || '\"08:30:00\"';
        const minutes = generator.valueToCode(block, 'MINUTES', Order.NONE) || '1';
        const wrapper = generator.provideFunction_(
            'time_add_minutes',
            `function ${generator.FUNCTION_NAME_PLACEHOLDER_}(time, minutes) {
                const parts = String(time).split(':');
                let total = (parseInt(parts[0], 10) || 0) * 3600
                    + (parseInt(parts[1], 10) || 0) * 60
                    + (parseInt(parts[2], 10) || 0)
                    + minutes * 60;
                total = ((total % 86400) + 86400) % 86400;
                const h = Math.floor(total / 3600);
                const m = Math.floor((total % 3600) / 60);
                const s = total % 60;
                return \`$\{String(h).padStart(2, '0')}:$\{String(m).padStart(2, '0')}:$\{String(s).padStart(2, '0')}\`;
            }`,
        );
        return [`${wrapper}(${time}, ${minutes})`, Order.MEMBER];
    },
);
