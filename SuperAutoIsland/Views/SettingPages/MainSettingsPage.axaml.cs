using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Styling;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Helpers.UI;
using ClassIsland.Core.Icons;
using ClassIsland.Core.Models.UI;
using ClassIsland.Shared;
using SuperAutoIsland.Models.Settings;
using SuperAutoIsland.Shared;

namespace SuperAutoIsland.Views.SettingPages;

/// <summary>
/// 「SuperAutoIsland 主页」视图
/// </summary>
[HidePageTitle]
[Group("sai.settings")]
[SettingsPageInfo("sai.settings.main","主设置",FluentIcons.HomeRegular,FluentIcons.HomeFilled)]
public partial class MainSettingsPage : SettingsPageBase {
    public MainConfigModel Settings { get; set; }
    private bool _isRequestedRestart = false;

    private int _clickCounts = 0;
    
    public MainSettingsPage()
    {
        Settings = GlobalConstants.Configs.MainConfig!.Data;
        InitializeComponent();
        
        Settings.RestartPropertyChanged += SettingsOnPropertyChanged;
        
        DebugComboBox.ItemsSource = ActionSerializer.GetActionsId();
    }
    
    private void SettingsOnPropertyChanged()
    {
        if (_isRequestedRestart) return;
        
        RequestRestart();
        _isRequestedRestart = true;
    }

    /// <summary>
    /// 查看日志点击事件
    /// </summary>
    private void ViewLogsButton_OnClick(object? sender, RoutedEventArgs e)
    {
        IAppHost.GetService<SaiLogsView>().Open();
    }

    private void DebugGetInfo_OnClick(object? sender, RoutedEventArgs e)
    {
        var selectedItem = DebugComboBox.SelectedItem;
        if (selectedItem is not string actionId) return;
        
        DebugTextBox.Text = ActionSerializer.GetActionInfo(actionId);
    }

    private void EasterEggsItem_OnClick(object? sender, PointerPressedEventArgs e)
    {
        if (!Settings.EnableEasterEggs)
        {
            ImageVisibility.IsVisible = false;
            ImageVisibility.IsChecked = false;
            _clickCounts = 0;
            return;
        }

        _clickCounts++;

        if (_clickCounts >= 50)
        {
            ImageVisibility.IsVisible = true;
        }
    }

    private void ImageVisibility_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (!(ImageVisibility.IsChecked ?? false))
            return;
        
        this.ShowToast(new ToastMessage
        {
            Message = "SuperAutoIsland 1周年快乐。\n愿 SAI 能够继续前进，成就更好的自动化！",
            AutoClose = true,
            Duration = TimeSpan.FromSeconds(10)
        });

        var action = new Button
        {
            Content = "因为，我们本就是一个人啊...",
            Theme = this.FindResource("TransparentButton") as ControlTheme
        };

        action.Click += (_, _) =>
        {
            this.ShowToast(new ToastMessage
            {
                Title = "那些我们所留驻的...",
                Message = "留住珍贵之物的方法并不只有坚守你已有的东西。\n只要愿意伸出手去争取，那么你得到的会比你想的多得多。",
                AutoClose = true,
                Duration = TimeSpan.FromSeconds(5)
            });
        };
        
        this.ShowToast(new ToastMessage
        {
            Title = "我的...愿望是...",
            Message = "我知道的，我一直知道",
            ActionContent = action,
            AutoClose = true,
            Duration = TimeSpan.FromSeconds(5)
        });
    }
}