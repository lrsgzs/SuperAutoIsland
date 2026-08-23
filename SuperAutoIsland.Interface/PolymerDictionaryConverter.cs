using System.Text.Json;
using System.Text.Json.Serialization;

namespace SuperAutoIsland.Interface;

public class PolymerDictionaryConverter<TKey, TValue> : JsonConverter<Dictionary<TKey, TValue>> where TKey : notnull
{
    public override Dictionary<TKey, TValue> Read(
        ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotSupportedException("此转换器仅用于序列化输出。若需反序列化还原子类，请在 TBase 上标记 [JsonDerivedType]。");
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<TKey, TValue> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (var kvp in value)
        {
            writer.WritePropertyName(kvp.Key.ToString() ?? "???");

            if (kvp.Value is null)
            {
                writer.WriteNullValue();
                continue;
            }

            JsonSerializer.Serialize(writer, kvp.Value, kvp.Value.GetType(), options);
        }

        writer.WriteEndObject();
    }
}
