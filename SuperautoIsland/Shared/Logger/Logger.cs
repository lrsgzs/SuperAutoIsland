namespace SuperAutoIsland.Shared.Logger;

/// <summary>
/// 日志
/// </summary>
/// <param name="name">区域名称</param>
/// <param name="showTime">控制台是否显示时间</param>
/// <param name="theme">主题</param>
public class Logger(string name, bool showTime = true, Theme? theme = null)
{
    public static readonly RootLogger Root = new();
    
    public readonly string Name = name;
    public bool ShowTime = showTime;
    public Theme Theme = theme ?? DefaultThemes.Default;

    /// <summary>
    /// 基本日志函数
    /// </summary>
    /// <param name="level">日志等级</param>
    /// <param name="messages">消息</param>
    public void BaseLog(string level, params object[] messages)
    {
        level = level.ToUpper();

        var messageList = messages
            .Select(e => e.ToString() ?? "");

        foreach (var message in messageList)
        {
            var originColor = Console.ForegroundColor;
            Console.ForegroundColor = Theme[level];
            
            Console.Write($"[{Name}] ");
            if (ShowTime)
            {
                Console.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ");
            }
            Console.WriteLine($"[{level}]");
            Console.ForegroundColor = originColor;
            
            Console.WriteLine(message);
            
            // 写入根 logger
            Root.AddLog(new LogData()
            {
                Scope = Name,
                Level = level,
                Message = message,
                Time = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}"
            });
        }
    }
    
    public void Log(params object[] message) => BaseLog("INFO", message);
    
    public void Info(params object[] message) => BaseLog("INFO", message);

    public void Warn(params object[] message) => BaseLog("WARN", message);

    public void Warning(params object[] message) => BaseLog("WARN", message);
    
    public void Error(params object[] message) => BaseLog("ERROR", message);
    
    public void Debug(params object[] message) => BaseLog("DEBUG", message);

    /// <summary>
    /// 渲染 Exception
    /// </summary>
    public void FormatException(Exception exception)
    {
        var exceptionType = exception.GetType().ToString();
        Error($"{exceptionType}: {exception.Message}\nStackTrace:\n{exception.StackTrace}\nHelpLink: {exception.HelpLink}");
    }
}

/// <summary>
/// 日志
/// </summary>
/// <param name="showTime">控制台是否显示时间</param>
/// <param name="theme">主题</param>
/// <typeparam name="TObject">日志所在对象</typeparam>
public class Logger<TObject>(bool showTime = true, Theme? theme = null) : Logger(typeof(TObject).ToString(), showTime, theme);
