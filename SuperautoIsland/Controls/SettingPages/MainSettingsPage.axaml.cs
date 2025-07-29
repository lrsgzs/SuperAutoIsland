using Avalonia.Controls;
using Avalonia.Media;
using ClassIsland.Core;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;

namespace SuperAutoIsland.Controls.SettingPages;

[SettingsPageInfo("sai.master","SuperAutoIsland 设置","\uEDC1","\uEDC0")]
public partial class MainSettingsPage : SettingsPageBase {
    public MainSettingsPage()
    {
        InitializeComponent();
    }
}