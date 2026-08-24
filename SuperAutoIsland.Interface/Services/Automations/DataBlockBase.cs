using SuperAutoIsland.Interface.Metadata;

namespace SuperAutoIsland.Interface.Services.Automations;

public abstract class DataBlockBase : BlockBase
{
    public override BlockKind Kind => BlockKind.Data;
    public virtual Type SettingsType => typeof(object);
    
    public virtual Task<object> Handler(object? data)
    {
        return Task.FromResult<object>("???");
    }
}