using System.Text.Json;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlanGroup;

/// <summary>
///     设置课表群名称。
/// </summary>
public class SetClassPlanGroupNameBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.setClassPlanGroupName";
    public override string Name => "设置课表群名称";

    public override void GetFields(FieldsRegister it)
    {
        it
            .AddField("ClassPlanGroup", ProfileFields.ClassPlanGroup(""))
            .AddField("Name", BasicFields.Text("名称"));
    }

    public override Task Handler(ActionItem actionItem)
    {
        var settings = JsonSerializer.SerializeToElement(actionItem.Settings);
        var group = ProfileBlockHelpers.ClassPlanGroup(settings);
        if (group != null)
        {
            group.Name = settings.GetProperty("Name").GetString() ?? string.Empty;
        }

        return Task.CompletedTask;
    }
}