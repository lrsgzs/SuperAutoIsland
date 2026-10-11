using System.Text;
using SuperAutoIsland.Shared;

namespace SuperAutoIsland.Services.Automations.Blocks.Files;

/// <summary>
///     文件夹列表的枚举范围。
/// </summary>
public enum DirectoryListScope
{
    /// <summary>
    ///     文件和文件夹都要。
    /// </summary>
    All,

    /// <summary>
    ///     只要文件。
    /// </summary>
    Files,

    /// <summary>
    ///     只要文件夹。
    /// </summary>
    Directories
}

/// <summary>
///     「文件」分类积木的公共实现：路径解析、文本文件读写、文件夹枚举。
/// </summary>
/// <remarks>
///     <para>
///         只处理文本（UTF-8；读取时自动识别 BOM）。二进制文件、非 UTF-8（ANSI / GBK）文件请勿用这里的积木读写。
///     </para>
///     <para>
///         相对路径以插件配置目录下的 <c>files</c> 子目录为根（刻意避开 <c>Main.json</c> / <c>Projects.json</c> /
///         <c>Projects</c> / <c>Scripts</c>，免得脚本把插件自己的配置写坏）；绝对路径原样使用，
///         路径里的环境变量（<c>%USERPROFILE%</c> 之类）会被展开。
///     </para>
///     <para>
///         读写都用 <see cref="FileShare.ReadWrite" /> + <see cref="FileShare.Delete" /> 打开：
///         别的程序正开着的日志文件也要能读、能追加（<see cref="File.ReadAllText" /> 的默认共享模式会被这种情况拒绝）。
///     </para>
///     <para>
///         这里的函数都是同步的，异常由各积木自行兜底（见各自 Handler），所以不要直接
///         在 UI 线程上调用：积木的 Handler 是被 <c>SaiBlockRunner</c> 放到 UI 线程执行的，
///         调用方需要自己套 <see cref="Task.Run(Action)" /> 挪到线程池。
///     </para>
/// </remarks>
public static class FileBlockHelpers
{
    /// <summary>
    ///     单次读取的字节上限，挡住了「一不小心读了几个 G 的日志」这种情况。
    /// </summary>
    public const long MaxReadBytes = 8L * 1024 * 1024;

    /// <summary>
    ///     单次列出的条目上限，挡住「递归列一下 C 盘」这种情况。
    /// </summary>
    public const int MaxListEntries = 20_000;

    private static readonly UTF8Encoding Utf8NoBom = new(false);

    /// <summary>
    ///     相对路径的根目录：插件配置目录下的 <c>files</c> 子目录。
    /// </summary>
    /// <exception cref="InvalidOperationException">插件配置目录尚未初始化</exception>
    public static string RootFolder
    {
        get
        {
            if (string.IsNullOrWhiteSpace(GlobalConstants.PluginConfigFolder))
            {
                throw new InvalidOperationException("插件配置目录尚未初始化，无法解析相对路径。");
            }

            return Path.Combine(GlobalConstants.PluginConfigFolder!, "files");
        }
    }

    /// <summary>
    ///     把积木里填的路径解析成绝对路径：相对路径以 <see cref="RootFolder" /> 为根，绝对路径原样使用。
    /// </summary>
    /// <param name="path">积木里填的路径</param>
    /// <exception cref="ArgumentException">路径为空，或是 <c>C:foo</c> 这种盘符相对路径</exception>
    public static string ResolvePath(string? path)
    {
        var trimmed = Normalize(path);
        if (trimmed.Length == 0)
        {
            throw new ArgumentException("路径不能为空。", nameof(path));
        }

        trimmed = Environment.ExpandEnvironmentVariables(trimmed);

        if (Path.IsPathFullyQualified(trimmed))
        {
            return Path.GetFullPath(trimmed);
        }

        // Path.Combine 遇到 "C:foo" 会原样返回，而 GetFullPath 会把它解析到「进程当前目录」，
        // 于是文档里「相对路径挂在 files 目录下」的规则就被绕过去了。这种写法直接拒绝。
        if (Path.IsPathRooted(trimmed))
        {
            throw new ArgumentException("不支持的路径写法（如 C:foo 这种盘符相对路径），请写完整路径或相对路径。", nameof(path));
        }

        return Path.GetFullPath(Path.Combine(RootFolder, trimmed));
    }

    /// <summary>
    ///     读取文本文件内容。
    /// </summary>
    /// <param name="path">绝对路径</param>
    /// <exception cref="IOException">文件过大（超过 <see cref="MaxReadBytes" />）</exception>
    public static string ReadText(string path)
    {
        // 只用一个句柄：先量长度再读，避免文件在两次打开之间长大而绕过上限
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read,
                                          FileShare.ReadWrite | FileShare.Delete);
        if (stream.Length > MaxReadBytes)
        {
            throw new IOException($"文件过大（{stream.Length} 字节，上限 {MaxReadBytes} 字节）：{path}");
        }

        using var reader = new StreamReader(stream, Encoding.UTF8, true);
        return reader.ReadToEnd();
    }

    /// <summary>
    ///     写入文本文件。
    /// </summary>
    /// <param name="path">绝对路径</param>
    /// <param name="content">要写入的文本</param>
    /// <param name="append">true 追加到末尾，false 覆盖原内容</param>
    /// <param name="createDirectory">文件所在文件夹不存在时是否自动创建</param>
    public static void WriteText(string path, string content, bool append, bool createDirectory)
    {
        if (createDirectory && Path.GetDirectoryName(path) is { Length: > 0 } folder)
        {
            Directory.CreateDirectory(folder);
        }

        using var stream = new FileStream(path, append ? FileMode.Append : FileMode.Create, FileAccess.Write,
                                          FileShare.ReadWrite | FileShare.Delete);
        using var writer = new StreamWriter(stream, Utf8NoBom);
        writer.Write(content);
    }

    /// <summary>
    ///     列出文件夹里的内容，返回完整路径。
    /// </summary>
    /// <param name="path">绝对路径</param>
    /// <param name="scope">枚举范围</param>
    /// <param name="recursive">是否递归子文件夹</param>
    /// <param name="truncated">是否因为超过 <see cref="MaxListEntries" /> 而截断</param>
    /// <exception cref="DirectoryNotFoundException">路径不存在，或不是一个文件夹</exception>
    public static List<string> ListEntries(string path, DirectoryListScope scope, bool recursive, out bool truncated)
    {
        var directory = new DirectoryInfo(path);
        if (!directory.Exists)
        {
            throw File.Exists(path)
                      ? new DirectoryNotFoundException($"这是一个文件，不是文件夹：{path}")
                      : new DirectoryNotFoundException($"文件夹不存在：{path}");
        }

        // 系统文件与重解析点（符号链接 / 联接）跳过：前者没意义，后者会让递归打转
        var options = new EnumerationOptions
        {
            RecurseSubdirectories = recursive,
            IgnoreInaccessible = true,
            AttributesToSkip = FileAttributes.System | FileAttributes.ReparsePoint
        };

        IEnumerable<string> entries = scope switch
        {
            DirectoryListScope.Files       => directory.EnumerateFiles("*", options).Select(x => x.FullName),
            DirectoryListScope.Directories => directory.EnumerateDirectories("*", options).Select(x => x.FullName),
            _                              => directory.EnumerateFileSystemInfos("*", options).Select(x => x.FullName)
        };

        var result = new List<string>();
        truncated = false;
        foreach (var entry in entries)
        {
            result.Add(entry);
            if (result.Count >= MaxListEntries)
            {
                truncated = true;
                break;
            }
        }

        // 排序只是为了结果稳定，方便用户写「取第 n 个」这类脚本
        result.Sort(StringComparer.OrdinalIgnoreCase);
        return result;
    }

    /// <summary>
    ///     路径是否存在且是文件。路径非法时返回 false，不抛异常。
    /// </summary>
    public static bool FileExists(string? path)
    {
        try
        {
            return File.Exists(ResolvePath(path));
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    ///     路径是否存在且是文件夹。路径非法时返回 false，不抛异常。
    /// </summary>
    public static bool DirectoryExists(string? path)
    {
        try
        {
            return Directory.Exists(ResolvePath(path));
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    ///     去掉首尾空白；整段被引号包起来时把引号也去掉（粘贴路径的常见写法）。
    /// </summary>
    private static string Normalize(string? path)
    {
        var trimmed = (path ?? string.Empty).Trim();
        if (trimmed.Length >= 2 && trimmed[0] == '"' && trimmed[^1] == '"')
        {
            trimmed = trimmed[1..^1].Trim();
        }

        return trimmed;
    }
}
