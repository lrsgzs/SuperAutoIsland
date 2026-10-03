using System.Text.Json.Serialization;
using Avalonia.Media;
using ClassIsland.Core.Converters;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SuperAutoIsland.Models.Actions;

public partial class SetDynamicTextColorActionSettings : ObservableRecipient
{
    [property: JsonConverter(typeof(ColorHexJsonConverter))]
    [ObservableProperty]
    private Color _color = DefaultColor;

    [ObservableProperty]
    private string _key = string.Empty;

    [ObservableProperty]
    private bool _useDefaultColor = true;

    public static Color DefaultColor => Colors.White;
}