using Microsoft.ClearScript.V8;
using SuperAutoIsland.Enums;
using SuperAutoIsland.Models;
using SuperAutoIsland.Shared.Logger;
using V8Extended;

namespace SuperAutoIsland.Services.BlocklyRunner;

/// <summary>
/// Blockly 项目运行器
/// </summary>
public class BlocklyRunner
{
    private readonly Logger<BlocklyRunner> _logger = new();
    private readonly V8ScriptEngine _engine = new();
    private readonly Intervals _v8Intervals = new();

    /// <summary>
    /// 构造函数
    /// <see cref="BlocklyRunner"/>
    /// </summary>
    public BlocklyRunner()
    {
        _engine.AddHostObject("logger", _logger);
        _engine.AddHostObject("callAction", JavaScriptNamespace.CallAction);
        _engine.AddHostObject("getRuleState", JavaScriptNamespace.GetRuleState);
        _engine.AddHostObject("console", JavaScriptNamespace.Console);

        _v8Intervals.Extend(_engine);
        _v8Intervals.StartEventsLoopBackground();
    }

    /// <summary>
    /// 销毁函数
    /// </summary>
    public void Dispose()
    {
        _v8Intervals.StopEventsLoop();
        GC.SuppressFinalize(this);
    }
    
    /// <summary>
    /// 运行 js 脚本
    /// </summary>
    /// <param name="script">脚本代码</param>
    public void RunJavaScript(string script)
    {
        _logger.Log("开始运行 JavaScript 脚本");
        _logger.Debug(script);
        
        _engine.Execute(script);
    }

    /// <summary>
    /// 运行项目
    /// </summary>
    /// <param name="project">项目实例</param>
    /// <exception cref="NotSupportedException">遇到不支持的项目会报这个错误</exception>
    public void RunActionProject(Project project)
    {
        if (project.Type == ProjectsType.BlocklyAction)
        {
            var script = ProjectsConfigManager.LoadBlocklyProjectJs(project);
            RunJavaScript(script);
            return;
        }

        throw new NotSupportedException();
    }
}