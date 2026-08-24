using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Services.Automations;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

public class SaveProfileBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.saveProfile";
    public override string Name => "保存档案";

    public override Task Handler(ActionItem actionItem)
    {
        IAppHost.GetService<IProfileService>().SaveProfile();
        return Task.CompletedTask;
    }
}
