using System.Text.Json;
using ClassIsland.Shared.Models.Automation;
using ClassIsland.Shared.Models.Profile;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

public class SetWeeklyCycleRuleBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.setWeeklyCycleRule";
    public override string Name => "设置课表触发规则为";
    public override void GetFields(FieldsRegister it) => it
        .AddDummy("每周")
        .AddField("ClassPlan", ProfileFields.ClassPlan("课表"))
        .AddField("WeekCountDivTotal", BasicFields.Number("每几周", 2))
        .AddField("WeekCountDiv", BasicFields.Number("的第几周(0=每周)", 0))
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
        var s = JsonSerializer.SerializeToElement(actionItem.Settings);
        var plan = ProfileBlockHelpers.ClassPlan(s);
        if (plan != null)
        {
            var total = ProfileBlockHelpers.Number(s, "WeekCountDivTotal");
            var div = ProfileBlockHelpers.Number(s, "WeekCountDiv");
            if (total < 2 || div < 0 || div > total)
                return Task.CompletedTask;

            plan.TimeRule.Type = TimeRule.TimeRuleType.Weekly;
            plan.TimeRule.WeekCountDiv = div;
            plan.TimeRule.WeekCountDivTotal = total;
            plan.TimeRule.WeekDay = Math.Clamp(ProfileBlockHelpers.Number(s, "WeekDay"), 0, 6);
        }
        return Task.CompletedTask;
    }
}
