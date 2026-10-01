using System.Text.Json.Serialization;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentAvalonia.UI.Controls;

namespace SuperAutoIsland.Models.Components;

public partial class DynamicTextSettings : ObservableObject
{
    [ObservableProperty] private string _id = GenerateRandomId();
    [ObservableProperty] private double _iconSize = 24;

    [property: JsonIgnore] [ObservableProperty] private string _lastText = string.Empty;
    [property: JsonIgnore] [ObservableProperty] private Color _lastColor = Colors.White;
    [property: JsonIgnore] [ObservableProperty] private FAIconSource? _lastIconSource;

    private static string GenerateRandomId()
    {
        return Guid.NewGuid().ToString("N")[..8];
    }
}
