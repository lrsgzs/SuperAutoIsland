namespace SuperAutoIsland.Shared.Logger;

/// <summary>
/// 主题类
/// </summary>
public class Theme
{
    private readonly Dictionary<string, ConsoleColor> _themeData;
    
    /// <summary>
    /// 默认构造函数
    /// </summary>
    public Theme()
    {
        _themeData = new Dictionary<string, ConsoleColor>();
    }

    /// <summary>
    /// 字典构造函数
    /// </summary>
    /// <param name="origin">原主题字典</param>
    public Theme(Dictionary<string, ConsoleColor> origin)
    {
        _themeData = new Dictionary<string, ConsoleColor>(origin);
    }
    
    /// <summary>
    /// 主题构造函数
    /// </summary>
    /// <param name="origin">原主题实例</param>
    public Theme(Theme origin)
    {
        _themeData = new Dictionary<string, ConsoleColor>(origin._themeData);
    }

    /// <summary>
    /// 设置多个等级颜色
    /// </summary>
    /// <param name="themeData">等级-颜色 字典</param>
    public void SetTheme(Dictionary<string, ConsoleColor> themeData)
    {
        foreach (var kv in themeData)
        {
            _themeData[kv.Key] = kv.Value;
        }
    }
    
    /// <summary>
    /// 设置单一等级颜色
    /// </summary>
    /// <param name="themeKey">等级</param>
    /// <param name="color">颜色</param>
    public void SetTheme(string themeKey, ConsoleColor color)
    {
        _themeData[themeKey] = color;
    }

    /// <summary>
    /// 按等级获取颜色
    /// </summary>
    /// <param name="themeKey">等级</param>
    /// <returns>颜色</returns>
    public ConsoleColor GetTheme(string themeKey)
    {
        return _themeData.GetValueOrDefault(themeKey, ConsoleColor.Gray);
    }
    
    /// <inheritdoc cref="GetTheme"/>
    public ConsoleColor this[string themeKey] => GetTheme(themeKey);
}