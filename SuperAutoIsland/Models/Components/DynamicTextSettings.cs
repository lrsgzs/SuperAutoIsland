using System.Text.Json.Serialization;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SuperAutoIsland.Models.Components;

public partial class DynamicTextSettings : ObservableObject
{
    [ObservableProperty] private string _id = GenerateRandomId();
    [property: JsonIgnore] [ObservableProperty] private string _lastText = string.Empty;
    [property: JsonIgnore] [ObservableProperty] private Color _lastColor = Colors.White;

    private static string GenerateRandomId()
    {
        return Guid.NewGuid().ToString("N")[..8];
    }
}
