using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Enums.SettingsWindow;

namespace SuperAutoIsland.Views.SettingPages;

/// <summary>
/// 「关于 SuperAutoIsland」视图
/// </summary>
[SettingsPageInfo("sai.about","关于 SuperAutoIsland","\uF373","\uF372", SettingsPageCategory.About)]
public partial class AboutSettingsPage : SettingsPageBase {
    public AboutSettingsPage()
    {
        InitializeComponent();
    }
}