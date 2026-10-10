using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Helpers.UI;
using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using FluentAvalonia.UI.Controls;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Models.Settings;
using SuperAutoIsland.Shared;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Views.SettingPages;

/// <summary>
///     「Blockly 设置」视图。
/// </summary>
[HidePageTitle]
[Group("sai.settings")]
[SettingsPageInfo("sai.settings.blockly", "Blockly 设置", FluentIcons.AppsListRegular, FluentIcons.AppsListFilled)]
public partial class BlocklySettingsPage : SettingsPageBase
{
    private readonly Logger<BlocklySettingsPage> _logger = new();
    private bool _isRequestedRestart;

    public BlocklySettingsPage()
    {
        Settings = GlobalConstants.Configs.MainConfig!.Data;
        InitializeComponent();

        Settings.RestartPropertyChanged += SettingsOnRestartPropertyChanged;
        Settings.ProfileFeatures.PropertyChanged += ProfileFeaturesOnPropertyChanged;
        Settings.AppSettingsBlocks.PropertyChanged += AppSettingsBlocksOnPropertyChanged;
        Settings.BlocklyCategories.PropertyChanged += BlocklyCategoriesOnPropertyChanged;
        Settings.PropertyChanged += SettingsOnPropertyChanged;

        UpdateStorageSummary();
    }

    public MainConfigModel Settings { get; set; }

    /// <summary>
    ///     是否为安卓端。安卓端强制使用应用内 JS 编辑器，对应的开关隐藏。
    /// </summary>
    public bool IsAndroid { get; } = OperatingSystem.IsAndroid();

    /// <summary>
    ///     本页的某个开关需要重启才能生效：请求重启应用。
    ///     只响应本页展示的设置项，避免与其它设置页重复提示。
    /// </summary>
    private void SettingsOnRestartPropertyChanged(string propertyName)
    {
        if (propertyName is not (nameof(MainConfigModel.EnableProfileFeatures)
            or nameof(MainConfigModel.EnableAppSettingsBlocks)))
        {
            return;
        }

        if (_isRequestedRestart)
        {
            return;
        }

        RequestRestart();
        _isRequestedRestart = true;
    }

    /// <summary>
    ///     档案功能子开关变化：重新构建档案分类的积木，无需重启
    /// </summary>
    private void ProfileFeaturesOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (!Settings.EnableProfileFeatures)
        {
            return;
        }

        IAppHost.GetService<ISaiServer>().NotifyCategoryUpdated();
    }

    /// <summary>
    ///     应用设置积木的选择变化：重新构建「应用设置」分类，无需重启
    /// </summary>
    private void AppSettingsBlocksOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (!Settings.EnableAppSettingsBlocks)
        {
            return;
        }

        IAppHost.GetService<ISaiServer>().NotifyCategoryUpdated();
    }

    /// <summary>
    ///     打开「选择要展示的设置项」视图
    /// </summary>
    private async void SelectAppSettingsButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var view = new AppSettingsSelectorView();
        var owner = this.FindAncestorOfType<ViewBase>();

        if (owner != null)
        {
            await view.ShowModal(owner);
        }
        else
        {
            await view.ShowModal();
        }
    }

    /// <summary>
    ///     分类顺序或显示选择变化：重建后端分类（无需重启）。
    ///     <para>
    ///         只影响发给 Blockly 工具箱的分类列表，积木本身照常注册，运行时调用不受影响。
    ///     </para>
    /// </summary>
    private void BlocklyCategoriesOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        IAppHost.GetService<ISaiServer>().NotifyCategoryUpdated();
    }

    /// <summary>
    ///     打开「积木分类管理」视图
    /// </summary>
    private async void ManageCategoriesButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var view = new BlocklyCategoryManagerView();
        var owner = this.FindAncestorOfType<ViewBase>();

        if (owner != null)
        {
            await view.ShowModal(owner);
        }
        else
        {
            await view.ShowModal();
        }
    }

    /// <summary>
    ///     主配置变化：脚本跑完把 localStorage 落盘时会通知这个属性，顺手刷新「脚本存储」的摘要。
    ///     <para>
    ///         脚本在后台线程跑，所以通知可能来自非 UI 线程，刷控件要回到 UI 线程。
    ///     </para>
    /// </summary>
    private void SettingsOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is not nameof(MainConfigModel.ScriptLocalStorage))
        {
            return;
        }

        Dispatcher.UIThread.Post(UpdateStorageSummary);
    }

    /// <summary>
    ///     刷新「脚本存储」的摘要文本与清理按钮的可用状态。
    /// </summary>
    private void UpdateStorageSummary()
    {
        var items = Settings.ScriptLocalStorage;

        StorageSummaryText.Text = items.Count == 0
            ? "当前没有脚本存储的数据"
            : $"当前 {items.Count} 项，约 {FormatSize(EstimateSize(items))}";
        ClearStorageButton.IsEnabled = items.Count > 0;
    }

    /// <summary>
    ///     估算存储占用。Jint 按 UTF-16 计费（每个字符 2 字节），这里跟它保持一致。
    /// </summary>
    private static long EstimateSize(Dictionary<string, string> items)
    {
        return items.Sum(item => (long)(item.Key.Length + item.Value.Length) * 2);
    }

    /// <summary>
    ///     把字节数写成人类看得懂的形式。
    /// </summary>
    private static string FormatSize(long bytes)
    {
        return bytes < 1024 ? $"{bytes} 字节" : $"{bytes / 1024.0:0.#} KB";
    }

    /// <summary>
    ///     清理脚本存储：先弹 ContentDialog 确认，再清空配置里的字典并落盘。
    /// </summary>
    private async void ClearStorageButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var items = Settings.ScriptLocalStorage;
        if (items.Count == 0)
        {
            return;
        }

        var count = items.Count;
        var size = FormatSize(EstimateSize(items));

        try
        {
            var dialog = new FAContentDialog
            {
                Title = "清理脚本存储",
                Content = new TextBlock
                {
                    Text = $"将删除脚本通过 localStorage 写入的 {count} 项数据（约 {size}），此操作无法撤销。",
                    TextWrapping = TextWrapping.Wrap
                },
                PrimaryButtonText = "清理",
                CloseButtonText = "取消",
                DefaultButton = FAContentDialogButton.Close
            };

            if (await dialog.ShowAsync(TopLevel.GetTopLevel(this)) != FAContentDialogResult.Primary)
            {
                return;
            }

            items.Clear();
            Settings.NotifyScriptLocalStorageChanged();
            UpdateStorageSummary();
            this.ShowSuccessToast($"已清理 {count} 项脚本存储数据。");
        }
        catch (Exception exception)
        {
            _logger.Error("清理脚本存储失败。");
            _logger.FormatException(exception);
            this.ShowErrorToast("清理脚本存储失败。", exception);
        }
    }
}
