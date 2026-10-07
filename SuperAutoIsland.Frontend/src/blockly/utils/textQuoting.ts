import type { JavascriptGenerator } from 'blockly/javascript';

/**
 * 覆盖 Blockly 的文本转义，补上它漏掉的 \r。
 *
 * 源码（blockly@13.3.0 `generators/javascript/javascript_generator.ts`）：
 *
 *     quote_(string: string): string {
 *         // Can't use goog.string.quote since Google's style guide recommends
 *         // JS string literals use single quotes.
 *         string = string
 *             .replace(/\\/g, '\\\\')
 *             .replace(/\n/g, '\\\n')
 *             .replace(/'/g, "\\'");
 *         return "'" + string + "'";
 *     }
 *
 *     multiline_quote_(string: string): string {
 *         const lines = string.split(/\n/g).map(this.quote_);
 *         return lines.join(" + '\\n' +\n");
 *     }
 *
 * 它只转义了 \ 和 \n，没管 \r：从 Excel / 记事本复制来的文本基本是 CRLF（CSV 尤其常见），
 * 粘进「文本（多行）」后生成的字符串字面量里会留下裸 \r，是非法 JS，运行直接语法错误。
 *
 * 这里照源码把 quote_ 覆盖一遍，只多一行 \r 转义。multiline_quote_ 是逐行走 quote_ 的
 * （`string.split(/\n/g).map(this.quote_)`），所以不用单独覆盖，一并修好。
 *
 * \r 输出成 \r 转义序列，而不是像 \n 那样输出成行连接符（反斜杠 + 换行），
 * 是为了让文本内容原样保留：CRLF 进去、CRLF 出来，CSV 解析照常认得出换行。
 *
 * @param generator 要打补丁的 JS 代码生成器（项目里就是 `javascriptGenerator` 单例）
 */
export function installTextQuotingFix(generator: JavascriptGenerator): void {
    generator.quote_ = (text: string): string => {
        const escaped = text.replace(/\\/g, '\\\\').replace(/\n/g, '\\\n').replace(/\r/g, '\\r').replace(/'/g, "\\'");
        return "'" + escaped + "'";
    };
}
