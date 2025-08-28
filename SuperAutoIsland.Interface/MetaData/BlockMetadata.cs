using System.Text.Json.Serialization;
using SuperAutoIsland.Interface.MetaData.ArgsType;

namespace SuperAutoIsland.Interface.MetaData;

// 类型定义（与之前相同） 

public class BlockMetadata
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("icon")]
    public required (string, string) Icon { get; set; }

    [JsonPropertyName("args")]
    public required Dictionary<string, MetaArgsBase> Args { get; set; }

    [JsonPropertyName("inlineBlock")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? InlineBlock { get; set; }

    [JsonPropertyName("inlineField")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? InlineField { get; set; }

    [JsonPropertyName("dropdownUseNumbers")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? DropdownUseNumbers { get; set; }

    [JsonPropertyName("isRule")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsRule { get; set; }
}