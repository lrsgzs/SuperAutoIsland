using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

public class DeleteClassPlanBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.deleteClassPlan";
    public override string Name => "删除课表";
    public override void GetFields(FieldsRegister it) => it.AddField("ClassPlan", ProfileFields.ClassPlan(""));
    public override Task Handler(ActionItem actionItem)
    {
        var s = JsonSerializer.SerializeToElement(actionItem.Settings);
        var profile = IAppHost.GetService<IProfileService>().Profile;
        var id = ProfileBlockHelpers.Guid(s, "ClassPlan");
        profile.ClassPlans.Remove(id);

        foreach (var date in profile.OrderedSchedules
                     .Where(x => x.Value.ClassPlanId == id)
                     .Select(x => x.Key)
                     .ToList())
        {
            profile.OrderedSchedules.Remove(date);
        }

        if (profile.TempClassPlanId == id)
            profile.TempClassPlanId = null;

        if (profile.OverlayClassPlanId == id)
        {
            profile.OverlayClassPlanId = null;
            profile.IsOverlayClassPlanEnabled = false;
        }

        return Task.CompletedTask;
    }
}
