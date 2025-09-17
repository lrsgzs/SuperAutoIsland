using System.ComponentModel;
using ClassIsland.Shared.Helpers;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Abstractions;

/// <summary>
/// 配置处理器类，用于处理特定类型数据的配置文件。
/// TData 必须实现 INotifyPropertyChanged 接口并提供无参构造函数。
/// </summary>
public class ConfigHandler<TData> where TData : INotifyPropertyChanged, new()
{
    public string ConfigPath = string.Empty;
    public TData Data { get; private set; }
    private readonly Logger<ConfigHandler<TData>> _logger = new();

    /// <summary>
    /// 初始化数据对象并订阅属性更改事件。
    /// </summary>
    protected ConfigHandler()
    {
        Data = new TData();
        Data.PropertyChanged += OnPropertyChanged;
    }
    
    /// <summary>
    /// 初始化配置文件，检查路径是否存在并加载或创建配置文件。
    /// </summary>
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
    
    /// <summary>
    /// 当数据属性更改时触发，调用 Save 方法保存配置。
    /// </summary>
    /// <param name="sender">触发事件的对象</param>
    /// <param name="e">属性更改事件参数</param>
    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        Save();
    }

    /// <summary>
    /// 保存数据到配置文件，记录日志并在发生异常时进行处理。
    /// </summary>
    public void Save()
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
