using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.TimeLayout;

/// <summary>
///     删除指定时间表。仍有课表使用该时间表时不删除。
/// </summary>
public class DeleteTimeLayoutBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.deleteTimeLayout";
    public override string Name => "删除时间表";
    public override string Tooltip => "删除指定时间表。若仍有课表在使用该时间表，则不会删除。";

    public override void GetFields(FieldsRegister it)
    {
        it
            .AddField("TimeLayout", ProfileFields.TimeLayout(""));
    }

    public override Task Handler(ActionItem actionItem)
    {
        var settings = JsonSerializer.SerializeToElement(actionItem.Settings);
        var profile = IAppHost.GetService<IProfileService>().Profile;
        var id = ProfileBlockHelpers.Guid(settings, "TimeLayout");
        if (id == Guid.Empty || !profile.TimeLayouts.ContainsKey(id))
        {
            return Task.CompletedTask;
        }

        // 仍有课表在使用该时间表时，删除会导致这些课表失去时间表，因此不做修改
        if (profile.ClassPlans.Any(x => x.Value.TimeLayoutId == id))
        {
            return Task.CompletedTask;
        }

        profile.TimeLayouts.Remove(id);
        return Task.CompletedTask;
    }
}