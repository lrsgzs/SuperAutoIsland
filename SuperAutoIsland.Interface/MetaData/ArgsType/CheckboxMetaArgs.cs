using System.Text.Json.Serialization;

namespace SuperAutoIsland.Interface.MetaData.ArgsType;

public class CheckboxMetaArgs : MetaArgsBase
{
    [JsonPropertyName("defaultValue")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? DefaultValue { get; set; }
}