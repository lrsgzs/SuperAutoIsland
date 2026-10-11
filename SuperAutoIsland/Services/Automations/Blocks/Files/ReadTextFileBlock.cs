using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Models.Data;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Services.Automations.Blocks.Files;

/// <summary>
///     读取文本文件（数据积木）。
/// </summary>
public class ReadTextFileBlock : DataBlockBase
{
    private readonly Logger<ReadTextFileBlock> _logger = new();

    public override string Id => "sai.files.data.readText";
    public override string Name => "读取文本文件";
    public override (string, string) Icon => ("读取", FluentIcons.DocumentTextRegular);
    public override string DataOutput => "String";
    public override Type SettingsType => typeof(FilePathSettings);

    public override string Tooltip =>
        "读取文本文件的内容（UTF-8，自动识别 BOM；非 UTF-8 的 ANSI / GBK 文件会乱码）。\n"
        + "相对路径以 SAI 配置目录下的 files 文件夹为根，绝对路径原样使用，路径里的环境变量会被展开。\n"
        + "读取失败（不存在、被占用、超过 8 MiB 等）时返回空文本，原因见运行日志。\n"
        + "本分类可以读写本机任意文件，请勿运行来源不明的 Blockly 项目。";

    public override void GetFields(FieldsRegister it)
    {
        it.AddField("Path", BasicFields.Text("路径", "notes.txt"));
    }

    public override async Task<object> Handler(object? data)
    {
        if (data is not FilePathSettings settings)
        {
            return string.Empty;
        }

        try
        {
            var path = FileBlockHelpers.ResolvePath(settings.Path);
            // Handler 是在 UI 线程上跑的，文件 IO 挪到线程池
            return await Task.Run(() => FileBlockHelpers.ReadText(path));
        }
        catch (Exception e)
        {
            _logger.FormatException(e);
            _logger.Warn($"读取文本文件失败：{settings.Path}");
            return string.Empty;
        }
    }
}
