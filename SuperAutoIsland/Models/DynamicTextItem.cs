using Avalonia.Media;

namespace SuperAutoIsland.Models;

public class DynamicTextItem
{
    public string Text { get; set; } = string.Empty;
    public bool HasCustomColor { get; set; }
    public Color Color { get; set; } = Colors.White;
}
