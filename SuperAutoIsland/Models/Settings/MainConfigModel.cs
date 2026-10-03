using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SuperAutoIsland.Models.Settings;

/// <summary>
///     主设置模型
/// </summary>
public partial class MainConfigModel : ObservableObject
{
    [ObservableProperty]
    private bool _enableEasterEggs;

    /// <summary>
    ///     构造函数。
    ///     <para>
    ///         属性初始化器会直接写入后备字段、不会经过 setter，因此这里需要为初始实例补上子配置的变更订阅，
    ///         否则子项变化不会冒泡到 <see cref="ObservableObject.PropertyChanged" />，配置也不会即时保存。
    ///     </para>
    /// </summary>
    public MainConfigModel()
    {
        AppSettingsBlocks.PropertyChanged += OnAppSettingsBlocksChanged;
    }

    /// <summary>
    ///     服务器端口号
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
    ///     是否启用档案功能
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

    /// <summary>
    ///     档案功能设置（板块的读写开关）。子功能开关无需重启即可生效。
    /// </summary>
    public ProfileFeaturesModel ProfileFeatures
    {
        get;
        set
        {
            if (value is null || ReferenceEquals(value, field)) return;
            field.PropertyChanged -= OnProfileFeaturesChanged;
            field = value;
            field.PropertyChanged += OnProfileFeaturesChanged;
            OnPropertyChanged();
        }
    } = new();

    /// <summary>
    ///     是否启用应用设置积木（实验性功能）。
    /// </summary>
    public bool EnableAppSettingsBlocks
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

    /// <summary>
    ///     应用设置积木设置（要展示的设置项）。子项变化无需重启即可生效。
    /// </summary>
    public AppSettingsBlocksModel AppSettingsBlocks
    {
        get;
        set
        {
            if (value is null || ReferenceEquals(value, field)) return;
            field.PropertyChanged -= OnAppSettingsBlocksChanged;
            field = value;
            field.PropertyChanged += OnAppSettingsBlocksChanged;
            OnPropertyChanged();
        }
    } = new();

    /// <summary>
    ///     需要重启的类型修改时触发的事件。
    /// </summary>
    public event Action? RestartPropertyChanged;

    /// <summary>
    ///     档案功能子开关变化时保存配置（不请求重启）。
    /// </summary>
    private void OnProfileFeaturesChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(ProfileFeatures));
    }

    /// <summary>
    ///     应用设置积木的选择变化时向上转发，便于配置保存与积木重建（不请求重启）。
    /// </summary>
    private void OnAppSettingsBlocksChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(AppSettingsBlocks));
    }
}