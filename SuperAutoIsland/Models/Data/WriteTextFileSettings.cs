namespace SuperAutoIsland.Models.Data;

/// <summary>
///     「写入文本文件」积木的设置。
/// </summary>
public class WriteTextFileSettings
{
    /// <summary>
    ///     文件路径，相对路径以插件配置目录为根。
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    ///     要写入的文本。
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    ///     写入方式：<c>overwrite</c> 覆盖原内容，<c>append</c> 追加到末尾。
    /// </summary>
    public string Mode { get; set; } = "overwrite";

    /// <summary>
    ///     文件所在文件夹不存在时是否自动创建。
    /// </summary>
    public bool CreateDirectory { get; set; } = true;
}
