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
    public override MetaArgsBase Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException("This converter is for serialization only");
    }

    public override void Write(Utf8JsonWriter writer, MetaArgsBase value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        
        // 写入名称
        writer.WriteStringValue(value.Name);
        
        // 写入类型（小写）
        writer.WriteStringValue(value.Type.ToString().ToLower());
        
        // 根据类型写入额外数据
        switch (value)
        {
            case DropDownMetaArgs dropdown:
                // 写入选项数组
                writer.WriteStartArray();
                foreach (var option in dropdown.Options)
                {
                    writer.WriteStartArray();
                    writer.WriteStringValue(option.Item1);
                    writer.WriteStringValue(option.Item2);
                    writer.WriteEndArray();
                }
                writer.WriteEndArray();
                break;
                
            case CheckboxMetaArgs checkbox when checkbox.DefaultValue.HasValue:
                writer.WriteBooleanValue(checkbox.DefaultValue.Value);
                break;
        }
        
        writer.WriteEndArray();
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
