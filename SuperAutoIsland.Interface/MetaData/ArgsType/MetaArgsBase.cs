using System.Text.Json.Serialization;

namespace SuperAutoIsland.Interface.MetaData.ArgsType;

/// <summary>
/// 基类元数据
/// </summary>
public abstract class MetaArgsBase
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("type")]
    public required MetaType Type { get; set; }
}