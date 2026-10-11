using System.Text.Json;
using ClassIsland.Core.Icons;
using ClassIsland.Core.Models.Ruleset;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Models.Data;

namespace SuperAutoIsland.Services.Automations.Blocks.Files;

/// <summary>
///     路径是否存在且是文件夹（规则积木）。
/// </summary>
public class DirectoryExistsBlock : RuleBlockBase
{
    public override string Id => "sai.files.rules.directoryExists";
    public override string Name => "是否为文件夹?";
    public override (string, string) Icon => ("文件夹", FluentIcons.FolderOpenRegular);

    public override string Tooltip =>
        "路径存在且是文件夹时为真（是文件时为假）。\n"
        + "相对路径以 SAI 配置目录下的 files 文件夹为根，绝对路径原样使用，路径里的环境变量会被展开；路径非法时按假处理。";

    public override void GetFields(FieldsRegister it)
    {
        it.AddField("Path", BasicFields.Text("路径", "logs"));
    }

    public override bool Handler(Rule rule)
    {
        try
        {
            var settings = JsonSerializer.SerializeToElement(rule.Settings)
                                       .Deserialize<FilePathSettings>() ?? new FilePathSettings();

            // 同 FileExistsBlock：规则 Handler 同步且跑在 UI 线程上，UNC / 掉线的映射盘可能卡 UI。
            return FileBlockHelpers.DirectoryExists(settings.Path);
        }
        catch (Exception)
        {
            return false;
        }
    }
}
