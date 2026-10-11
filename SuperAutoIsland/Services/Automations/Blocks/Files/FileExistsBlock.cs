using System.Text.Json;
using ClassIsland.Core.Icons;
using ClassIsland.Core.Models.Ruleset;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Models.Data;

namespace SuperAutoIsland.Services.Automations.Blocks.Files;

/// <summary>
///     路径是否存在且是文件（规则积木）。
/// </summary>
public class FileExistsBlock : RuleBlockBase
{
    public override string Id => "sai.files.rules.fileExists";
    public override string Name => "文件存在?";
    public override (string, string) Icon => ("文件", FluentIcons.DocumentQuestionMarkRegular);

    public override string Tooltip =>
        "路径存在且是文件时为真。\n"
        + "相对路径以 SAI 配置目录下的 files 文件夹为根，绝对路径原样使用，路径里的环境变量会被展开；路径非法时按假处理。";

    public override void GetFields(FieldsRegister it)
    {
        it.AddField("Path", BasicFields.Text("路径", "notes.txt"));
    }

    public override bool Handler(Rule rule)
    {
        try
        {
            var settings = JsonSerializer.SerializeToElement(rule.Settings)
                                       .Deserialize<FilePathSettings>() ?? new FilePathSettings();

            // 规则 Handler 是同步的、且跑在 UI 线程上（见 SaiBlockRunner），这里只能同步 IO。
            // 本机路径够快；写死的 UNC / 断掉的映射盘可能卡住 UI，属已知限制。
            return FileBlockHelpers.FileExists(settings.Path);
        }
        catch (Exception)
        {
            return false;
        }
    }
}
