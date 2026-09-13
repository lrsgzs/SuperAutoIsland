using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
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
        var timeLayoutId = ProfileBlockHelpers.Guid(s, "TimeLayout");
        var profile = IAppHost.GetService<IProfileService>().Profile;
        if (plan != null && (timeLayoutId == Guid.Empty || profile.TimeLayouts.ContainsKey(timeLayoutId)))
            plan.TimeLayoutId = timeLayoutId;
        return Task.CompletedTask;
    }
}
