using System.Text.Json;
using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Schedule;

/// <summary>
///     获取课程的开始时间，格式 HH:mm:ss。
/// </summary>
public class GetScheduleItemStartTimeBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.scheduleItemStartTime";
    public override string Name => "课程";
    public override (string, string) Icon => ("日程", FluentIcons.CalendarRegular);
    public override string Tooltip => "获取课程的开始时间，格式 HH:mm:ss。课程不存在时返回 00:00:00。";
    public override string DataOutput => "SAI_Time";
    public override bool InlineBlock => true;
    public override bool InlineField => true;

    public override void GetFields(FieldsRegister it)
    {
        it
            .AddField("ScheduleItem", ProfileFields.ScheduleItem(""))
            .AddDummy("的开始时间");
    }

    public override Task<object> Handler(object? data)
    {
        var settings = JsonSerializer.SerializeToElement(data);
        var startTime = ProfileBlockHelpers.ScheduleItem(settings)?.StartTime;
        return Task.FromResult<object>(startTime?.ToString(@"hh\:mm\:ss") ?? "00:00:00");
    }
}