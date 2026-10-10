using Jint.WebApi;

namespace SuperAutoIsland.Services;

/// <summary>
///     脚本 <c>localStorage</c> 的存储后端。
/// </summary>
/// <remarks>
///     <para>
///         <c>sessionStorage</c> 不走这里，用 Jint 自带的 <see cref="InMemoryStorageProvider" />，
///         每个引擎一份、脚本跑完即丢。
///     </para>
/// </remarks>
/// <param name="items">实际的键值数据（来自主配置）</param>
/// <param name="persist">把数据写回配置文件（由主配置的保存逻辑实现）</param>
public sealed class ScriptLocalStorageProvider(Dictionary<string, string> items, Action persist) : StorageProvider
{
    private static readonly object Gate = new();

    private bool _isDirty;

    /// <inheritdoc />
    public override string? GetItem(string key)
    {
        lock (Gate)
        {
            return items.TryGetValue(key, out var value) ? value : null;
        }
    }

    /// <inheritdoc />
    public override void SetItem(string key, string value)
    {
        lock (Gate)
        {
            // 写同一个值不算改动，省掉一次落盘
            if (items.TryGetValue(key, out var old) && old == value)
            {
                return;
            }

            items[key] = value;
            _isDirty = true;
        }
    }

    /// <inheritdoc />
    public override void RemoveItem(string key)
    {
        lock (Gate)
        {
            if (items.Remove(key))
            {
                _isDirty = true;
            }
        }
    }

    /// <inheritdoc />
    public override void Clear()
    {
        lock (Gate)
        {
            if (items.Count == 0)
            {
                return;
            }

            items.Clear();
            _isDirty = true;
        }
    }

    /// <inheritdoc />
    public override IReadOnlyList<string> Keys
    {
        get
        {
            lock (Gate)
            {
                return items.Keys.ToList();
            }
        }
    }

    /// <inheritdoc />
    public override int Count
    {
        get
        {
            lock (Gate)
            {
                return items.Count;
            }
        }
    }

    /// <summary>
    ///     有改动时把数据写回配置，没有改动就什么都不做。
    /// </summary>
    /// <remarks>
    ///     在锁外调用 <c>persist</c>：保存会做文件 IO，没必要占着锁。
    /// </remarks>
    public void Flush()
    {
        lock (Gate)
        {
            if (!_isDirty)
            {
                return;
            }

            _isDirty = false;
        }

        persist();
    }
}
