using System.Text.Json;
using ClassIsland.Shared.Models.Automation;
using ClassIsland.Shared.Models.Profile;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Schedule;

/// <summary>
/// 设置课程的触发规则为「循环」。
/// </summary>
public class SetScheduleItemLoopRuleBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.setScheduleItemLoopRule";
    public override string Name => "设置课程触发规则为";

    public override void GetFields(FieldsRegister it) => it
        .AddDummy("循环")
        .AddField("ScheduleItem", ProfileFields.ScheduleItem("课程"))
        .AddField("CycleDays", BasicFields.Number("每几天启用一次", 3))
        .AddField("OffsetDays", BasicFields.Number("向后偏移几天", 0));

    public override Task Handler(ActionItem actionItem)
    {
        var settings = JsonSerializer.SerializeToElement(actionItem.Settings);
        var item = ProfileBlockHelpers.ScheduleItem(settings);
        if (item != null)
        {
            item.EnableRule.Type = TimeRule.TimeRuleType.Loop;
            var cycleDays = Math.Max(1, ProfileBlockHelpers.Number(settings, "CycleDays"));
            var offsetDays = ProfileBlockHelpers.Number(settings, "OffsetDays");
            item.EnableRule.LoopCycleDays = cycleDays;
            item.EnableRule.LoopOffsetDays = ((offsetDays % cycleDays) + cycleDays) % cycleDays;
        }

        return Task.CompletedTask;
    }
}
