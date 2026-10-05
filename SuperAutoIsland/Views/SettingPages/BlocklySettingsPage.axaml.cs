using System.ComponentModel;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Helpers.UI;
using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Models.Settings;
using SuperAutoIsland.Shared;

namespace SuperAutoIsland.Views.SettingPages;

/// <summary>
///     「Blockly 设置」视图。集中管理 Blockly 的实验性积木分类。
/// </summary>
[HidePageTitle]
[Group("sai.settings")]
[SettingsPageInfo("sai.settings.blockly", "Blockly 设置", FluentIcons.AppsListRegular, FluentIcons.AppsListFilled)]
public partial class BlocklySettingsPage : SettingsPageBase
{
    private bool _isRequestedRestart;

    public BlocklySettingsPage()
    {
        Settings = GlobalConstants.Configs.MainConfig!.Data;
        InitializeComponent();

        Settings.RestartPropertyChanged += SettingsOnRestartPropertyChanged;
        Settings.ProfileFeatures.PropertyChanged += ProfileFeaturesOnPropertyChanged;
        Settings.AppSettingsBlocks.PropertyChanged += AppSettingsBlocksOnPropertyChanged;
        Settings.BlocklyCategories.PropertyChanged += BlocklyCategoriesOnPropertyChanged;
    }

    public MainConfigModel Settings { get; set; }

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
}
