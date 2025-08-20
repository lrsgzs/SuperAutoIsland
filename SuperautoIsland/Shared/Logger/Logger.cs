namespace SuperAutoIsland.Shared.Logger;

public class Logger(string name, bool showTime = true, Theme? theme = null)
{
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
            .Select(e => e.ToString())
            .SelectMany(e => (e ?? string.Empty).Split("\n"));

        foreach (var message in messageList)
        {
            var originColor = Console.ForegroundColor;
            Console.ForegroundColor = Theme[level];
            
            Console.Write($"[{Name}] ");
            if (ShowTime)
            {
                Console.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ");
            }
            Console.WriteLine($"[{level}] {message}");
            
            Console.ForegroundColor = originColor;
        }
    }
    
    public void Log(params object[] message) => BaseLog("INFO", message);
    
    public void Info(params object[] message) => BaseLog("INFO", message);

    public void Warn(params object[] message) => BaseLog("WARN", message);

    public void Warning(params object[] message) => BaseLog("WARN", message);
    
    public void Error(params object[] message) => BaseLog("ERROR", message);
    
    public void Debug(params object[] message) => BaseLog("DEBUG", message);
    
    public void FormatException() => Error(Environment.StackTrace);
}