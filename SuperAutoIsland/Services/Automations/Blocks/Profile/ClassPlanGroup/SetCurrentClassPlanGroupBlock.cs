using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlanGroup;

public class SetCurrentClassPlanGroupBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.setCurrentClassPlanGroup";
    public override string Name => "切换课表群";
    public override void GetFields(FieldsRegister it) => it.AddField("Group", ProfileFields.ClassPlanGroup(""));
    public override Task Handler(ActionItem actionItem)
    {
        var s = JsonSerializer.SerializeToElement(actionItem.Settings);
        var profile = IAppHost.GetService<IProfileService>().Profile;
        var id = ProfileBlockHelpers.Guid(s, "Group");
        if (profile.ClassPlanGroups.ContainsKey(id))
            profile.SelectedClassPlanGroupId = id;
        return Task.CompletedTask;
    }
}
