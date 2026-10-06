using System.ComponentModel;
using System.Text.Json.Serialization;
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
        BlocklyCategories.PropertyChanged += OnBlocklyCategoriesChanged;
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
            RestartPropertyChanged?.Invoke(nameof(ServerPort));
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
            RestartPropertyChanged?.Invoke(nameof(EnableProfileFeatures));
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
            RestartPropertyChanged?.Invoke(nameof(EnableAppSettingsBlocks));
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
    ///     是否使用应用内编辑器（实验性功能）。
    ///     <para>
    ///         启用后，打开 Blockly 编辑器时会在应用内的独立视图（非模态）中展示前端界面，
    ///         而不是把编辑器交给系统浏览器。此开关在打开编辑器时读取，无需重启即可生效。
    ///     </para>
    /// </summary>
    [ObservableProperty]
    private bool _enableInAppBlocklyEditor = false;

    /// <summary>
    ///     是否使用应用内 JS 编辑器（实验性功能）。
    ///     <para>
    ///         启用后，JavaScript 行动的「打开编辑器」使用应用内的代码编辑器；关闭时改用系统默认程序打开脚本文件。
    ///         此开关在打开编辑器时读取，无需重启即可生效。
    ///     </para>
    /// </summary>
    [ObservableProperty]
    private bool _enableInAppJsEditor = false;

    /// <summary>
    ///     实际是否使用应用内 JS 编辑器。
    ///     <para>
    ///         安卓端没有文件关联，也没有可用的外部编辑器，因此强制启用应用内编辑器
    ///         （对应的设置项在安卓端会隐藏）。
    ///     </para>
    /// </summary>
    [JsonIgnore]
    public bool IsInAppJsEditorEnabled => EnableInAppJsEditor || OperatingSystem.IsAndroid();

    /// <summary>
    ///     Blockly 分类展示设置（工具箱中的顺序与是否展示）。子项变化无需重启即可生效。
    /// </summary>
    public BlocklyCategoriesModel BlocklyCategories
    {
        get;
        set
        {
            if (value is null || ReferenceEquals(value, field)) return;
            field.PropertyChanged -= OnBlocklyCategoriesChanged;
            field = value;
            field.PropertyChanged += OnBlocklyCategoriesChanged;
            OnPropertyChanged();
        }
    } = new();

    /// <summary>
    ///     需要重启的类型修改时触发的事件。参数为发生变化的属性名。
    ///     <para>
    ///         设置项分散在多个设置页面，因此带上属性名，便于各页面只响应自己展示的设置项，
    ///         避免同一个开关触发多次重启提示。
    ///     </para>
    /// </summary>
    public event Action<string>? RestartPropertyChanged;

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

    /// <summary>
    ///     分类顺序或显示选择变化时向上转发，便于配置保存与积木分类重建（不请求重启）。
    /// </summary>
    private void OnBlocklyCategoriesChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(BlocklyCategories));
    }
}