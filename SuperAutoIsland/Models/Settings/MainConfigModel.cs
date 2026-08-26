using CommunityToolkit.Mvvm.ComponentModel;

namespace SuperAutoIsland.Models.Settings;

/// <summary>
/// 主设置模型
/// </summary>
public partial class MainConfigModel : ObservableObject
{
    /// <summary>
    /// 需要重启的类型修改时触发的事件。
    /// </summary>
    public event Action? RestartPropertyChanged;

    /// <summary>
    /// 服务器端口号
    /// </summary>
    public string ServerPort
    {
        get;
        set
        {
            if (value == field) return;
            field = value;
            RestartPropertyChanged?.Invoke();
            OnPropertyChanged();
        }
    } = "21870";

    /// <summary>
    /// 是否启用档案功能
    /// </summary>
    public bool EnableProfileFeatures
    {
        get;
        set
        {
            if (value == field) return;
            field = value;
            RestartPropertyChanged?.Invoke();
            OnPropertyChanged();
        }
    } = false;

    [ObservableProperty] private bool _enableEasterEggs;
}