using CommunityToolkit.Mvvm.ComponentModel;

namespace SuperAutoIsland.Models.Settings;

public class MainConfigModel : ObservableObject
{
    public event Action? RestartPropertyChanged;
    
    private string _serverPort = "21870";
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