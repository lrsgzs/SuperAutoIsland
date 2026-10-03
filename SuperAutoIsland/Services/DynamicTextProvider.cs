using Avalonia.Media;
using SuperAutoIsland.Models;

namespace SuperAutoIsland.Services;

public class DynamicTextProvider
{
    private readonly Dictionary<string, Color?> _oldColorValues = new();
    private readonly Dictionary<string, string?> _oldIconValues = new();

    private readonly Dictionary<string, string?> _oldTextValues = new();
    private readonly Dictionary<string, DynamicTextItem> _textDictionary = new();
    private readonly Dictionary<string, DynamicTextItem> _textOldDictionary = new();

    public EventHandler<DynamicTextChangedEventArgs>? Changed;

    public void SetText(string key, string? value)
    {
        var item = BeginUpdate(key);
        _oldTextValues[key] = item.Text;
        item.Text = value;
        NotifyChanged(key, item);
    }

    public void SetIcon(string key, string? value)
    {
        var item = BeginUpdate(key);
        _oldIconValues[key] = item.Icon;
        item.Icon = value;
        NotifyChanged(key, item);
    }

    public void SetColor(string key, Color? color)
    {
        var item = BeginUpdate(key);
        _oldColorValues[key] = item.Color;
        item.Color = color;
        NotifyChanged(key, item);
    }

    public void RevertText(string key)
    {
        if (!_oldTextValues.Remove(key, out var oldValue) ||
            !_textDictionary.TryGetValue(key, out var item))
        {
            return;
        }

        item.Text = oldValue;
        NotifyOrRemove(key, item);
    }

    public void RevertIcon(string key)
    {
        if (!_oldIconValues.Remove(key, out var oldValue) ||
            !_textDictionary.TryGetValue(key, out var item))
        {
            return;
        }

        item.Icon = oldValue;
        NotifyOrRemove(key, item);
    }

    public void RevertColor(string key)
    {
        if (!_oldColorValues.Remove(key, out var oldValue) ||
            !_textDictionary.TryGetValue(key, out var item))
        {
            return;
        }

        item.Color = oldValue;
        NotifyOrRemove(key, item);
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
            _textOldDictionary[key] = oldValue.Clone();
        }
        else
        {
            _textOldDictionary.Remove(key);
        }

        _textDictionary[key] = item;
        NotifyChanged(key, item);
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
        var removed = _textDictionary.Remove(key);
        _textOldDictionary.Remove(key);
        _oldTextValues.Remove(key);
        _oldIconValues.Remove(key);
        _oldColorValues.Remove(key);

        if (removed)
        {
            NotifyChanged(key, null);
        }
    }

    private DynamicTextItem BeginUpdate(string key)
    {
        if (!_textDictionary.TryGetValue(key, out var item))
        {
            item = new DynamicTextItem();
            _textDictionary[key] = item;
            _textOldDictionary.Remove(key);
            return item;
        }

        _textOldDictionary[key] = item.Clone();
        return item;
    }

    private void NotifyOrRemove(string key, DynamicTextItem item)
    {
        if (item.Text == null && item.Icon == null && item.Color == null)
        {
            RemoveText(key);
            return;
        }

        NotifyChanged(key, item);
    }

    private void NotifyChanged(string key, DynamicTextItem? item)
    {
        Changed?.Invoke(this, new DynamicTextChangedEventArgs
        {
            Key = key,
            Value = item
        });
    }
}