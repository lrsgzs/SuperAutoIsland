using SuperAutoIsland.Interface.Metadata;

namespace SuperAutoIsland.Interface.Services.Automations;

public abstract class DataBlockBase : BlockBase
{
    public override BlockKind Kind => BlockKind.Data;
    public virtual Type SettingsType => typeof(object);
    
    /// <summary>
    /// 会在 ui 线程运行，无需 Dispatcher
    /// </summary>
    /// <param name="data">设置对象</param>
    /// <returns>运行结果。请和 DataOutput 指定内容相匹配</returns>
    public virtual Task<object> Handler(object? data)
    {
        return Task.FromResult<object>("???");
    }
}