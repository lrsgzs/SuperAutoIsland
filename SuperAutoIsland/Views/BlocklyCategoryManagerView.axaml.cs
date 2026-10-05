using ClassIsland.Core.Abstractions.Controls;
using SuperAutoIsland.Shared;
using SuperAutoIsland.ViewModel.SettingPages;

namespace SuperAutoIsland.Views;

/// <summary>
///     「积木分类管理」视图。
///     <para>
///         抽屉空间过于局促，因此改为以 <see cref="ViewBase" /> 独立视图展示，
///         内容容器使用与设置页一致的 <c>SettingsContainerWidth</c>。
///     </para>
/// </summary>
public partial class BlocklyCategoryManagerView : ViewBase
{
    public BlocklyCategoryManagerView()
    {
        InitializeComponent();
    }

    /// <summary>
    ///     视图模型。属性初始化器先于构造函数体执行，因此 <c>InitializeComponent</c> 时已可用。
    /// </summary>
    public BlocklyCategoryManagerViewModel ViewModel { get; } =
        new(GlobalConstants.Configs.MainConfig!.Data.BlocklyCategories);
}
