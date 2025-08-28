using System.Text.Json.Serialization;
using SuperAutoIsland.Interface.MetaData;

namespace SuperAutoIsland.Interface;

public class RegisterData
{
    [JsonPropertyName("actions")]
    public required List<BlockMetadata> Actions { get; set; }
    
    [JsonPropertyName("rules")]
    public required List<BlockMetadata> Rules { get; set; }
}