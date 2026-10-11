import {addBlock, addLabel} from '../utils/blockGenerator';
import {Order} from 'blockly/javascript';

/**
 * 网络请求积木。
 *
 * 生成的都是普通 JS，最终跑在**后端 Jint 的 fetch** 上（不受浏览器 / WebView 的 CORS 限制）。
 * 三种对象类型都是普通 JS 对象：
 *
 * - `SAI_FetchBody`    `{ content, contentType }`：请求正文，`content == null` 表示不带正文
 * - `SAI_FetchOptions` `{ timeout, userAgent }`：请求参数，`null` / 空串表示用运行时默认值
 * - `SAI_FetchData`    `{ ok, status, statusText, headers, content, contentType }`：请求结果
 *
 * 运行时默认值由后端 `BlocklyRunner` 配置：响应体上限 8 MiB、UserAgent 为桌面 Edge UA；
 * 超时与 UA 都可以被「请求参数」积木逐请求覆盖。
 */

/** 「默认参数」的超时时间（秒）。请求参数里 timeout <= 0 表示不限时。 */
const DEFAULT_TIMEOUT_SECONDS = 30;

/**
 * 请求实现，作为公共函数生成到脚本里（provideFunction_）。
 *
 * 注意：这段代码的运行时行为是用一个 Jint 探针工程验证过的（本地 HTTP 服务 + 与
 * `BlocklyRunner` 相同的引擎配置），改动后要重新验证：GET/POST 正文、响应头、逐请求
 * User-Agent、超时中断、8 MiB 响应上限。
 */
const requestHelperSource = (placeholder: string) => `async function ${placeholder}(method, url, body, options) {
    const settings = options || {};
    const timeoutSeconds = settings.timeout == null ? ${DEFAULT_TIMEOUT_SECONDS} : settings.timeout;
    const controller = new AbortController();
    const timer = timeoutSeconds > 0 ? setTimeout(() => controller.abort(), timeoutSeconds * 1000) : null;

    const headers = {};
    if (body && body.contentType) headers['Content-Type'] = body.contentType;
    if (settings.userAgent) headers['User-Agent'] = settings.userAgent;
    const hasBody = !!body && body.content != null && method !== 'GET' && method !== 'HEAD';

    try {
        const response = await fetch(url, {
            method: method,
            headers: headers,
            body: hasBody ? body.content : undefined,
            signal: controller.signal,
        });
        const content = await response.text();
        const responseHeaders = {};
        response.headers.forEach((value, key) => {
            responseHeaders[key] = value;
        });
        return {
            ok: response.ok,
            status: response.status,
            statusText: response.statusText,
            headers: responseHeaders,
            content: content,
            contentType: response.headers.get('content-type'),
        };
    } finally {
        if (timer !== null) clearTimeout(timer);
    }
}`;

addBlock(
    {
        type: 'network_fetch',
        message: '发送网络请求 %1 %2 正文 %3 请求参数 %4',
        inputs: {
            METHOD: {
                type: 'field_dropdown',
                data: {
                    options: [
                        ['GET', 'GET'],
                        ['POST', 'POST'],
                        ['PUT', 'PUT'],
                        ['PATCH', 'PATCH'],
                        ['DELETE', 'DELETE'],
                        ['HEAD', 'HEAD'],
                    ],
                },
            },
            URL: {
                type: 'input_value',
                blockType: 'text',
                check: 'String',
                fields: {
                    TEXT: 'https://example.com/',
                },
            },
            BODY: {
                type: 'input_value',
                check: 'SAI_FetchBody',
                blockType: 'network_fetch_body_empty',
            },
            OPTIONS: {
                type: 'input_value',
                check: 'SAI_FetchOptions',
                blockType: 'network_fetch_options_default',
            },
        },
        inline: false,
        style: 'network_blocks',
        output: 'SAI_FetchData',
        tooltip:
            '发送一个 HTTP 请求，返回请求结果（对象）。\n' +
            '请求由后端 Jint 发出，不受浏览器 CORS 限制；响应体上限 8 MiB，默认 UserAgent 为桌面 Edge UA。\n' +
            '非 2xx 不会报错，用「请求成功?」「请求状态码」这些请求结果积木自行判断。',
        isReporter: true,
    },
    (block, generator) => {
        const method = block.getFieldValue('METHOD') || 'GET';
        const url = generator.valueToCode(block, 'URL', Order.NONE) || "''";
        const body = generator.valueToCode(block, 'BODY', Order.NONE) || 'null';
        const options = generator.valueToCode(block, 'OPTIONS', Order.NONE) || 'null';

        const helper = generator.provideFunction_(
            'sai_http_request',
            requestHelperSource(generator.FUNCTION_NAME_PLACEHOLDER_),
        );

        return [`await ${helper}(${JSON.stringify(method)}, ${url}, ${body}, ${options})`, Order.MEMBER];
    },
);

addLabel('请求正文');

addBlock(
    {
        type: 'network_fetch_body_empty',
        message: '空正文',
        inputs: {},
        inline: true,
        output: 'SAI_FetchBody',
        style: 'network_blocks',
        tooltip: '不带请求正文（GET 等请求的默认写法）。',
        isReporter: true,
    },
    () => ['({ content: null, contentType: null })', Order.ATOMIC],
);

addBlock(
    {
        type: 'network_fetch_body_form',
        message: '表单正文 %1',
        inputs: {
            DATA: {
                type: 'input_value',
                check: 'Dictionary',
            },
        },
        inline: false,
        output: 'SAI_FetchBody',
        style: 'network_blocks',
        tooltip: '把字典编码成 application/x-www-form-urlencoded 正文（键值都会做百分号编码）。',
        isReporter: true,
    },
    (block, generator) => {
        const data = generator.valueToCode(block, 'DATA', Order.NONE) || '{}';
        return [
            `({ content: new URLSearchParams(${data}).toString(), contentType: 'application/x-www-form-urlencoded' })`,
            Order.ATOMIC,
        ];
    },
);

addBlock(
    {
        type: 'network_fetch_body_json',
        message: 'JSON 正文 %1',
        inputs: {
            DATA: {
                type: 'input_value',
                check: 'Dictionary',
            },
        },
        inline: false,
        output: 'SAI_FetchBody',
        style: 'network_blocks',
        tooltip: '把字典序列化成 application/json 正文。',
        isReporter: true,
    },
    (block, generator) => {
        const data = generator.valueToCode(block, 'DATA', Order.NONE) || '{}';
        return [`({ content: JSON.stringify(${data}), contentType: 'application/json' })`, Order.ATOMIC];
    },
);

addBlock(
    {
        type: 'network_fetch_body_custom',
        message: '自定义正文 类型 %1 内容 %2',
        inputs: {
            CONTENT_TYPE: {
                type: 'input_value',
                blockType: 'text',
                check: 'String',
                fields: {
                    TEXT: 'application/json',
                },
            },
            DATA: {
                type: 'input_value',
                blockType: 'text',
                check: 'String',
                fields: {
                    TEXT: '',
                },
            },
        },
        inline: false,
        output: 'SAI_FetchBody',
        style: 'network_blocks',
        tooltip: '直接指定 Content-Type 与正文内容（正文不会做任何编码，自己写好）。',
        isReporter: true,
    },
    (block, generator) => {
        const contentType = generator.valueToCode(block, 'CONTENT_TYPE', Order.NONE) || "'application/json'";
        const content = generator.valueToCode(block, 'DATA', Order.NONE) || "''";
        return [`({ content: ${content}, contentType: ${contentType} })`, Order.ATOMIC];
    },
);

addLabel('请求参数');

addBlock(
    {
        type: 'network_fetch_options_default',
        message: '默认参数',
        inputs: {},
        inline: true,
        output: 'SAI_FetchOptions',
        style: 'network_blocks',
        tooltip: '用运行时默认值：超时 30 秒，UserAgent 为内置的桌面 Edge UA。',
        isReporter: true,
    },
    () => ['({ timeout: null, userAgent: null })', Order.ATOMIC],
);

addBlock(
    {
        type: 'network_fetch_options',
        message: '参数 超时(s) %1 UserAgent %2',
        inputs: {
            TIMEOUT: {
                type: 'input_value',
                blockType: 'math_number',
                check: 'Number',
                fields: {
                    NUM: DEFAULT_TIMEOUT_SECONDS,
                },
            },
            USER_AGENT: {
                type: 'input_value',
                blockType: 'text',
                check: 'String',
                fields: {
                    TEXT: '',
                },
            },
        },
        inline: false,
        output: 'SAI_FetchOptions',
        style: 'network_blocks',
        tooltip:
            '自定义请求参数：超时时间（秒，<= 0 表示不限时）、UserAgent（留空表示用内置默认 UA）。\n' +
            '请求由后端发出，逐请求的 UserAgent 会覆盖全局默认值。',
        isReporter: true,
    },
    (block, generator) => {
        const timeout = generator.valueToCode(block, 'TIMEOUT', Order.NONE) || 'null';
        const userAgent = generator.valueToCode(block, 'USER_AGENT', Order.NONE) || "''";
        return [`({ timeout: ${timeout}, userAgent: ${userAgent} })`, Order.ATOMIC];
    },
);

addLabel('请求结果');

/**
 * 生成一个「从请求结果里取某个字段」的积木。
 *
 * @param type 积木类型
 * @param message 积木文案（`%1` 是请求结果的输入）
 * @param output 输出类型（Boolean / Number / String / Dictionary），用于 Blockly 类型检查
 * @param member `SAI_FetchData` 里的字段名
 * @param tooltip 提示
 */
function addResultBlock(type: string, message: string, output: string, member: string, tooltip: string) {
    addBlock(
        {
            type: type,
            message: message,
            inputs: {
                DATA: {
                    type: 'input_value',
                    check: 'SAI_FetchData',
                },
            },
            inline: false,
            style: 'network_blocks',
            output: output,
            tooltip: tooltip,
            isReporter: true,
        },
        (block, generator) => {
            const data = generator.valueToCode(block, 'DATA', Order.NONE) || 'null';
            return [`(${data}).${member}`, Order.MEMBER];
        },
    );
}

addResultBlock('network_result_ok', '请求成功? %1', 'Boolean', 'ok', '请求是否成功（HTTP 状态码在 200–299 之间）。');

addResultBlock('network_result_status', '请求状态码 %1', 'Number', 'status', 'HTTP 状态码（数字，比如 200、404）。');

addResultBlock(
    'network_result_status_text',
    '请求状态文本 %1',
    'String',
    'statusText',
    'HTTP 状态文本（比如 OK、Not Found）。',
);

addResultBlock(
    'network_result_headers',
    '请求响应头 %1',
    'Dictionary',
    'headers',
    '响应头字典（键都是小写），可以接「获取字典中指定键的值」等字典积木。',
);

addResultBlock('network_result_content', '请求正文 %1', 'String', 'content', '响应正文文本（按 UTF-8 解码）。');

addResultBlock(
    'network_result_content_type',
    '请求正文类型 %1',
    'String',
    'contentType',
    '响应头里的 Content-Type；服务端没给这个头时是 null。',
);
