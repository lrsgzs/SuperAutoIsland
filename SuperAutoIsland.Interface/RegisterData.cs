using System.Text.Json.Serialization;
using SuperAutoIsland.Interface.Metadata;

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
    public List<BlockMetadata> Actions { get; set; } = [];
    
    /// <summary>
    /// 规则
    /// </summary>
    [JsonPropertyName("rules")]
    public List<BlockMetadata> Rules { get; set; } = [];
    
    /// <summary>
    /// 数据
    /// </summary>
    [JsonPropertyName("data")]
    public List<BlockMetadata> Data { get; set; } = [];
}