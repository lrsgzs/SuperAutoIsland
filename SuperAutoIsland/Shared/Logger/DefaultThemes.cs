namespace SuperAutoIsland.Shared.Logger;

/// <summary>
/// 默认主题
/// </summary>
public static class DefaultThemes
{
    /// <summary>
    /// 默认
    /// </summary>
    public static readonly Theme Default = new Theme(new Dictionary<string, ConsoleColor>
    {
        { "INFO", ConsoleColor.Cyan },
        { "WARN", ConsoleColor.Yellow },
        { "ERROR", ConsoleColor.Red },
        { "DEBUG", ConsoleColor.Blue }
    });
    
    /// <summary>
    /// 原子
    /// </summary>
    public static readonly Theme Origin = new Theme(new Dictionary<string, ConsoleColor>
    {
        { "INFO", ConsoleColor.White },
        { "WARN", ConsoleColor.Yellow },
        { "ERROR", ConsoleColor.Red },
        { "DEBUG", ConsoleColor.DarkGray }
    });
}