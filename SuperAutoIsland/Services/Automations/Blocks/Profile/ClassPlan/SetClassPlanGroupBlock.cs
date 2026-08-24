using System.Text.Json;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

public class SetClassPlanGroupBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.setClassPlanGroup";
    public override string Name => "设置课表群";
    public override void GetFields(FieldsRegister it) => it
        .AddField("ClassPlan", ProfileFields.ClassPlan(""))
        .AddField("Group", ProfileFields.ClassPlanGroup("课表群"));
    public override Task Handler(ActionItem actionItem)
    {
        var s = JsonSerializer.SerializeToElement(actionItem.Settings);
        var plan = ProfileBlockHelpers.ClassPlan(s);
        if (plan != null) plan.AssociatedGroup = ProfileBlockHelpers.Guid(s, "Group");
        return Task.CompletedTask;
    }
}
