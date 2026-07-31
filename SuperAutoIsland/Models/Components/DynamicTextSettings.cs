using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SuperAutoIsland.Models.Components;

public partial class DynamicTextSettings : ObservableObject
{
    [ObservableProperty] private string _id = GenerateRandomId();
    [property: JsonIgnore] [ObservableProperty] private string _lastText = string.Empty;

    private static string GenerateRandomId()
    {
        return Guid.NewGuid().ToString("N")[..8];
    }
}