using System.Text.Json;
using ClassIsland.Core.Icons;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Models.Data;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Services.Automations.Blocks.Files;

/// <summary>
///     写入文本文件（行动积木）。
/// </summary>
public class WriteTextFileBlock : ActionBlockBase
{
    private readonly Logger<WriteTextFileBlock> _logger = new();

    public override string Id => "sai.files.actions.writeText";
    public override string Name => "写入文本文件";
    public override (string, string) Icon => ("写入", FluentIcons.DocumentSaveRegular);

    public override string Tooltip =>
        "把文本写入文件（UTF-8，不带 BOM）。\n"
        + "相对路径以 SAI 配置目录下的 files 文件夹为根，绝对路径原样使用，路径里的环境变量会被展开。\n"
        + "写入失败只记日志，不会中断脚本。\n"
        + "本分类可以读写本机任意文件，请勿运行来源不明的 Blockly 项目。";

    public override void GetFields(FieldsRegister it)
    {
        it
            .AddField("Path", BasicFields.Text("路径", "notes.txt"))
            .AddField("Content", BasicFields.Text("内容"))
            .AddField("Mode", BasicFields.Dropdown("写入方式", [("覆盖原内容", "overwrite"), ("追加到末尾", "append")]))
            .AddField("CreateDirectory", BasicFields.CheckBox("自动创建所在文件夹", true));
    }

    public override async Task Handler(ActionItem actionItem)
    {
        WriteTextFileSettings settings;
        try
        {
            settings = JsonSerializer.SerializeToElement(actionItem.Settings)
                                     .Deserialize<WriteTextFileSettings>() ?? new WriteTextFileSettings();
        }
        catch (Exception e)
        {
            _logger.FormatException(e);
            _logger.Warn("写入文本文件的设置无法解析，已跳过本次写入");
            return;
        }

        try
        {
            var path = FileBlockHelpers.ResolvePath(settings.Path);
            var append = string.Equals(settings.Mode, "append", StringComparison.OrdinalIgnoreCase);
            var content = settings.Content ?? string.Empty;
            var createDirectory = settings.CreateDirectory;

            await Task.Run(() => FileBlockHelpers.WriteText(path, content, append, createDirectory));
            _logger.Debug($"已写入文本文件 {path}（{(append ? "追加" : "覆盖")}，{content.Length} 字符）");
        }
        catch (Exception e)
        {
            _logger.FormatException(e);
            _logger.Warn($"写入文本文件失败：{settings.Path}");
        }
    }
}
