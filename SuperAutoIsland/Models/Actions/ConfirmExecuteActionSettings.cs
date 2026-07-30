using CommunityToolkit.Mvvm.ComponentModel;
using SuperAutoIsland.Enums;

namespace SuperAutoIsland.Models.Actions;

public partial class ConfirmExecuteActionSettings : ObservableRecipient
{
    public class TestObject
    {
        public string Test { get; set; } = string.Empty;
    }
    
    [ObservableProperty] private string _header = "「{name}」工作流执行确认";
    [ObservableProperty] private string _message = "将要执行工作流「{name}」，是否继续？";
    
    [ObservableProperty] private string _yesText = "是";
    [ObservableProperty] private string _noText = "否";
    [ObservableProperty] private bool _preferYes = true;
    
    [ObservableProperty] private bool _topmost = false;
    [ObservableProperty] private bool _countdownEnabled = false;
    [ObservableProperty] private double _countdownTime = 5;
    [ObservableProperty] private CountdownMode _countdownMode = CountdownMode.Enable;

    [ObservableProperty] private bool _canDelay = false;
    [ObservableProperty] private double _defaultDelayTime = 5;
    [ObservableProperty] private string _delayText = "延时执行";
    [ObservableProperty] private string _delayDescription = "延时指定时间后，继续执行工作流「{name}」。";
    
    [ObservableProperty] private TestObject _test = new();
}