namespace SuperAutoIsland.Models.Data;

/// <summary>
///     只带一个路径的积木设置（读取文件、文件存在、是否为文件夹共用）。
/// </summary>
public class FilePathSettings
{
    /// <summary>
    ///     路径，相对路径以插件配置目录为根。
    /// </summary>
    public string Path { get; set; } = string.Empty;
}
