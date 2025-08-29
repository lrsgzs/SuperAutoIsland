using System.Text.Json.Serialization;

namespace SuperAutoIsland.Interface.MetaData.ArgsType;

/// <summary>
/// 复选框元数据
/// </summary>
public class CheckboxMetaArgs : MetaArgsBase
{
    [JsonPropertyName("defaultValue")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? DefaultValue { get; set; }
}