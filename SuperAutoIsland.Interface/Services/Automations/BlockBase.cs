using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Metadata;

namespace SuperAutoIsland.Interface.Services.Automations;

/// <summary>
/// 请不要在客户端直接继承此类！！！！
/// </summary>
public abstract class BlockBase
{
    public virtual BlockKind Kind => BlockKind.Label;
    
    public abstract string Id { get; }
    public abstract string Name { get; }
    public virtual (string, string) Icon => ("操作", FluentIcons.SettingsRegular);
    public virtual string Tooltip => string.Empty;
    
    public virtual bool InlineBlock => false;
    public virtual bool InlineField => false;
    public virtual string DataOutput => "String";

    public virtual void GetFields(FieldsRegister it)
    {
    }
}