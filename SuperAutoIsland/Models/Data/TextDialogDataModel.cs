namespace SuperAutoIsland.Models.Data;

public class TextDialogDataModel
{
    public string Header { get; set; } = "请输入文本";
    public string Message { get; set; } = "请输入文本：这将会是一场豪赌。";
    public string DefaultText { get; set; } = "这将会是一场豪赌。";
    public string OkText { get; set; } = "确定";
    public string CancelText { get; set; } = "取消";
    public bool Topmost { get; set; } = false;
    public bool CountdownEnabled { get; set; } = false;
    public double CountdownTime { get; set; } = 5;
}
