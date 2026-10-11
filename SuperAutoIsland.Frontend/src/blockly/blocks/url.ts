import { addBlock } from '../utils/blockGenerator';
import { Order } from 'blockly/javascript';

/**
 * URLSearchParams 积木（解析 / 生成查询字符串）。
 *
 * 生成的是普通 JS，跑在后端 Jint 上（`URLSearchParams` 在后端运行时里可用）。
 */

addBlock(
    {
        type: 'url_search_params_parse',
        message: '解析 URLSearchParams %1',
        inputs: {
            TEXT: {
                type: 'input_value',
                blockType: 'text',
                check: 'String',
                fields: {
                    TEXT: 'a=1&b=2',
                },
            },
        },
        inline: false,
        style: 'my_blocks',
        output: 'Dictionary',
        tooltip: '把 a=1&b=2 这样的查询字符串解析成字典：值会做百分号解码，重复的键取最后一个。',
        isReporter: true,
    },
    (block, generator) => {
        const text = generator.valueToCode(block, 'TEXT', Order.NONE) || "''";
        return [`Object.fromEntries(new URLSearchParams(${text}).entries())`, Order.FUNCTION_CALL];
    },
);

addBlock(
    {
        type: 'url_search_params_generate',
        message: '生成 URLSearchParams %1',
        inputs: {
            DICT: {
                type: 'input_value',
                check: 'Dictionary',
            },
        },
        inline: false,
        style: 'my_blocks',
        output: 'String',
        tooltip: '把字典编码成 a=1&b=2 这样的查询字符串（键值都会做百分号编码，不带开头的 ?）。',
        isReporter: true,
    },
    (block, generator) => {
        const dict = generator.valueToCode(block, 'DICT', Order.NONE) || '{}';
        return [`new URLSearchParams(${dict}).toString()`, Order.FUNCTION_CALL];
    },
);
