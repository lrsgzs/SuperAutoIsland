using System.Text.Json;
using System.Text.Json.Serialization;

namespace SuperAutoIsland.Shared;

public class StringTupleConverter : JsonConverter<ValueTuple<string, string>>
{
    /// <inheritdoc />
    public override (string, string) Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException("Expected array start");
        
        reader.Read();
        var item1 = reader.GetString()!;
        reader.Read();
        var item2 = reader.GetString()!;
        reader.Read(); // Consume end array
        
        return (item1, item2);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, (string, string) value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        writer.WriteStringValue(value.Item1);
        writer.WriteStringValue(value.Item2);
        writer.WriteEndArray();
    }
}