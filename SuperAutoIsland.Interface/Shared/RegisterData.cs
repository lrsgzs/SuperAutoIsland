using System.Text.Json.Serialization;

namespace SuperAutoIsland.Interface.Shared;

public class RegisterData
{
    [JsonPropertyName("actions")]
    public required List<BlockMetadata> Actions { get; set; }
    
    [JsonPropertyName("rules")]
    public required List<BlockMetadata> Rules { get; set; }
}