using System.Text.Json.Serialization;
using Avalonia.Media;
using ClassIsland.Core.Converters;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SuperAutoIsland.Models.Actions;

public partial class SetDynamicTextColorActionSettings : ObservableRecipient
{
    [ObservableProperty] private string _key = string.Empty;
    [ObservableProperty] private bool _useDefaultColor = true;
    [property: JsonConverter(typeof(ColorHexJsonConverter))]
    [ObservableProperty] private Color _color = Colors.White;

    [JsonIgnore] public bool ShowColorPicker => !UseDefaultColor;

    partial void OnUseDefaultColorChanged(bool value)
    {
        OnPropertyChanged(nameof(ShowColorPicker));
    }
}
