using System.Text.Json;
using System.Text.Json.Serialization;
using SuperAutoIsland.Interface.MetaData;
using SuperAutoIsland.Interface.MetaData.ArgsType;

namespace SuperAutoIsland.Shared;

/// <summary>
/// 元数据 json 生成器（为什么会有这个？）
/// </summary>
public static class MetadataGenerator
{
    /// <summary>
    /// 生成 json
    /// </summary>
    /// <param name="data">积木元数据</param>
    /// <returns>积木 json</returns>
    /// <exception cref="InvalidOperationException">无效操作错误</exception>
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

/// <summary>
/// 自定义转换器处理多态类型
/// </summary>
public class MetaArgsConverter : JsonConverter<MetaArgsBase>
{
    /// <inheritdoc />
    public override MetaArgsBase Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException("This converter is for serialization only");
    }

    /// <inheritdoc />
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


/// <summary>
/// 元组转换器，好像不工作？不知道啊
/// </summary>
public class TupleConverter : JsonConverter<ValueTuple<string, string>>
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
