using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using SuperAutoIsland.ConfigHandlers;
using SuperAutoIsland.Shared;

namespace SuperAutoIsland.Controls.SettingPages;

[SettingsPageInfo("sai.master","SuperAutoIsland 主设置","\uF3AF","\uF3AE")]
public partial class MainSettingsPage : SettingsPageBase {
    public MainConfigData Settings { get; set; }
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
}