namespace SuperAutoIsland.Models.Data;

public class TextDialogDataModel
{
    public string Header { get; set; } = "运行世界式...";
    public string Message { get; set; } = "请输入要运行的世界式。";
    public string DefaultText { get; set; } = "世界式·反转「归约」";
    public string OkText { get; set; } = "确定";
    public string CancelText { get; set; } = "取消";
    public bool Topmost { get; set; } = false;
    public bool CountdownEnabled { get; set; } = true;
    public double CountdownTime { get; set; } = 5;
}
