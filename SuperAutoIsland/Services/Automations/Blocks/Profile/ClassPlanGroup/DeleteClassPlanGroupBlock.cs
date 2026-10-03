using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlanGroup;

/// <summary>
/// 删除指定课表群。群内的课表会被一并删除。
/// </summary>
public class DeleteClassPlanGroupBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.deleteClassPlanGroup";
    public override string Name => "删除课表群";
    public override string Tooltip => "删除指定课表群，群内的课表会被一并删除。默认课表群和全局课表群无法删除。";

    public override void GetFields(FieldsRegister it) => it
        .AddField("ClassPlanGroup", ProfileFields.ClassPlanGroup(""));

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
            profile.DeleteClassPlanGroup(id);
        }
        catch (ArgumentException)
        {
            // 默认课表群和全局课表群无法删除
            return Task.CompletedTask;
        }

        ProfileBlockHelpers.ClearClassPlanGroupReferences(id);
        return Task.CompletedTask;
    }
}
