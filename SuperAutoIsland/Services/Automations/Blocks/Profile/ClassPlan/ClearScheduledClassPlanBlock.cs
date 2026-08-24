using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

public class ClearScheduledClassPlanBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.clearScheduledClassPlan";
    public override string Name => "清除预定课表";
    public override void GetFields(FieldsRegister it) => it.AddField("Date", BasicFields.Date("日期"));
    public override Task Handler(ActionItem actionItem)
    {
        var settings = JsonSerializer.SerializeToElement(actionItem.Settings);
        IAppHost.GetService<IProfileService>().Profile.OrderedSchedules.Remove(
            ProfileBlockHelpers.Date(settings, "Date").ToDateTime(TimeOnly.MinValue));
        return Task.CompletedTask;
    }
}
