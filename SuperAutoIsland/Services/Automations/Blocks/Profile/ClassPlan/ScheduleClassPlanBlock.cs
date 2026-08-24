using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using ClassIsland.Shared.Models.Automation;
using ClassIsland.Shared.Models.Profile;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

public class ScheduleClassPlanBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.scheduleClassPlan";
    public override string Name => "预定临时课表";
    public override void GetFields(FieldsRegister it) => it
        .AddField("ClassPlan", ProfileFields.ClassPlan(""))
        .AddField("Date", BasicFields.Date("日期"));
    public override Task Handler(ActionItem actionItem)
    {
        var settings = JsonSerializer.SerializeToElement(actionItem.Settings);
        var date = ProfileBlockHelpers.Date(settings, "Date").ToDateTime(TimeOnly.MinValue);
        IAppHost.GetService<IProfileService>().Profile.OrderedSchedules[date] = new OrderedSchedule
        {
            ClassPlanId = ProfileBlockHelpers.Guid(settings, "ClassPlan")
        };
        return Task.CompletedTask;
    }
}
