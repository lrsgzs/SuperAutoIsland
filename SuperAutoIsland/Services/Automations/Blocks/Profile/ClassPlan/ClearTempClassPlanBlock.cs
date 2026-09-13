using SuperAutoIsland.Interface.Services.Automations;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using ClassIsland.Shared.Models.Automation;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

public class ClearTempClassPlanBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.clearTempClassPlan";
    public override string Name => "清除临时课表";
    public override Task Handler(ActionItem actionItem)
    {
        IAppHost.GetService<IProfileService>().Profile.TempClassPlanId = null;
        return Task.CompletedTask;
    }
}
