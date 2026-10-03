using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Helpers.UI;
using SuperAutoIsland.Services.Automations.AppSettings;
using SuperAutoIsland.Shared;
using SuperAutoIsland.Shared.Logger;
using SuperAutoIsland.ViewModel.SettingPages;

namespace SuperAutoIsland.Views;

/// <summary>
///     「选择要展示的设置项」视图。
///     <para>
///         抽屉空间过于局促，因此改为以 <see cref="ViewBase" /> 独立视图展示，
///         内容容器使用与设置页一致的 <c>SettingsContainerWidth</c>。
///     </para>
/// </summary>
public partial class AppSettingsSelectorView : ViewBase
{
    private readonly Logger<AppSettingsSelectorView> _logger = new();

    public AppSettingsSelectorView()
    {
        InitializeComponent();
    }

    /// <summary>
    ///     视图模型。属性初始化器先于构造函数体执行，因此 <c>InitializeComponent</c> 时已可用。
    /// </summary>
    public AppSettingsSelectorViewModel ViewModel { get; } =
        new(GlobalConstants.Configs.MainConfig!.Data.AppSettingsBlocks);

    /// <summary>
    ///     复制设置项当前值点击事件。
    /// </summary>
    private async void ButtonCopyValue_OnClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { DataContext: AppSettingRowViewModel row } || row.Descriptor == null)
        {
            return;
        }

        try
        {
            var text = ClassIslandSettingsAccessor.GetCopyValue(row.Descriptor);

            if (string.IsNullOrEmpty(text))
            {
                this.ShowWarningToast($"「{row.DisplayName}」的当前值无法复制。");
                return;
            }

            var clipboard = TopLevel?.Clipboard;
            if (clipboard == null)
            {
                this.ShowErrorToast("无法访问剪贴板。");
                return;
            }

            await clipboard.SetTextAsync(text);
            this.ShowSuccessToast($"已复制「{row.DisplayName}」的当前值。");
        }
        catch (Exception ex)
        {
            _logger.Error($"复制设置项 {row.Descriptor.PropertyName} 的当前值失败。");
            _logger.FormatException(ex);
            this.ShowErrorToast("复制当前值失败。", ex);
        }
    }
}
