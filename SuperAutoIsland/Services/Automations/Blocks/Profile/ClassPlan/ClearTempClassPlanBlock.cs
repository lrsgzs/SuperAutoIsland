using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;
using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

public class ClearTempClassPlanBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.clearTempClassPlan";
    public override string Name => "清除临时课表";
    public override void GetFields(FieldsRegister it) => it.AddField("Date", BasicFields.Date("日期"));
    public override Task Handler(ActionItem actionItem)
    {
        var settings = JsonSerializer.SerializeToElement(actionItem.Settings);
        var profile = IAppHost.GetService<IProfileService>().Profile;
        var date = ProfileBlockHelpers.Date(settings, "Date");
        var today = DateOnly.FromDateTime(DateTime.Now);
        if (date == today)
        {
            profile.TempClassPlanId = null;
        }
        else
        {
            profile.OrderedSchedules.Remove(date.ToDateTime(TimeOnly.MinValue));
        }
        return Task.CompletedTask;
    }
}
