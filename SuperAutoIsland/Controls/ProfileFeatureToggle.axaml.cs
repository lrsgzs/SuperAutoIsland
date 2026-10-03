using Avalonia;
using Avalonia.Controls;
using SuperAutoIsland.Models.Settings;

namespace SuperAutoIsland.Controls;

public partial class ProfileFeatureToggle : UserControl
{
    public static readonly StyledProperty<string> LabelProperty =
        AvaloniaProperty.Register<ProfileFeatureToggle, string>(nameof(Label), string.Empty);

    public static readonly StyledProperty<ProfileSectionSettings?> SettingsProperty =
        AvaloniaProperty.Register<ProfileFeatureToggle, ProfileSectionSettings?>(nameof(Settings));

    public ProfileFeatureToggle()
    {
        InitializeComponent();
    }

    public string Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public ProfileSectionSettings? Settings
    {
        get => GetValue(SettingsProperty);
        set => SetValue(SettingsProperty, value);
    }
}