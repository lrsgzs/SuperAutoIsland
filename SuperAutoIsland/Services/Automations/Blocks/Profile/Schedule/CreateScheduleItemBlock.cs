using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;
using ProfileScheduleItem = ClassIsland.Shared.Models.Profile.ScheduleItem;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Schedule;

/// <summary>
///     创建一个新课程（日程项目），并输出新课程的 GUID。
/// </summary>
public class CreateScheduleItemBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.createScheduleItem";
    public override string Name => "创建课程";
    public override (string, string) Icon => ("日程", FluentIcons.CalendarRegular);
    public override string Tooltip => "创建一个新课程（日程项目），并输出新课程的 GUID。课程按指定的星期每周启用。";
    public override string DataOutput => "SAI_Profile_ScheduleItem";

    public override void GetFields(FieldsRegister it)
    {
        it
            .AddField("Subject", ProfileFields.Subject(""))
            .AddField("WeekDay", BasicFields.Dropdown("星期", [
                ("周一", "1"), ("周二", "2"), ("周三", "3"), ("周四", "4"), ("周五", "5"), ("周六", "6"), ("周日", "0")
            ], true))
            .AddField("StartTime", BasicFields.Time("开始时间", TimeSpan.FromHours(8)))
            .AddField("EndTime", BasicFields.Time("结束时间", TimeSpan.FromHours(8) + TimeSpan.FromMinutes(40)));
    }

    public override Task<object> Handler(object? data)
    {
        var settings = JsonSerializer.SerializeToElement(data);
        var profile = IAppHost.GetService<IProfileService>().Profile;
        var start = ProfileBlockHelpers.Time(settings, "StartTime");
        var end = ProfileBlockHelpers.Time(settings, "EndTime");
        if (end < start)
        {
            end = start;
        }

        var item = new ProfileScheduleItem
        {
            SubjectId = ProfileBlockHelpers.Guid(settings, "Subject"),
            StartTime = start,
            EndTime = end
        };
        item.EnableRule.WeekDay = Math.Clamp((int)settings.GetProperty("WeekDay").GetDouble(), 0, 6);

        var id = Guid.NewGuid();
        profile.ScheduleItems.Add(id, item);
        return Task.FromResult<object>(id.ToString());
    }
}