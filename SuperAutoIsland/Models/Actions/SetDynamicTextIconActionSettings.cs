using ClassIsland.Core.Icons;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SuperAutoIsland.Models.Actions;

/// <summary>
/// 设置动态文本图标的设置
/// </summary>
public partial class SetDynamicTextIconActionSettings : ObservableRecipient
{
    [ObservableProperty] private string _key = string.Empty;
    [ObservableProperty] private bool _includeIcon = true;
    [ObservableProperty] private string? _icon = DefaultIcon;

    public static string DefaultIcon => $"lucide({LucideIcons.Info})";
}
