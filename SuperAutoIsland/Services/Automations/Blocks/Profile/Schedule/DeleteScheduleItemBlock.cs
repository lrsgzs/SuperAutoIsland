using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Schedule;

/// <summary>
/// 删除指定课程（日程项目）。
/// </summary>
public class DeleteScheduleItemBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.deleteScheduleItem";
    public override string Name => "删除课程";

    public override void GetFields(FieldsRegister it) => it
        .AddField("ScheduleItem", ProfileFields.ScheduleItem(""));

    public override Task Handler(ActionItem actionItem)
    {
        var settings = JsonSerializer.SerializeToElement(actionItem.Settings);
        var profile = IAppHost.GetService<IProfileService>().Profile;
        var id = ProfileBlockHelpers.Guid(settings, "ScheduleItem");
        if (id == Guid.Empty)
        {
            return Task.CompletedTask;
        }

        profile.ScheduleItems.Remove(id);
        return Task.CompletedTask;
    }
}
