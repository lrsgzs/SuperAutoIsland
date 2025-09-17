using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;

namespace SuperAutoIsland.Shared.Logger;

/// <summary>
/// 日志信息
/// </summary>
public partial class LogData : ObservableRecipient
{
    /// <summary>
    /// 区域
    /// </summary>
    [ObservableProperty] private string _scope = "";
    
    /// <summary>
    /// 等级
    /// </summary>
    [ObservableProperty] private string _level = "NONE";
    
    /// <summary>
    /// 消息内容
    /// </summary>
    [ObservableProperty] private string _message = "";
    
    /// <summary>
    /// 时间
    /// </summary>
    [ObservableProperty] private string _time = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
    
    /// <summary>
    /// 转字符串
    /// </summary>
    public override string ToString()
    {
        return $"[{Scope}] [{Time}] [{Level}] {Message}";
    }
}

/// <summary>
/// 根日志
/// </summary>
public class RootLogger : ObservableObject
{
    /// <summary>
    /// 最大日志量
    /// </summary>
    private const int MaxLogEntries = 1000;
    public readonly SourceList<LogData> LogDataList = new();

    /// <summary>
    /// 添加日志
    /// </summary>
    /// <param name="data">日志数据</param>
    public void AddLog(LogData data)
    {
        LogDataList.Add(data);
        while (LogDataList.Count > MaxLogEntries)
        {
            LogDataList.RemoveAt(0);
        }
        OnPropertyChanged();
    }
    
    /// <summary>
    /// 清空日志
    /// </summary>
    public void ClearLogs()
    {
        LogDataList.Clear();
        OnPropertyChanged();
    }
}