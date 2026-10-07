import { addBlock } from '../utils/blockGenerator';
import { Order } from 'blockly/javascript';

/**
 * CSV 解析积木。
 *
 * 解析逻辑全部作为 JS 代码生成（脚本最终跑在后端 Jint 上），不经过 C# 后端接口。
 *
 * 规则（大致按 RFC 4180，但不做严格校验）：
 * - 返回「列表的列表」：外层每个元素是一行，内层每个元素是一个单元格（都是文本）
 * - 支持 "引号包裹的单元格"（含分隔符、换行），"" 表示一个转义引号
 * - 换行支持 \r\n / \n / \r，末尾的换行不会多出一行空数据
 * - 单元格不做类型转换，需要数字用「类型」分类里的转数字积木
 */
addBlock(
    {
        type: 'csv_parse',
        message: '解析 CSV %1 分隔符 %2',
        inputs: {
            TEXT: {
                type: 'input_value',
                blockType: 'text',
                fields: {
                    TEXT: 'a,b,c',
                },
            },
            DELIMITER: {
                type: 'field_dropdown',
                data: {
                    options: [
                        ['逗号 ,', ','],
                        ['制表符 Tab', '\t'],
                        ['分号 ;', ';'],
                        ['竖线 |', '|'],
                    ],
                },
            },
        },
        inline: false,
        style: 'my_blocks',
        output: 'Array',
        tooltip: '把 CSV 文本解析成列表的列表（每行一个列表，单元格都是文本）。支持引号包裹与转义引号。',
        isReporter: true,
    },
    (block, generator) => {
        const text = generator.valueToCode(block, 'TEXT', Order.NONE) || "''";
        const delimiter = block.getFieldValue('DELIMITER') || ',';

        const wrapper = generator.provideFunction_(
            'csv_parse',
            `function ${generator.FUNCTION_NAME_PLACEHOLDER_}(text, delimiter) {
                const rows = [];
                let row = [];
                let field = '';
                let quoted = false;
                const source = text == null ? '' : String(text);
                for (let i = 0; i < source.length; i++) {
                    const ch = source.charAt(i);
                    if (quoted) {
                        if (ch === '"') {
                            if (source.charAt(i + 1) === '"') {
                                field += '"';
                                i++;
                            } else {
                                quoted = false;
                            }
                        } else {
                            field += ch;
                        }
                    } else if (ch === '"' && field === '') {
                        quoted = true;
                    } else if (ch === delimiter) {
                        row.push(field);
                        field = '';
                    } else if (ch === '\\n' || ch === '\\r') {
                        if (ch === '\\r' && source.charAt(i + 1) === '\\n') i++;
                        row.push(field);
                        field = '';
                        rows.push(row);
                        row = [];
                    } else {
                        field += ch;
                    }
                }
                if (field !== '' || row.length > 0) {
                    row.push(field);
                    rows.push(row);
                }
                return rows;
            }`,
        );

        return [`${wrapper}(${text}, ${JSON.stringify(delimiter)})`, Order.FUNCTION_CALL];
    },
);
