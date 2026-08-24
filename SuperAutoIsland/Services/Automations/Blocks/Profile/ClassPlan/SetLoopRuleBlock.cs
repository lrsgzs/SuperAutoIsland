using System.Text.Json;
using ClassIsland.Shared.Models.Automation;
using ClassIsland.Shared.Models.Profile;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

public class SetLoopRuleBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.setLoopRule";
    public override string Name => "设置课表触发规则为";

    public override void GetFields(FieldsRegister it) => it
        .AddDummy("循环")
        .AddField("ClassPlan", ProfileFields.ClassPlan(""))
        .AddField("CycleDays", BasicFields.Number("每几天启用一次", 3))
        .AddField("OffsetDays", BasicFields.Number("向后偏移几天", 0));

    public override Task Handler(ActionItem actionItem)
    {
        var settings = JsonSerializer.SerializeToElement(actionItem.Settings);
        var plan = ProfileBlockHelpers.ClassPlan(settings);
        if (plan != null)
        {
            plan.TimeRule.Type = TimeRule.TimeRuleType.Loop;
            plan.TimeRule.LoopCycleDays = Math.Max(1, ProfileBlockHelpers.Number(settings, "CycleDays"));
            plan.TimeRule.LoopOffsetDays = ProfileBlockHelpers.Number(settings, "OffsetDays");
        }

        return Task.CompletedTask;
    }
}
