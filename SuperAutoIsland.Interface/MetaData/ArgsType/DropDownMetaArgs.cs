using System.Text.Json.Serialization;

namespace SuperAutoIsland.Interface.MetaData.ArgsType;

/// <summary>
/// 下拉框元数据
/// </summary>
public class DropDownMetaArgs : MetaArgsBase
{
    [JsonPropertyName("options")]
    public required List<(string, string)> Options { get; set; }
}