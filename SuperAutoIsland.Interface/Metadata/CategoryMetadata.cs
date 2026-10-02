using ClassIsland.Core.Icons;

namespace SuperAutoIsland.Interface.Metadata;

/// <summary>
/// 分类元数据
/// </summary>
public class CategoryMetadata(string name)
{
    /// <summary>
    /// 分类名称（同时作为分类的唯一标识，重名会覆盖原分类）
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// 分类图标
    /// </summary>
    public (string, string) Icon { get; set; } = ("分类", FluentIcons.SettingsRegular);
}
