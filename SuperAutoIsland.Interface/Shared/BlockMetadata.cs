using System.Text.Json.Serialization;

namespace SuperAutoIsland.Interface.Shared;

// 类型定义（与之前相同）
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

public abstract class MetaArgsBase
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("type")]
    public required MetaType Type { get; set; }
}

public class CommonMetaArgs : MetaArgsBase
{
    // 没有额外属性
}

public class DropDownMetaArgs : MetaArgsBase
{
    [JsonPropertyName("options")]
    public required List<Tuple<string, string>> Options { get; set; }
}

public class CheckboxMetaArgs : MetaArgsBase
{
    [JsonPropertyName("defaultValue")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? DefaultValue { get; set; }
}

public class BlockMetadata
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("icon")]
    public required Tuple<string, string> Icon { get; set; }

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