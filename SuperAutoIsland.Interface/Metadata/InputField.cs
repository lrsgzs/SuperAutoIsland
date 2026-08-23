namespace SuperAutoIsland.Interface.Metadata;

/// <summary>
/// 直接实例化该类型，即代表您接受所有可能引入的风险，如 js 生成出来的代码不可运行等。
/// SAI 开发者不对直接实例化此类型造成的后果负责。
/// </summary>
public class InputField : Field
{
    public string Check { get; set; } = "String";
    public string? ShadowBlockType { get; set; }
}