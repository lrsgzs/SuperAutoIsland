using Jint;
using SuperAutoIsland.Enums;
using SuperAutoIsland.Models;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Services.BlocklyRunner;

/// <summary>
///     Blockly 项目运行器
/// </summary>
/// <remarks>
///     运行策略：
///     <list type="bullet">
///         <item>
///             每次运行新建一个 <see cref="Engine" />。Jint 的引擎不是线程安全的（官方原话：one engine, one
///             thread），而运行会被「编辑器点运行」「自动化触发」「设置页运行」并发发起，所以不做引擎复用。
///         </item>
///         <item>脚本一律丢到线程池执行，不在调用线程（可能是 UI 线程）上跑。</item>
///     </list>
/// </remarks>
public class BlocklyRunner
{
    /// <summary>
    ///     单次运行的语句预算，用来兜住 <c>while (true) { }</c> 之类的死循环。
    /// </summary>
    public const int DefaultMaxStatements = 10_000_000;

    /// <summary>
    ///     单次运行的最大递归深度。
    /// </summary>
    public const int DefaultMaxRecursionDepth = 256;

    private readonly Logger<BlocklyRunner> _logger = new();

    /// <summary>
    ///     运行 js 脚本
    /// </summary>
    /// <param name="script">脚本代码</param>
    /// <param name="cancellationToken">中断 token</param>
    public async Task RunJavaScript(string script, CancellationToken cancellationToken = default)
    {
        _logger.Log("开始运行 JavaScript 脚本");
        _logger.Debug(script);

        await Task.Run(async () =>
        {
            using var engine = CreateEngine(cancellationToken);
            await engine.EvaluateAsync(script, "main.js", cancellationToken);
        }, cancellationToken);

        _logger.Log("JavaScript 脚本运行完毕");
    }

    /// <summary>
    ///     运行项目
    /// </summary>
    /// <param name="project">项目实例</param>
    /// <param name="cancellationToken">中断 token</param>
    /// <exception cref="NotSupportedException">遇到不支持的项目会报这个错误</exception>
    public async Task RunActionProject(Project project, CancellationToken cancellationToken = default)
    {
        if (project.Type != ProjectsType.BlocklyAction)
            throw new NotSupportedException();

        _logger.Info($"正在运行 Blockly 项目 {project.Name}");
        var script = ProjectsConfigManager.LoadBlocklyProjectJs(project);
        await RunJavaScript(script, cancellationToken);
    }

    /// <summary>
    ///     创建一个配置好的引擎
    /// </summary>
    /// <param name="cancellationToken">中断 token</param>
    private Engine CreateEngine(CancellationToken cancellationToken)
    {
        var engine = new Engine(options =>
        {
            options.Constraints.PromiseTimeout = TimeSpan.Zero;
            options.MaxStatements(DefaultMaxStatements);
            options.LimitRecursion(DefaultMaxRecursionDepth);
            options.Constraints.StackOverflowGuard = true;
            options.CancellationToken(cancellationToken);
        });

        var jsNamespace = new JavaScriptNamespace();
        engine.SetValue("logger", _logger);
        engine.SetValue("console", jsNamespace.Console);
        engine.SetValue("callAction", jsNamespace.CallAction);
        engine.SetValue("getRuleState", jsNamespace.GetRuleState);
        engine.SetValue("getData", jsNamespace.GetData);
        return engine;
    }
}
