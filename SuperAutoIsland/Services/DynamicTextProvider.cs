using Avalonia.Media;
using SuperAutoIsland.Models;

namespace SuperAutoIsland.Services;

public class DynamicTextProvider
{
    private Dictionary<string, DynamicTextItem> _textDictionary = new();
    private Dictionary<string, DynamicTextItem> _textOldDictionary = new();
    
    public EventHandler<DynamicTextChangedEventArgs>? Changed;
    
    public void SetText(string key, string value)
    {
        var current = _textDictionary.GetValueOrDefault(key) ?? new DynamicTextItem();
        SetItem(key, new DynamicTextItem
        {
            Text = value,
            HasCustomColor = current.HasCustomColor,
            Color = current.Color
        });
    }

    public void SetColor(string key, bool hasCustomColor, Color color)
    {
        var current = _textDictionary.GetValueOrDefault(key) ?? new DynamicTextItem();
        SetItem(key, new DynamicTextItem
        {
            Text = current.Text,
            HasCustomColor = hasCustomColor,
            Color = color
        });
    }

    public void SetItem(string key, DynamicTextItem? item)
    {
        if (item == null)
        {
            RemoveText(key);
            return;
        }

        if (_textDictionary.TryGetValue(key, out var oldValue))
        {
            _textOldDictionary[key] = oldValue;
        }
        
        _textDictionary[key] = item;
        Changed?.Invoke(this, new DynamicTextChangedEventArgs
        {
            Key = key,
            Value = item
        });
    }
    
    public DynamicTextItem? GetText(string key)
    {
        return _textDictionary.GetValueOrDefault(key);
    }
    
    public DynamicTextItem? GetTextOldValue(string key)
    {
        return _textOldDictionary.GetValueOrDefault(key);
    }
    
    public void RemoveText(string key)
    {
        _textDictionary.Remove(key);
        _textOldDictionary.Remove(key);
    }
}
