using System.Text.Json.Serialization;

namespace SuperAutoIsland.Interface.MetaData.ArgsType;

/// <summary>
/// 动态下拉框元数据
/// </summary>
public class DynamicDropdownMetaArgs : MetaArgsBase
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }
}