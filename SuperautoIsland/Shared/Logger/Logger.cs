namespace SuperAutoIsland.Shared.Logger;

public class Logger(string name, bool showTime = true, Theme? theme = null)
{
    public static readonly RootLogger Root = new();
    
    public readonly string Name = name;
    public bool ShowTime = showTime;
    public Theme Theme = theme ?? DefaultThemes.Default;

    public void Config(bool? showTime = null, Theme? theme = null)
    {
        ShowTime = showTime ?? ShowTime;
        Theme = theme ?? Theme;
    }

    public Logger Sub(string subName)
    {
        var newLogger = new Logger($"{Name} -> {subName}");
        newLogger.Config(ShowTime, Theme);
        return newLogger;
    }

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

    public void FormatException(Exception exception)
    {
        var exceptionType = exception.GetType().ToString();
        Error($"{exceptionType}: {exception.Message}\nStackTrace:\n{exception.StackTrace}\nHelpLink: {exception.HelpLink}");
    }
}

public class Logger<TObject>(bool showTime = true, Theme? theme = null) : Logger(typeof(TObject).ToString(), showTime, theme);
