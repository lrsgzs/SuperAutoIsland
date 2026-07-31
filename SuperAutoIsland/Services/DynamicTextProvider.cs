using SuperAutoIsland.Models;

namespace SuperAutoIsland.Services;

public class DynamicTextProvider
{
    private Dictionary<string, string> _textDictionary = new();
    private Dictionary<string, string> _textOldDictionary = new();
    
    public EventHandler<DynamicTextChangedEventArgs>? Changed;
    
    public void SetText(string key, string value)
    {
        if (_textDictionary.TryGetValue(key, out var oldValue))
        {
            _textOldDictionary[key] = oldValue;
        }
        
        _textDictionary[key] = value;
        Changed?.Invoke(this, new DynamicTextChangedEventArgs
        {
            Key = key,
            Value = value
        });
    }
    
    public string? GetText(string key)
    {
        return _textDictionary.GetValueOrDefault(key);
    }
    
    public string? GetTextOldValue(string key)
    {
        return _textOldDictionary.GetValueOrDefault(key);
    }
    
    public void RemoveText(string key)
    {
        _textDictionary.Remove(key);
        _textOldDictionary.Remove(key);
    }
}