using CommunityToolkit.Mvvm.ComponentModel;

namespace SuperAutoIsland.Models.Settings;

/// <summary>
///     应用设置积木设置。
/// </summary>
public partial class AppSettingsBlocksModel : ObservableObject
{
    /// <summary>
    ///     要展示的设置项属性名。
    ///     <para>
    ///         null 表示用户尚未自定义，使用默认选择（全部标注了 <c>SettingsInfo</c> 的设置项）；
    ///         空列表表示用户主动取消了全部选择。
    ///     </para>
    /// </summary>
    [ObservableProperty]
    private List<string>? _selected;
}