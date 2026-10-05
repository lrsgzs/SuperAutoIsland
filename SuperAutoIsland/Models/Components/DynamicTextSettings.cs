using System.Text.Json.Serialization;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentAvalonia.UI.Controls;
using SuperAutoIsland.Shared;

namespace SuperAutoIsland.Models.Components;

public partial class DynamicTextSettings : ObservableObject
{
    [ObservableProperty]
    private double _iconSize = 24;

    [ObservableProperty]
    private string _id = Utils.GenerateRandomId();

    [property: JsonIgnore]
    [ObservableProperty]
    private Color _lastColor = Colors.White;

    [property: JsonIgnore]
    [ObservableProperty]
    private FAIconSource? _lastIconSource;

    [property: JsonIgnore]
    [ObservableProperty]
    private string _lastText = string.Empty;
}