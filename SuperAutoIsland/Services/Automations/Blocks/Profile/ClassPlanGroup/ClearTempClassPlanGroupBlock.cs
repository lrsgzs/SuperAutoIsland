using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Services.Automations;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlanGroup;

public class ClearTempClassPlanGroupBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.clearTempClassPlanGroup";
    public override string Name => "清除临时课表群";
    public override Task Handler(ActionItem actionItem)
    {
        IAppHost.GetService<IProfileService>().ClearTempClassPlanGroup();
        return Task.CompletedTask;
    }
}
