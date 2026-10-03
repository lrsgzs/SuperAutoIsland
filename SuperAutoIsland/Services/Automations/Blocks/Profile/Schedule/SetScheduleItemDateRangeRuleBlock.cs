using System.Text.Json;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Schedule;

/// <summary>
///     设置课程的启用日期范围。
/// </summary>
public class SetScheduleItemDateRangeRuleBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.setScheduleItemDateRangeRule";
    public override string Name => "设置课程启用日期范围";

    public override void GetFields(FieldsRegister it)
    {
        it
            .AddField("ScheduleItem", ProfileFields.ScheduleItem("课程"))
            .AddField("StartDate", BasicFields.Date("开始日期", DateOnly.FromDateTime(DateTime.Today)))
            .AddField("EndDate", BasicFields.Date("结束日期", DateOnly.FromDateTime(DateTime.Today)));
    }

    public override Task Handler(ActionItem actionItem)
    {
        var settings = JsonSerializer.SerializeToElement(actionItem.Settings);
        var item = ProfileBlockHelpers.ScheduleItem(settings);
        if (item != null)
        {
            item.EnableRule.RestrictsEnableRange = true;
            item.EnableRule.RangeStart = ProfileBlockHelpers.Date(settings, "StartDate");
            item.EnableRule.RangeEnd = ProfileBlockHelpers.Date(settings, "EndDate");
        }

        return Task.CompletedTask;
    }
}