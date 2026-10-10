using Jint;
using Jint.Runtime;

namespace SuperAutoIsland.Services;

/// <summary>
///     JavaScript 格式化器，使用 Jint 跑 Prettier。
/// </summary>
/// <param name="assetFolder">存放 prettier bundle 的目录</param>
public sealed class PrettierFormatter(string assetFolder)
{
    private const int MaxStatements = 100_000_000;
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(20);

    private readonly Lazy<Bundles> _bundles = new(() => new Bundles(
        File.ReadAllText(Path.Combine(assetFolder, "prettier-standalone.js")),
        File.ReadAllText(Path.Combine(assetFolder, "prettier-estree.js")),
        File.ReadAllText(Path.Combine(assetFolder, "prettier-babel.js"))));

    /// <summary>
    ///     格式化一段 JavaScript。
    /// </summary>
    /// <param name="source">脚本内容</param>
    /// <param name="cancellationToken">取消 token</param>
    /// <returns>格式化后的脚本</returns>
    /// <exception cref="FormatException">
    ///     脚本有语法错误（消息里带 prettier 的行列与代码片段）、超时，或 prettier 资源读不到。
    /// </exception>
    public Task<string> FormatAsync(string source, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => Format(source, cancellationToken), cancellationToken);
    }

    private string Format(string source, CancellationToken cancellationToken)
    {
        Bundles bundles;
        try
        {
            bundles = _bundles.Value;
        }
        catch (Exception exception)
        {
            throw new FormatException($"读取 prettier 资源失败，检查插件目录下的 {assetFolder}。", exception);
        }

        var engine = new Engine(options =>
        {
            options.Strict = false;
            options.LimitStatements(MaxStatements);
            options.LimitExecutionTime(Timeout);
            options.Constraints.PromiseTimeout = Timeout;
            options.Constraints.StackOverflowGuard = true;
            options.ObserveCancellation(cancellationToken);
        });

        LoadModule(engine, bundles.Standalone, "__prettier");
        LoadModule(engine, bundles.Estree, "__prettierEstree");
        LoadModule(engine, bundles.Babel, "__prettierBabel");

        // 选项与前端 src/blockly/index.ts 里 generateCode 的那份保持一致
        engine.Execute("""
            var __format = async function (source) {
                return await __prettier.format(source, {
                    semi: true,
                    singleQuote: true,
                    trailingComma: 'all',
                    parser: 'babel',
                    plugins: [__prettierEstree, __prettierBabel],
                });
            };
            """);

        engine.SetValue("__source", source);
        try
        {
            return engine.Evaluate("__format(__source)").UnwrapIfPromise().AsString();
        }
        catch (PromiseRejectedException exception)
        {
            // prettier 的语法错误：RejectedValue 就是那个 JS SyntaxError，ToString() 带行列和出错处的代码片段
            throw new FormatException(exception.RejectedValue?.ToString() ?? exception.Message, exception);
        }
        catch (TimeoutException exception)
        {
            throw new FormatException($"格式化超时（超过 {Timeout.TotalSeconds:0} 秒），脚本可能太大了。", exception);
        }
    }

    /// <summary>
    ///     按 CommonJS 的样子在引擎里执行一个 bundle，把 <c>module.exports</c> 挂到全局变量上。
    /// </summary>
    private static void LoadModule(Engine engine, string source, string globalName)
    {
        engine.Execute("var module = { exports: {} }; var exports = module.exports;");
        engine.Execute(source);
        engine.SetValue(globalName, engine.Evaluate("module.exports"));
    }

    /// <summary>
    ///     prettier 的三个 bundle。
    /// </summary>
    private sealed record Bundles(string Standalone, string Estree, string Babel);
}
