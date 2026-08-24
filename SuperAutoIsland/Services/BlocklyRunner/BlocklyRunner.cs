using Jint;
using SuperAutoIsland.Enums;
using SuperAutoIsland.Models;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Services.BlocklyRunner;

/// <summary>
/// Blockly 项目运行器
/// </summary>
public class BlocklyRunner
{
    private readonly Logger<BlocklyRunner> _logger = new();
    private Engine? _engine;
    private JavaScriptNamespace? _jsNamespace;

    /// <summary>
    /// 运行 js 脚本
    /// </summary>
    /// <param name="script">脚本代码</param>
    /// <param name="cancellationToken">中断 token</param>
    public async Task RunJavaScript(string script, CancellationToken cancellationToken = default)
    {
        if (_engine == null)
        {
            _engine = new Engine(options =>
            {
                options.Constraints.PromiseTimeout = TimeSpan.Zero;
            });
            _jsNamespace = new JavaScriptNamespace();
            
            _engine.SetValue("logger", _logger);
            _engine.SetValue("console", _jsNamespace.Console);
            _engine.SetValue("callAction", _jsNamespace.CallAction);
            _engine.SetValue("getRuleState", _jsNamespace.GetRuleState);
            _engine.SetValue("getData", _jsNamespace.GetData);
        }
        
        _logger.Log("开始运行 JavaScript 脚本");
        _logger.Debug(script);
        
        await _engine.EvaluateAsync(script, "main.js", cancellationToken);
    }

    /// <summary>
    /// 运行项目
    /// </summary>
    /// <param name="project">项目实例</param>
    /// <param name="cancellationToken">中断 token</param>
    /// <exception cref="NotSupportedException">遇到不支持的项目会报这个错误</exception>
    public async Task RunActionProject(Project project, CancellationToken cancellationToken = default)
    {
        if (project.Type != ProjectsType.BlocklyAction)
            throw new NotSupportedException();
        
        var script = ProjectsConfigManager.LoadBlocklyProjectJs(project);
        await RunJavaScript(script, cancellationToken);
    }
}