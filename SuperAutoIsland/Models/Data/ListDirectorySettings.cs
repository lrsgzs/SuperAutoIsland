namespace SuperAutoIsland.Models.Data;

/// <summary>
///     「列出文件夹内容」积木的设置。
/// </summary>
public class ListDirectorySettings
{
    /// <summary>
    ///     文件夹路径，相对路径以插件配置目录为根。
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    ///     枚举范围：<c>all</c> 全部，<c>files</c> 仅文件，<c>directories</c> 仅文件夹。
    /// </summary>
    public string Scope { get; set; } = "all";

    /// <summary>
    ///     是否递归子文件夹。
    /// </summary>
    public bool Recursive { get; set; }
}
