using System.Text.Json.Serialization;
using ClassIsland.Core.Icons;

namespace SuperAutoIsland.Interface.Metadata;

public class BlockMetadata(string id)
{
    public required BlockKind Kind { get; set; }
    
    public string Id { get; set; } = id;
    public required string Name { get; set; }
    public (string, string) Icon { get; set; } = ("操作", FluentIcons.SettingsRegular);
    public string Tooltip { get; set; } = string.Empty;

    [JsonConverter(typeof(PolymerDictionaryConverter<string, Field>))]
    public Dictionary<string, Field> Fields { get; set; } = [];

    public bool InlineBlock { get; set; } = false;
    public bool InlineField { get; set; } = false;
    public string DataOutput { get; set; } = "String";
}