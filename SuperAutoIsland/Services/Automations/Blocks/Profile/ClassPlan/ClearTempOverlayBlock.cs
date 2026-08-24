using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Services.Automations;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

public class ClearTempOverlayBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.clearTempOverlay";
    public override string Name => "清除临时层";
    public override Task Handler(ActionItem actionItem)
    {
        IAppHost.GetService<IProfileService>().ClearTempClassPlan();
        return Task.CompletedTask;
    }
}
