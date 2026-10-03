using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using SuperAutoIsland.Enums;
using SuperAutoIsland.Shared;

namespace SuperAutoIsland.Models.Rules;

public partial class YesNoDialogRuleSettings : ObservableRecipient
{
    [ObservableProperty]
    private bool _countdownEnabled;

    [ObservableProperty]
    private CountdownMode _countdownMode = CountdownMode.Enable;

    [ObservableProperty]
    private double _countdownTime = 5;

    [ObservableProperty]
    private string _header = "做出您的选择...";

    [ObservableProperty]
    private string _message = "您是否同意..";

    [ObservableProperty]
    private string _noText = "否";

    [ObservableProperty]
    private bool _preferYes = true;

    // 教学安全叠甲
    [ObservableProperty]
    private bool _showOnce;

    [ObservableProperty]
    private bool _topmost;

    [ObservableProperty]
    private string _yesText = "是";

    public YesNoDialogRuleSettings()
    {
        if (GlobalConstants.Configs.MainConfig!.Data.EnableEasterEggs)
        {
            _message = "确定要接纳「错谬」吗？";
        }
    }

    [JsonIgnore]
    public bool Showed { get; set; } = false;

    [JsonIgnore]
    public bool LastResult { get; set; } = false;
}