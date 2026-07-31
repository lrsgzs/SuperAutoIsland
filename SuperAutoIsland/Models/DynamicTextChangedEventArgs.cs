namespace SuperAutoIsland.Models;

public class DynamicTextChangedEventArgs : EventArgs
{
    public required string Key { get; init; }
    public required string Value { get; init; }
}