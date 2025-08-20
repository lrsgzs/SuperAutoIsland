using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;

namespace SuperAutoIsland.Controls.SettingPages;

[FullWidthPage]
[SettingsPageInfo("sai.automation","SuperAutoIsland 自动化","\uEDC1","\uEDC0")]
public partial class AutomationSettingsPage : SettingsPageBase {
    public AutomationSettingsPage()
    {
        InitializeComponent();
    }
}