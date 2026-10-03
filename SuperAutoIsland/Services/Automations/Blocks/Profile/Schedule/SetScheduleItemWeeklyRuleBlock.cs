using System.Text.Json;
using ClassIsland.Shared.Models.Automation;
using ClassIsland.Shared.Models.Profile;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Schedule;

/// <summary>
/// 设置课程的触发规则为「每周」。
/// </summary>
public class SetScheduleItemWeeklyRuleBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.setScheduleItemWeeklyRule";
    public override string Name => "设置课程触发规则为";
    public override void GetFields(FieldsRegister it) => it
        .AddDummy("每周")
        .AddField("ScheduleItem", ProfileFields.ScheduleItem("课程"))
        .AddField("WeekDay", BasicFields.Dropdown("且今天是", WeekDays, useNumbers: true));

    private static List<(string, string)> WeekDays =>
    [
        ("星期日", "0"),
        ("星期一", "1"),
        ("星期二", "2"),
        ("星期三", "3"),
        ("星期四", "4"),
        ("星期五", "5"),
        ("星期六", "6"),
    ];

    public override Task Handler(ActionItem actionItem)
    {
        var settings = JsonSerializer.SerializeToElement(actionItem.Settings);
        var item = ProfileBlockHelpers.ScheduleItem(settings);
        if (item != null)
        {
            item.EnableRule.Type = TimeRule.TimeRuleType.Weekly;
            item.EnableRule.WeekDay = Math.Clamp(ProfileBlockHelpers.Number(settings, "WeekDay"), 0, 6);
            item.EnableRule.WeekCountDiv = 0;
        }

        return Task.CompletedTask;
    }
}
