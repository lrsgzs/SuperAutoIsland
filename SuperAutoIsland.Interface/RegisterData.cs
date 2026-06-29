using System.Text.Json.Serialization;
using SuperAutoIsland.Interface.MetaData;

namespace SuperAutoIsland.Interface;

/// <summary>
/// 注册数据
/// </summary>
public class RegisterData
{
    /// <summary>
    /// 行动
    /// </summary>
    [JsonPropertyName("actions")]
    public required List<BlockMetadata> Actions { get; set; }
    
    /// <summary>
    /// 规则
    /// </summary>
    [JsonPropertyName("rules")]
    public required List<BlockMetadata> Rules { get; set; }
    
    /// <summary>
    /// 数据
    /// </summary>
    [JsonPropertyName("data")]
    public required List<BlockMetadata> Data { get; set; }
}