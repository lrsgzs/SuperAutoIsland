using CommunityToolkit.Mvvm.ComponentModel;

namespace SuperAutoIsland.Models.Settings;

/// <summary>
///     Blockly 分类展示设置：分类在工具箱中的顺序与是否展示。
/// </summary>
public partial class BlocklyCategoriesModel : ObservableObject
{
    /// <summary>
    ///     分类展示顺序（分类名称）。
    ///     <para>
    ///         null 表示用户尚未自定义，按分类注册顺序展示。已经不存在于注册表中的分类也会保留在这里，
    ///         这样插件恢复后仍能落在原来的位置。
    ///     </para>
    /// </summary>
    [ObservableProperty]
    private List<string>? _order;

    /// <summary>
    ///     不展示的分类名称。null 表示全部分类都展示。
    ///     <para>
    ///         只影响 Blockly 工具箱的展示，不影响积木的注册与运行时调用。
    ///     </para>
    /// </summary>
    [ObservableProperty]
    private List<string>? _hidden;
}
