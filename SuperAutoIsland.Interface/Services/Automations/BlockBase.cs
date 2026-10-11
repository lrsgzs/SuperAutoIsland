using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Metadata;

namespace SuperAutoIsland.Interface.Services.Automations;

/// <summary>
///     请不要在客户端直接继承此类！！！！
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

    /// <summary>
    ///     数据块的输出结果类型（JS 类型名），用于前端 Blockly 的类型检查。
    /// </summary>
    /// <remarks>
    ///     输出 <c>Array</c> 时请返回真正的数组（例如 <c>string[]</c> / <c>object[]</c>）：
    ///     脚本跑在 Jint 上，<c>List&lt;T&gt;</c> 会被包装成 CLR 对象，虽然能取下标、读 <c>length</c>，
    ///     但 <c>.slice()</c> / <c>.sort()</c> 这类数组方法会失效（列表积木会直接报错）。
    /// </remarks>
    public virtual string DataOutput => "String";

    public virtual void GetFields(FieldsRegister it) { }
}