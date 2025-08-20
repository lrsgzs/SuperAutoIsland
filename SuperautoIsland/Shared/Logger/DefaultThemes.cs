namespace SuperAutoIsland.Shared.Logger;

public static class DefaultThemes
{
    public static readonly Theme Default = new Theme(new Dictionary<string, ConsoleColor>
    {
        { "INFO", ConsoleColor.Cyan },
        { "WARN", ConsoleColor.Yellow },
        { "ERROR", ConsoleColor.Red },
        { "DEBUG", ConsoleColor.Blue }
    });
    
    public static readonly Theme Origin = new Theme(new Dictionary<string, ConsoleColor>
    {
        { "INFO", ConsoleColor.White },
        { "WARN", ConsoleColor.Yellow },
        { "ERROR", ConsoleColor.Red },
        { "DEBUG", ConsoleColor.DarkGray }
    });
}