using CommunityToolkit.Mvvm.ComponentModel;

namespace SuperAutoIsland.Models.Settings;

/// <summary>
///     档案功能中单个板块的积木开关。
///     写入类积木有破坏档案的风险，因此默认关闭；有写入类积木的板块在
///     <see cref="ProfileFeaturesModel" /> 中显式打开。
/// </summary>
public partial class ProfileSectionSettings(bool read, bool write) : ObservableObject
{
    [ObservableProperty]
    private bool _read = read;

    [ObservableProperty]
    private bool _write = write;

    public ProfileSectionSettings() : this(true, false) { }
}