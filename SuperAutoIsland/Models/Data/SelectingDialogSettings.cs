namespace SuperAutoIsland.Models.Data;

public class SelectingDialogSettings
{
    public string Header { get; set; } = "ChooseOne...";
    public string Message { get; set; } = "请选择你的心向之物。";
    public Dictionary<string, string> Items { get; set; } = [];
    public string Default { get; set; } = "sandrone";
    
    public bool Topmost { get; set; } = false;
    public bool CountdownEnabled { get; set; } = true;
    public double CountdownTime { get; set; } = 5;
}