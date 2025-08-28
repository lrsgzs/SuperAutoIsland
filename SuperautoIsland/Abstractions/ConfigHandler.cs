using System.ComponentModel;
using ClassIsland.Shared.Helpers;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Abstractions;

public class ConfigHandler<TData> where TData : INotifyPropertyChanged, new()
{
    public string ConfigPath = string.Empty;
    public TData Data { get; private set; }
    private readonly Logger<ConfigHandler<TData>> _logger = new();

    protected ConfigHandler()
    {
        Data = new TData();
        Data.PropertyChanged += OnPropertyChanged;
    }
    
    protected void InitializeConfig()
    {
        if (string.IsNullOrEmpty(ConfigPath))
        {
            throw new InvalidOperationException("配置文件夹路径未设置");
        }

        if (!File.Exists(ConfigPath))
        {
            Save();
            return;
        }

        try
        {
            Data = ConfigureFileHelper.LoadConfig<TData>(ConfigPath);
            Data.PropertyChanged += OnPropertyChanged;
        }
        catch (Exception ex)
        {
            _logger.Error("加载配置文件失败");
            _logger.FormatException(ex);
            File.Delete(ConfigPath);
            Save();
        }
    }
    
    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        Save();
    }

    protected void Save()
    {
        _logger.Info("保存配置中...");
        try
        {
            ConfigureFileHelper.SaveConfig(ConfigPath,Data);
        }
        catch (Exception ex)
        {
            _logger.Error("写入配置文件失败");
            _logger.FormatException(ex);
            throw;
        }
    }
}