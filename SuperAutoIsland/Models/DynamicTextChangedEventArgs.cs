namespace SuperAutoIsland.Models;

public class DynamicTextChangedEventArgs : EventArgs
{
    public required string Key { get; init; }
    public required DynamicTextItem Value { get; init; }
}
