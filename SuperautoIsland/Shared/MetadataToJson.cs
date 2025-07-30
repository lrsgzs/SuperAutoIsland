using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using SuperAutoIsland.Interface.Shared;

namespace SuperAutoIsland.Shared;

public static class MetadataGenerator
{
    public static string GenerateMetaBlock(BlockMetadata data)
    {
        // 配置 JSON 序列化选项
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new MetaArgsConverter() } // 添加自定义转换器
        };

        // 添加枚举字符串转换器
        options.Converters.Add(new JsonStringEnumConverter());

        try
        {
            // 序列化对象
            return JsonSerializer.Serialize(data, options);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Failed to generate metadata JSON", ex);
        }
    }
}

// 自定义转换器处理多态类型
public class MetaArgsConverter : JsonConverter<MetaArgsBase>
{
    public override MetaArgsBase Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException("This converter is for serialization only");
    }

    public override void Write(Utf8JsonWriter writer, MetaArgsBase value, JsonSerializerOptions options)
    {
        // 根据实际类型进行序列化
        switch (value)
        {
            case CommonMetaArgs common:
                JsonSerializer.Serialize(writer, common, options);
                break;
                
            case DropDownMetaArgs dropdown:
                JsonSerializer.Serialize(writer, dropdown, options);
                break;
                
            case CheckboxMetaArgs checkbox:
                JsonSerializer.Serialize(writer, checkbox, options);
                break;
                
            default:
                throw new JsonException($"Unknown MetaArgs type: {value.GetType()}");
        }
    }
}

public class TupleConverter : JsonConverter<ValueTuple<string, string>>
{
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

    public override void Write(Utf8JsonWriter writer, (string, string) value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        writer.WriteStringValue(value.Item1);
        writer.WriteStringValue(value.Item2);
        writer.WriteEndArray();
    }
}
