using Avalonia.Interactivity;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Shared;
using SuperAutoIsland.Models.Settings;
using SuperAutoIsland.Shared;

namespace SuperAutoIsland.Views.SettingPages;

[SettingsPageInfo("sai.master","SuperAutoIsland 主设置","\uF3AF","\uF3AE")]
public partial class MainSettingsPage : SettingsPageBase {
    public MainConfigModel Settings { get; set; }
    private bool _isRequestedRestart = false;
    
    public MainSettingsPage()
    {
        Settings = GlobalConstants.Configs.MainConfig!.Data;
        InitializeComponent();
        
        Settings.RestartPropertyChanged += SettingsOnPropertyChanged;
    }
    
    private void SettingsOnPropertyChanged()
    {
        if (_isRequestedRestart) return;
        
        RequestRestart();
        _isRequestedRestart = true;
    }

    private void ViewLogsButton_OnClick(object? sender, RoutedEventArgs e)
    {
        IAppHost.GetService<SaiLogsWindow>().Open();
    }
}