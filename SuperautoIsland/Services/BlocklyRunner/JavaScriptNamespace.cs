using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using ClassIsland.Shared;
using Microsoft.ClearScript.JavaScript;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Services.BlocklyRunner;

public static class JavaScriptNamespace
{
    private static readonly Logger Logger = new(typeof(JavaScriptNamespace).ToString());

    public static readonly DummyConsole Console = new();
    
    public static async Task _callAction(string id, object data)
    {
        var dataJson = JsonSerializer.Serialize(data);
        var jsonDocument = JsonDocument.Parse(dataJson);
        Logger.Debug($"Calling Action: {id} {dataJson}");
        
        var runnerService = IAppHost.GetService<ActionAndRuleRunner>();
        await runnerService.RunAction(id, jsonDocument.RootElement);
    }

    public static async Task<bool> _getRuleState(string id, object data)
    {
        var dataJson = JsonSerializer.Serialize(data);
        var jsonDocument = JsonDocument.Parse(dataJson);
        Logger.Debug($"Getting Rule State: {id} {dataJson}");

        var runnerService = IAppHost.GetService<ActionAndRuleRunner>();
        return runnerService.RunRule(id, jsonDocument.RootElement);
    }

    public static object CallAction(string id, object data)
    {
        Logger.BaseLog("TRACE", "收到 CallAction");
        return _callAction(id, data).ToPromise();
    }
    
    public static object GetRuleState(string id, object data)
    {
        Logger.BaseLog("TRACE", "收到 GetRuleState");
        return _getRuleState(id, data).ToPromise();
    }

    // 忽略方法名。
    public class DummyConsole
    {
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