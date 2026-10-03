using System.Text.Json;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Schedule;

/// <summary>
///     设置课程的开始时间。
/// </summary>
public class SetScheduleItemStartTimeBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.setScheduleItemStartTime";
    public override string Name => "设置课程开始时间";
    public override string Tooltip => "设置课程的开始时间。开始时间晚于结束时间时，结束时间会一起向后移动。";

    public override void GetFields(FieldsRegister it)
    {
        it
            .AddField("ScheduleItem", ProfileFields.ScheduleItem(""))
            .AddField("StartTime", BasicFields.Time("开始时间", TimeSpan.FromHours(8)));
    }

    public override Task Handler(ActionItem actionItem)
    {
        var settings = JsonSerializer.SerializeToElement(actionItem.Settings);
        var item = ProfileBlockHelpers.ScheduleItem(settings);
        if (item != null)
        {
            item.StartTime = ProfileBlockHelpers.Time(settings, "StartTime");
        }

        return Task.CompletedTask;
    }
}