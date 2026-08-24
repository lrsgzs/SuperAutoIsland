using System.Text.Json;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

public class SetDateRangeRuleBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.setDateRangeRule";
    public override string Name => "设置课表启用日期范围";
    public override void GetFields(FieldsRegister it) => it
        .AddField("ClassPlan", ProfileFields.ClassPlan(""))
        .AddField("StartDate", BasicFields.Date("开始日期"))
        .AddField("EndDate", BasicFields.Date("结束日期"));
    public override Task Handler(ActionItem actionItem)
    {
        var s = JsonSerializer.SerializeToElement(actionItem.Settings);
        var plan = ProfileBlockHelpers.ClassPlan(s);
        if (plan != null)
        {
            plan.TimeRule.RestrictsEnableRange = true;
            plan.TimeRule.RangeStart = ProfileBlockHelpers.Date(s, "StartDate");
            plan.TimeRule.RangeEnd = ProfileBlockHelpers.Date(s, "EndDate");
        }
        return Task.CompletedTask;
    }
}
