using Jint.WebApi;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Services.BlocklyRunner;

/// <summary>
///     脚本 <c>console</c> 的输出出口：把 Jint 的 console 记录转成插件日志等级，
///     同时写进插件日志和调用方的回流口。
/// </summary>
/// <param name="logSink">
///     日志回流口，见 <see cref="BlocklyRunner.RunJavaScript" />：参数依次是日志等级和内容。
/// </param>
public sealed class ScriptConsoleSink(Action<string, string>? logSink = null) : ConsoleSink
{
    private readonly Logger _logger = new("ScriptConsole");

    /// <inheritdoc />
    public override void Write(ConsoleLogLevel level, string message)
    {
        Emit(level, message);
    }

    /// <inheritdoc />
    public override void Write(in ConsoleRecord record)
    {
        // console.groupEnd()、console.time() 这类控制方法没有正文，别往运行输出里塞空行
        if (!string.IsNullOrEmpty(record.Message))
        {
            Emit(record.Level, record.Message);
        }
    }

    /// <summary>
    ///     按插件日志的等级词表转发一条输出。
    /// </summary>
    private void Emit(ConsoleLogLevel level, string message)
    {
        var mapped = MapLevel(level);
        _logger.BaseLog(mapped, message);
        logSink?.Invoke(mapped, message);
    }

    /// <summary>
    ///     把 Jint 的日志等级映射成插件日志的等级（和原先假 console 用的词表一致）。
    /// </summary>
    private static string MapLevel(ConsoleLogLevel level)
    {
        return level switch
        {
            ConsoleLogLevel.Debug => "DEBUG",
            ConsoleLogLevel.Warn => "WARN",
            ConsoleLogLevel.Error => "ERROR",
            _ => "INFO" // Log / Info
        };
    }
}
