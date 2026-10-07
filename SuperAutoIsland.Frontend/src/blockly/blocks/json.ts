import { addBlock } from '../utils/blockGenerator';
import { Order } from 'blockly/javascript';

/**
 * JSON 解析积木。
 *
 * 直接用 JS 的 JSON.parse 生成代码（脚本最终跑在后端 Jint 上），不经过 C# 后端接口。
 * 解析结果可能是对象、列表、文本、数字等任意类型，所以 output 不限定 check。
 */
addBlock(
    {
        type: 'json_parse',
        message: '解析 JSON %1',
        inputs: {
            TEXT: {
                type: 'input_value',
                blockType: 'text',
                fields: {
                    TEXT: '{"key": "value"}',
                },
            },
        },
        inline: false,
        style: 'my_blocks',
        output: '',
        tooltip: '把 JSON 文本解析成对象 / 列表等；文本不是合法 JSON 时会报错。',
        isReporter: true,
    },
    (block, generator) => {
        const text = generator.valueToCode(block, 'TEXT', Order.NONE) || "''";
        return [`JSON.parse(${text})`, Order.FUNCTION_CALL];
    },
);
