using Microsoft.ClearScript.V8;
using SuperAutoIsland.Enums;
using SuperAutoIsland.Models;
using SuperAutoIsland.Shared;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Services.BlocklyRunner;

/// <summary>
/// Blockly 项目运行器
/// </summary>
public class BlocklyRunner
{
    private readonly Logger<BlocklyRunner> _logger = new();
    private V8ScriptEngine? _engine;
    
    /// <summary>
    /// 运行 js 脚本
    /// </summary>
    /// <param name="script">脚本代码</param>
    public async Task RunJavaScript(string script)
    {
        await V8Loader.InitializationTask;
        
        if (V8Loader.IsV8Denied)
        {
            _logger.Warn("V8 引擎未就绪");
            return;
        }

        if (_engine == null)
        {
            _engine = new V8ScriptEngine();
            _engine.AddHostObject("logger", _logger);
            _engine.AddHostObject("callAction", JavaScriptNamespace.CallAction);
            _engine.AddHostObject("getRuleState", JavaScriptNamespace.GetRuleState);
            _engine.AddHostObject("getData", JavaScriptNamespace.GetData);
            _engine.AddHostObject("console", JavaScriptNamespace.Console);
        }
        
        _logger.Log("开始运行 JavaScript 脚本");
        _logger.Debug(script);
        
        _engine.Execute(script);
    }

    /// <summary>
    /// 运行项目
    /// </summary>
    /// <param name="project">项目实例</param>
    /// <exception cref="NotSupportedException">遇到不支持的项目会报这个错误</exception>
    public async Task RunActionProject(Project project)
    {
        if (project.Type == ProjectsType.BlocklyAction)
        {
            var script = ProjectsConfigManager.LoadBlocklyProjectJs(project);
            await RunJavaScript(script);
            return;
        }

        throw new NotSupportedException();
    }
}