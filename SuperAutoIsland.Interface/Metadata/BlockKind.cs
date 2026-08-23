using System.Text.Json.Serialization;

namespace SuperAutoIsland.Interface.Metadata;

[JsonConverter(typeof(JsonStringEnumConverter<BlockKind>))]
public enum BlockKind
{
    [JsonStringEnumMemberName("action")] Action,
    [JsonStringEnumMemberName("text")] Rule,
    [JsonStringEnumMemberName("data")] Data,
    [JsonStringEnumMemberName("label")] Label
}