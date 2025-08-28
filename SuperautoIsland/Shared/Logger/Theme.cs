namespace SuperAutoIsland.Shared.Logger;

public class Theme
{
    private readonly Dictionary<string, ConsoleColor> _themeData;
    
    public Theme()
    {
        _themeData = new Dictionary<string, ConsoleColor>();
    }

    public Theme(Dictionary<string, ConsoleColor> origin)
    {
        _themeData = new Dictionary<string, ConsoleColor>(origin);
    }
    
    public Theme(Theme origin)
    {
        _themeData = new Dictionary<string, ConsoleColor>(origin._themeData);
    }

    public void SetTheme(Dictionary<string, ConsoleColor> themeData)
    {
        foreach (var kv in themeData)
        {
            _themeData[kv.Key] = kv.Value;
        }
    }
    
    public void SetTheme(string themeKey, ConsoleColor color)
    {
        _themeData[themeKey] = color;
    }

    public ConsoleColor GetTheme(string themeKey)
    {
        return _themeData.GetValueOrDefault(themeKey, ConsoleColor.Gray);
    }
    
    public ConsoleColor this[string themeKey] => GetTheme(themeKey);
}