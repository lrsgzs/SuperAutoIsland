using System.Text.Json;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Models.Actions;

namespace SuperAutoIsland.Services.Automations.Blocks.ClassIsland;

public class ClassIslandRunProgramBlock : ActionBlockBase
{
    public override string Id => "classisland.os.run.program";
    public override string Name => "运行程序";
    public override (string, string) Icon => ("窗口集", "\uF4B1");

    public override void GetFields(FieldsRegister it) => it
        .AddField("Value", BasicFields.Text(""))
        .AddField("Args", BasicFields.Text("应用程序启动参数"));

    public override ActionItem Wrapper(ActionItem actionItem)
    {
        var settingsJson = JsonSerializer.Serialize(actionItem.Settings);
        var settings = JsonSerializer.Deserialize<RunActionSettings>(settingsJson)!;
        settings.RunType = RunActionSettings.RunActionRunType.Application;

        return new ActionItem
        {
            Id = "classisland.os.run",
            Settings = JsonSerializer.Deserialize<object>(JsonSerializer.Serialize(settings))
        };
    }
}
