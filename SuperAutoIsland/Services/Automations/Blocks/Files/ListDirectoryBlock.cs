using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Models.Data;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Services.Automations.Blocks.Files;

/// <summary>
///     列出文件夹内容（数据积木，返回完整路径的列表）。
/// </summary>
public class ListDirectoryBlock : DataBlockBase
{
    private readonly Logger<ListDirectoryBlock> _logger = new();

    public override string Id => "sai.files.data.listDirectory";
    public override string Name => "列出文件夹内容";
    public override (string, string) Icon => ("列表", FluentIcons.FolderListRegular);
    public override string DataOutput => "Array";
    public override Type SettingsType => typeof(ListDirectorySettings);

    public override string Tooltip =>
        "列出文件夹里的文件和子文件夹，返回完整路径的列表（按路径排序）。\n"
        + "相对路径以 SAI 配置目录下的 files 文件夹为根，绝对路径原样使用，路径里的环境变量会被展开。\n"
        + $"系统文件与符号链接 / 联接会被跳过；递归时读不了的子文件夹直接忽略；最多返回 {FileBlockHelpers.MaxListEntries} 条，超出会截断并记日志。\n"
        + "文件夹不存在时返回空列表，原因见运行日志。";

    public override void GetFields(FieldsRegister it)
    {
        it
            .AddField("Path", BasicFields.Text("文件夹路径", "logs"))
            .AddField("Scope", BasicFields.Dropdown("内容", [("文件和文件夹", "all"), ("仅文件", "files"), ("仅文件夹", "directories")]))
            .AddField("Recursive", BasicFields.CheckBox("包含子文件夹"));
    }

    public override async Task<object> Handler(object? data)
    {
        if (data is not ListDirectorySettings settings)
        {
            return Array.Empty<string>();
        }

        try
        {
            var path = FileBlockHelpers.ResolvePath(settings.Path);
            var scope = settings.Scope?.ToLowerInvariant() switch
            {
                "files"       => DirectoryListScope.Files,
                "directories" => DirectoryListScope.Directories,
                _             => DirectoryListScope.All
            };

            // Handler 是在 UI 线程上跑的，目录枚举挪到线程池
            return await Task.Run(() =>
            {
                var entries = FileBlockHelpers.ListEntries(path, scope, settings.Recursive, out var truncated);
                if (truncated)
                {
                    _logger.Warn($"文件夹内容超过 {FileBlockHelpers.MaxListEntries} 条，已截断：{path}");
                }

                return entries.ToArray();
            });
        }
        catch (Exception e)
        {
            _logger.FormatException(e);
            _logger.Warn($"列出文件夹内容失败：{settings.Path}");
            return Array.Empty<string>();
        }
    }
}
