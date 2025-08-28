using System.Text.Json.Serialization;

namespace SuperAutoIsland.Interface.MetaData;

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