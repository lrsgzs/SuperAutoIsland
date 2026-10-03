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
}