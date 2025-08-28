using Microsoft.ClearScript.V8;
using SuperAutoIsland.Interface.Enums;
using SuperAutoIsland.Models;
using SuperAutoIsland.Shared.Logger;
using V8Extended;

namespace SuperAutoIsland.Services.BlocklyRunner;

public class BlocklyRunner
{
    private readonly Logger<BlocklyRunner> _logger = new();
    private readonly V8ScriptEngine _engine = new();
    private readonly Intervals _v8Intervals = new();

    public BlocklyRunner()
    {
        _engine.AddHostObject("logger", _logger);
        _engine.AddHostObject("callAction", JavaScriptNamespace.CallAction);
        _engine.AddHostObject("getRuleState", JavaScriptNamespace.GetRuleState);
        _engine.AddHostObject("console", JavaScriptNamespace.Console);

        _v8Intervals.Extend(_engine);
        _v8Intervals.StartEventsLoopBackground();
    }

    ~BlocklyRunner()
    {
        _v8Intervals.StopEventsLoop();
    }
    
    public void RunJavaScript(string script)
    {
        _logger.Log("开始运行 JavaScript 脚本");
        _logger.Debug(script);
        
        _engine.Execute(script);
    }

    public void RunProject(Project project)
    {
        if (project.Type == ProjectsType.BlocklyAction)
        {
            var script = ProjectsConfigManager.LoadProjectJs(project);
            RunJavaScript(script);
            return;
        }

        throw new NotSupportedException();
    }
}