using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Services.Automations.AppSettings;

namespace SuperAutoIsland.Services.Automations.Categories.AppSettings;

/// <summary>
///     「应用设置」分类提供方。
///     <para>
///         内容按用户选择的设置项动态生成：最上方是只有下拉框的「选项参照」积木（无分组标签），
///         然后是设置积木，最后是获取积木。
///     </para>
/// </summary>
public class AppSettingsCategoryProvider : ICategoryProvider
{
    /// <inheritdoc />
    public CategoryMetadata Metadata => new("CI 应用设置")
    {
        Icon = ("应用设置", FluentIcons.AppsRegular),
        Colors = new CategoryColors("#318098")
    };

    /// <inheritdoc />
    public void Build(BlocksRegister it)
    {
        AppSettingBlockFactory.Build(it);
    }
}