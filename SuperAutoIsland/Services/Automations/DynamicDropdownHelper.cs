namespace SuperAutoIsland.Services.Automations;

/// <summary>
///     动态下拉框内容辅助方法。
/// </summary>
public static class DynamicDropdownHelper
{
    /// <summary>
    ///     下拉框的空占位项。选择该项时对应的引用为空 GUID。
    /// </summary>
    public static (string Name, string Value) EmptyItem => ("[空]", Guid.Empty.ToString());

    /// <summary>
    ///     确保下拉框内容非空。Blockly 的下拉框内容为空时会在前端引发错误，因此需要填充占位项。
    /// </summary>
    public static List<T> EnsureNotEmpty<T>(List<T> items, T fallback)
    {
        return items.Count > 0 ? items : [fallback];
    }

    /// <summary>
    ///     确保下拉框内容非空。内容为空时返回只包含空占位项「[空]」的列表。
    /// </summary>
    public static List<(string Name, string Value)> EnsureNotEmpty(List<(string Name, string Value)> items)
    {
        return EnsureNotEmpty(items, EmptyItem);
    }
}