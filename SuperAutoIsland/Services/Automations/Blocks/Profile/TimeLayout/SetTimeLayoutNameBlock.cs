using System.Text.Json;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.TimeLayout;

/// <summary>
/// 设置时间表名称。
/// </summary>
public class SetTimeLayoutNameBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.setTimeLayoutName";
    public override string Name => "设置时间表名称";

    public override void GetFields(FieldsRegister it) => it
        .AddField("TimeLayout", ProfileFields.TimeLayout(""))
        .AddField("Name", BasicFields.Text("名称"));

    public override Task Handler(ActionItem actionItem)
    {
        var settings = JsonSerializer.SerializeToElement(actionItem.Settings);
        var timeLayout = ProfileBlockHelpers.ClassicTimeLayout(settings);
        if (timeLayout != null)
        {
            timeLayout.Name = settings.GetProperty("Name").GetString() ?? string.Empty;
        }

        return Task.CompletedTask;
    }
}
