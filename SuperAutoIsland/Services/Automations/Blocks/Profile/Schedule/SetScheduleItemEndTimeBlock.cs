using System.Text.Json;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Schedule;

/// <summary>
/// 设置课程的结束时间。
/// </summary>
public class SetScheduleItemEndTimeBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.setScheduleItemEndTime";
    public override string Name => "设置课程结束时间";
    public override string Tooltip => "设置课程的结束时间。结束时间早于开始时间时，开始时间会一起向前移动。";

    public override void GetFields(FieldsRegister it) => it
        .AddField("ScheduleItem", ProfileFields.ScheduleItem(""))
        .AddField("EndTime", BasicFields.Time("结束时间", TimeSpan.FromHours(8) + TimeSpan.FromMinutes(45)));

    public override Task Handler(ActionItem actionItem)
    {
        var settings = JsonSerializer.SerializeToElement(actionItem.Settings);
        var item = ProfileBlockHelpers.ScheduleItem(settings);
        if (item != null)
        {
            item.EndTime = ProfileBlockHelpers.Time(settings, "EndTime");
        }

        return Task.CompletedTask;
    }
}
