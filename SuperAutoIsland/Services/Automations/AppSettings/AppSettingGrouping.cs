namespace SuperAutoIsland.Services.Automations.AppSettings;

/// <summary>
///     设置项在「选择要展示的设置项」中的分组。枚举顺序即为展示顺序。
/// </summary>
public enum AppSettingGroup
{
    /// <summary>
    ///     有设置项信息（标注了 SettingsInfo）。默认勾选。
    /// </summary>
    Attributed,

    /// <summary>
    ///     其他设置项。默认不勾选。
    /// </summary>
    Other,

    /// <summary>
    ///     以 JSON 文本读写。
    /// </summary>
    Json,

    /// <summary>
    ///     仅可获取（不可修改）。
    /// </summary>
    GetOnly,

    /// <summary>
    ///     不支持（集合等无法用单个积木表达的类型）。
    /// </summary>
    Unsupported
}

/// <summary>
///     设置项的分组与排序。选择器界面与 Blockly「应用设置」分类共用这一套顺序，保证两边一致。
/// </summary>
public static class AppSettingGrouping
{
    /// <summary>
    ///     判定设置项所属分组。
    /// </summary>
    /// <param name="descriptor">设置项</param>
    public static AppSettingGroup Classify(AppSettingDescriptor descriptor)
    {
        if (!descriptor.IsSupported)
        {
            return AppSettingGroup.Unsupported;
        }

        if (!descriptor.CanSet)
        {
            return AppSettingGroup.GetOnly;
        }

        if (descriptor.IsSettingsInfoAttributed)
        {
            return AppSettingGroup.Attributed;
        }

        return descriptor.Kind == AppSettingValueKind.Json
                   ? AppSettingGroup.Json
                   : AppSettingGroup.Other;
    }

    /// <summary>
    ///     按「分组顺序 → SettingsInfo.Order → 展示名称」排序。
    /// </summary>
    /// <param name="descriptors">要排序的设置项</param>
    public static List<AppSettingDescriptor> SortForDisplay(IEnumerable<AppSettingDescriptor> descriptors)
    {
        return descriptors
               .OrderBy(d => (int)Classify(d))
               .ThenBy(d => d.Order)
               .ThenBy(d => d.DisplayName, StringComparer.CurrentCulture)
               .ToList();
    }
}
