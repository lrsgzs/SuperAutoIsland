using System.Text.Json;
using ClassIsland.Shared;
using Microsoft.ClearScript.JavaScript;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Services.BlocklyRunner;

/// <summary>
/// js 运行时命名空间
/// </summary>
public static class JavaScriptNamespace
{
    private static readonly Logger Logger = new(typeof(JavaScriptNamespace).ToString());

    /// <summary>
    /// 假的 console object
    /// </summary>
    public static readonly DummyConsole Console = new();
    
    /// <summary>
    /// 内部的 CallAction 实现
    /// </summary>
    private static async Task _callAction(string id, object data)
    {
        var dataJson = JsonSerializer.Serialize(data);
        var jsonDocument = JsonDocument.Parse(dataJson);
        Logger.BaseLog("TRACE", $"Calling Action: {id} {dataJson}");
        
        var runnerService = IAppHost.GetService<ActionAndRuleRunner>();
        await runnerService.RunAction(id, jsonDocument.RootElement);
    }
    
    /// <summary>
    /// 内部的 GetRuleState 实现
    /// </summary>
    private static async Task<bool> _getRuleState(string id, object data)
    {
        var dataJson = JsonSerializer.Serialize(data);
        var jsonDocument = JsonDocument.Parse(dataJson);
        Logger.BaseLog("TRACE", $"Getting Rule State: {id} {dataJson}");

        var runnerService = IAppHost.GetService<ActionAndRuleRunner>();
        return runnerService.RunRule(id, jsonDocument.RootElement);
    }

    /// <summary>
    /// 运行行动
    /// </summary>
    /// <param name="id">行动 id</param>
    /// <param name="data">行动 settings</param>
    /// <returns>Promise</returns>
    public static object CallAction(string id, object data)
    {
        Logger.BaseLog("TRACE", "收到 CallAction");
        return _callAction(id, data).ToPromise();
    }
    
    /// <summary>
    /// 获取规则状态
    /// </summary>
    /// <param name="id">规则 id</param>
    /// <param name="data">规则 settings</param>
    /// <returns>Promise&lt;bool&gt;</returns>
    public static object GetRuleState(string id, object data)
    {
        Logger.BaseLog("TRACE", "收到 GetRuleState");
        return _getRuleState(id, data).ToPromise();
    }

    /// <summary>
    /// 假的 console object
    /// </summary>
    public class DummyConsole
    {
        // 忽略方法名。
        
        public void log(params object[] message)
        {
            Logger.Log(message.Aggregate("", (current, obj) => current + obj + " "));
        }
        
        public void info(params object[] message)
        {
            Logger.Info(message.Aggregate("", (current, obj) => current + obj + " "));
        }
        
        public void warn(params object[] message)
        {
            Logger.Warn(message.Aggregate("", (current, obj) => current + obj + " "));
        }
        
        public void error(params object[] message)
        {
            Logger.Error(message.Aggregate("", (current, obj) => current + obj + " "));
        }
        
        public void debug(params object[] message)
        {
            Logger.Debug(message.Aggregate("", (current, obj) => current + obj + " "));
        }
    }
}