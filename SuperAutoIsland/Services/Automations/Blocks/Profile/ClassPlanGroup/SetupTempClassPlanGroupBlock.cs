using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlanGroup;

public class SetupTempClassPlanGroupBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.setupTempClassPlanGroup";
    public override string Name => "设置临时课表群";
    public override void GetFields(FieldsRegister it) => it.AddField("Group", ProfileFields.ClassPlanGroup(""));
    public override Task Handler(ActionItem actionItem)
    {
        var s = JsonSerializer.SerializeToElement(actionItem.Settings);
        var profileService = IAppHost.GetService<IProfileService>();
        var id = ProfileBlockHelpers.Guid(s, "Group");
        if (profileService.Profile.ClassPlanGroups.ContainsKey(id))
            profileService.SetupTempClassPlanGroup(id);
        return Task.CompletedTask;
    }
}
