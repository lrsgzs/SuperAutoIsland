using System.Text.Json;
using ClassIsland.Shared;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Services.BlocklyRunner;

/// <summary>
/// js 运行时命名空间
/// </summary>
public class JavaScriptNamespace
{
    /// <summary>
    /// 假的 console object
    /// </summary>
    public readonly DummyConsole Console = new();
    private readonly Logger<JavaScriptNamespace> _logger = new();
    
    /// <summary>
    /// 内部的 CallAction 实现
    /// </summary>
    private async Task _callAction(string id, object data)
    {
        var dataJson = JsonSerializer.Serialize(data);
        var jsonDocument = JsonDocument.Parse(dataJson);
        _logger.BaseLog("TRACE", $"Calling Action: {id} {dataJson}");
        
        var runnerService = IAppHost.GetService<SaiBlockRunner>();
        await runnerService.RunAction(id, jsonDocument.RootElement);
    }
    
    /// <summary>
    /// 内部的 GetRuleState 实现
    /// </summary>
    private Task<bool> _getRuleState(string id, object data)
    {
        var dataJson = JsonSerializer.Serialize(data);
        var jsonDocument = JsonDocument.Parse(dataJson);
        _logger.BaseLog("TRACE", $"Getting Rule State: {id} {dataJson}");

        var runnerService = IAppHost.GetService<SaiBlockRunner>();
        var result = runnerService.RunRule(id, jsonDocument.RootElement);
        return Task.FromResult(result);
    }
    
    /// <summary>
    /// 内部的 GetData 实现
    /// </summary>
    private async Task<object> _getData(string id, object data)
    {
        var dataJson = JsonSerializer.Serialize(data);
        var jsonDocument = JsonDocument.Parse(dataJson);
        _logger.BaseLog("TRACE", $"Getting Data: {id} {dataJson}");

        var runnerService = IAppHost.GetService<SaiBlockRunner>();
        return await runnerService.RunData(id, jsonDocument.RootElement);
    }

    /// <summary>
    /// 运行行动
    /// </summary>
    /// <param name="id">行动 id</param>
    /// <param name="data">行动 settings</param>
    /// <returns>Promise</returns>
    public Task CallAction(string id, object data)
    {
        _logger.BaseLog("TRACE", "收到 CallAction");
        return _callAction(id, data);
    }
    
    /// <summary>
    /// 获取规则状态
    /// </summary>
    /// <param name="id">规则 id</param>
    /// <param name="data">规则 settings</param>
    /// <returns>Promise&lt;bool&gt;</returns>
    public Task<bool> GetRuleState(string id, object data)
    {
        _logger.BaseLog("TRACE", "收到 GetRuleState");
        return _getRuleState(id, data);
    }
    
    /// <summary>
    /// 获取规则状态
    /// </summary>
    /// <param name="id">规则 id</param>
    /// <param name="data">规则 settings</param>
    /// <returns>Promise&lt;object&gt;</returns>
    public Task<object> GetData(string id, object data)
    {
        _logger.BaseLog("TRACE", "收到 GetData");
        return _getData(id, data);
    }

    /// <summary>
    /// 假的 console object
    /// </summary>
    public class DummyConsole
    {
        private Logger _logger = new("DummyConsole");
        
        // 忽略方法名。
        
        public void log(params object[] message)
        {
            _logger.Log(message.Aggregate("", (current, obj) => current + obj + " "));
        }
        
        public void info(params object[] message)
        {
            _logger.Info(message.Aggregate("", (current, obj) => current + obj + " "));
        }
        
        public void warn(params object[] message)
        {
            _logger.Warn(message.Aggregate("", (current, obj) => current + obj + " "));
        }
        
        public void error(params object[] message)
        {
            _logger.Error(message.Aggregate("", (current, obj) => current + obj + " "));
        }
        
        public void debug(params object[] message)
        {
            _logger.Debug(message.Aggregate("", (current, obj) => current + obj + " "));
        }
    }
}