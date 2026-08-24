using System.Text.Json;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

public class SetClassPlanTimeLayoutBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.setClassPlanTimeLayout";
    public override string Name => "设置时间表";
    
    public override void GetFields(FieldsRegister it) => it
        .AddField("ClassPlan", ProfileFields.ClassPlan(""))
        .AddField("TimeLayout", ProfileFields.TimeLayout("时间表"));
    
    public override Task Handler(ActionItem actionItem)
    {
        var s = JsonSerializer.SerializeToElement(actionItem.Settings);
        var plan = ProfileBlockHelpers.ClassPlan(s);
        if (plan != null) plan.TimeLayoutId = ProfileBlockHelpers.Guid(s, "TimeLayout");
        return Task.CompletedTask;
    }
}
