using Avalonia.Media;

namespace SuperAutoIsland.Models;

public class DynamicTextItem
{
    public string? Text { get; set; }
    public string? Icon { get; set; }
    public Color? Color { get; set; }

    public DynamicTextItem Clone() => new()
    {
        Text = Text,
        Icon = Icon,
        Color = Color
    };
}
