using CommunityToolkit.Mvvm.ComponentModel;

namespace SuperAutoIsland.Models.Settings;

/// <summary>
/// 主设置模型
/// </summary>
public class MainConfigModel : ObservableObject
{
    /// <summary>
    /// 需要重启的类型修改时触发的事件。
    /// </summary>
    public event Action? RestartPropertyChanged;
    
    private string _serverPort = "21870";
    
    /// <summary>
    /// 服务器端口号
    /// </summary>
    public string ServerPort
    {
        get => _serverPort;
        set {
            if (value == _serverPort) return;
            _serverPort = value;
            RestartPropertyChanged?.Invoke();
            OnPropertyChanged();
        }
    }
}