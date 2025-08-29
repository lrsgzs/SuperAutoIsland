using System.Text.Json.Serialization;

namespace SuperAutoIsland.Interface.MetaData;

/// <summary>
/// 参数类型
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MetaType
{
    dummy,
    text,
    number,
    boolean,
    dropdown,
    checkbox
}