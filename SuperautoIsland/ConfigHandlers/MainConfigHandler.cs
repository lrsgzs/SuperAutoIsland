using System.ComponentModel;
using ClassIsland.Shared.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using SuperAutoIsland.Shared;

namespace SuperAutoIsland.ConfigHandlers;

public class MainConfigHandler
{
    private readonly string _configPath;
    public MainConfigData Data { get; set; }
    
    public MainConfigHandler() {
        _configPath = Path.Combine(GlobalConstants.PluginConfigFolder!,"Main.json");
        Data = new MainConfigData();

        InitializeConfig();
        Data.PropertyChanged += OnPropertyChanged;
    }
    
    private void InitializeConfig() {
        if (string.IsNullOrEmpty(GlobalConstants.PluginConfigFolder)) {
            throw new InvalidOperationException("配置文件夹路径未设置");
        }

        if (!File.Exists(_configPath)) {
            Save();
            return;
        }

        try {
            Data = ConfigureFileHelper.LoadConfig<MainConfigData>(_configPath);
        }
        catch (Exception ex) {
            Console.WriteLine($"[ExIsLand][Tracer][MainCfgHandler] 加载配置文件失败: {ex.Message}");
            File.Delete(_configPath);
            Save();
        }
    }
    
    private void OnPropertyChanged(object? sender,PropertyChangedEventArgs e) {
        Save();
    }

    public void Save() {
        try {
            ConfigureFileHelper.SaveConfig(_configPath,Data);
        }
        catch (Exception ex) {
            Console.WriteLine($"[ExIsLand][Tracer][MainCfgHandler] 写入配置文件失败: {ex.Message}");
            throw;
        }
    }
}

public class MainConfigData : ObservableObject
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
