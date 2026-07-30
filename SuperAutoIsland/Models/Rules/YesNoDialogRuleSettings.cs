using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using SuperAutoIsland.Enums;

namespace SuperAutoIsland.Models.Rules;

public partial class YesNoDialogRuleSettings : ObservableRecipient
{
    [ObservableProperty] private string _header = "选择您的答案...";
    [ObservableProperty] private string _message = "NO ONE YES MAN!";
    
    [ObservableProperty] private string _yesText = "是";
    [ObservableProperty] private string _noText = "否";
    [ObservableProperty] private bool _preferYes = true;
    
    [ObservableProperty] private bool _topmost = false;
    [ObservableProperty] private bool _countdownEnabled = false;
    [ObservableProperty] private double _countdownTime = 5;
    [ObservableProperty] private CountdownMode _countdownMode = CountdownMode.Enable;

    // 教学安全叠甲
    [ObservableProperty] private bool _showOnce = false;
    [JsonIgnore] public bool Showed { get; set; } = false;
    [JsonIgnore] public bool LastResult { get; set; } = false;
}