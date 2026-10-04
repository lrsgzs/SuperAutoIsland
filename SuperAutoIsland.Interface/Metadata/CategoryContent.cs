namespace SuperAutoIsland.Interface.Metadata;

/// <summary>
///     一个分类的完整内容：分类元数据 + 分类下的积木。
///     也是发送给前端的结构（前端据此建立 Blockly 分类，包括名称与图标）。
/// </summary>
/// <param name="metadata">分类元数据</param>
/// <param name="blocks">分类下的积木元数据</param>
public class CategoryContent(CategoryMetadata metadata, List<BlockMetadata>? blocks = null)
{
    /// <summary>
    ///     分类元数据
    /// </summary>
    public CategoryMetadata Metadata { get; set; } = metadata;

    /// <summary>
    ///     分类下的积木元数据
    /// </summary>
    public List<BlockMetadata> Blocks { get; set; } = blocks ?? [];
}
