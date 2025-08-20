using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;

namespace SuperAutoIsland.Shared.Logger;

public partial class LogData : ObservableRecipient
{
    [ObservableProperty] private string _scope = "";
    [ObservableProperty] private string _level = "NONE";
    [ObservableProperty] private string _message = "";
    [ObservableProperty] private string _time = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
    
    public override string ToString()
    {
        return $"[{Scope}] [{Time}] [{Level}] {Message}";
    }
}

public class RootLogger : ObservableObject
{
    private const int MaxLogEntries = 1000;
    public readonly SourceList<LogData> LogDataList = new();

    public void AddLog(LogData data)
    {
        LogDataList.Add(data);
        while (LogDataList.Count > MaxLogEntries)
        {
            LogDataList.RemoveAt(0);
        }
        OnPropertyChanged();
    }
    
    public void ClearLogs()
    {
        LogDataList.Clear();
        OnPropertyChanged();
    }
}