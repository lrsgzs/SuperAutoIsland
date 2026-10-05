using System.Text.Json;
using ClassIsland.Shared;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Services.BlocklyRunner;

/// <summary>
///     js 运行时命名空间
/// </summary>
public class JavaScriptNamespace
{
    /// <summary>
    ///     假的 console object
    /// </summary>
    public readonly DummyConsole Console;

    private readonly Logger<JavaScriptNamespace> _logger = new();

    /// <summary>
    ///     构造函数
    /// </summary>
    /// <param name="logSink">
    ///     日志回流口：把脚本里 console.* 的输出同时交给调用方（例如把运行日志推给编辑器）。
    ///     参数依次是日志等级和内容。
    /// </param>
    public JavaScriptNamespace(Action<string, string>? logSink = null)
    {
        Console = new DummyConsole(logSink);
    }

    /// <summary>
    ///     内部的 CallAction 实现
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
    ///     内部的 GetRuleState 实现
    /// </summary>
    private async Task<bool> _getRuleState(string id, object data)
    {
        var dataJson = JsonSerializer.Serialize(data);
        var jsonDocument = JsonDocument.Parse(dataJson);
        _logger.BaseLog("TRACE", $"Getting Rule State: {id} {dataJson}");

        var runnerService = IAppHost.GetService<SaiBlockRunner>();
        var result = await runnerService.RunRule(id, jsonDocument.RootElement);
        return result;
    }

    /// <summary>
    ///     内部的 GetData 实现
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
    ///     运行行动
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
    ///     获取规则状态
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
    ///     获取规则状态
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
    ///     假的 console object
    /// </summary>
    public class DummyConsole(Action<string, string>? logSink = null)
    {
        private readonly Logger _logger = new("DummyConsole");

        // 忽略方法名。

        public void log(params object[] message)
        {
            Write("INFO", message);
        }

        public void info(params object[] message)
        {
            Write("INFO", message);
        }

        public void warn(params object[] message)
        {
            Write("WARN", message);
        }

        public void error(params object[] message)
        {
            Write("ERROR", message);
        }

        public void debug(params object[] message)
        {
            Write("DEBUG", message);
        }

        /// <summary>
        ///     写日志：既进插件日志，也（有回流口时）交给调用方
        /// </summary>
        /// <param name="level">日志等级</param>
        /// <param name="message">日志内容</param>
        private void Write(string level, object[] message)
        {
            var text = message.Aggregate("", (current, obj) => current + obj + " ");
            _logger.BaseLog(level, text);
            logSink?.Invoke(level, text);
        }
    }
}
