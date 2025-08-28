using System.Text.Json.Serialization;

namespace SuperAutoIsland.Interface.MetaData.ArgsType;

public class DropDownMetaArgs : MetaArgsBase
{
    [JsonPropertyName("options")]
    public required List<Tuple<string, string>> Options { get; set; }
}