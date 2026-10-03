using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlanGroup;

/// <summary>
///     解散指定课表群。群内的课表会被移动到默认课表群。
/// </summary>
public class DisbandClassPlanGroupBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.disbandClassPlanGroup";
    public override string Name => "解散课表群";
    public override string Tooltip => "解散指定课表群，群内的课表会被移动到默认课表群。默认课表群和全局课表群无法解散。";

    public override void GetFields(FieldsRegister it)
    {
        it
            .AddField("ClassPlanGroup", ProfileFields.ClassPlanGroup(""));
    }

    public override Task Handler(ActionItem actionItem)
    {
        var settings = JsonSerializer.SerializeToElement(actionItem.Settings);
        var profile = IAppHost.GetService<IProfileService>().Profile;
        var id = ProfileBlockHelpers.Guid(settings, "ClassPlanGroup");
        if (id == Guid.Empty || !profile.ClassPlanGroups.ContainsKey(id))
        {
            return Task.CompletedTask;
        }

        try
        {
            profile.DisbandClassPlanGroup(id);
        }
        catch (ArgumentException)
        {
            // 默认课表群和全局课表群无法解散
            return Task.CompletedTask;
        }

        ProfileBlockHelpers.ClearClassPlanGroupReferences(id);
        return Task.CompletedTask;
    }
}