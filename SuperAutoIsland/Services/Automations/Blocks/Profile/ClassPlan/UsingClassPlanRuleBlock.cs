using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

public class UsingClassPlanRuleBlock : RuleBlockBase
{
    public override string Id => "sai.profile.rules.usingClassPlan";
    public override string Name => "课表";
    public override bool InlineBlock => true;
    public override bool InlineField => true;
    
    public override void GetFields(FieldsRegister it) => it
        .AddDummy("正在使用")
        .AddField("ClassPlan", ProfileFields.ClassPlan(""))
        .AddDummy("?");
    public override bool Handler(global::ClassIsland.Core.Models.Ruleset.Rule rule)
    {
        var settings = JsonSerializer.SerializeToElement(rule.Settings);
        var configured = ProfileBlockHelpers.ClassPlanRef(settings, "ClassPlan");
        return string.Equals(GetCurrentClassPlanRef(), configured, StringComparison.OrdinalIgnoreCase);
    }

    private static string GetCurrentClassPlanRef()
    {
        var lessons = IAppHost.GetService<ILessonsService>();
        var now = IAppHost.GetService<IExactTimeService>().GetCurrentLocalDateTime();
        var plan = lessons.GetClassPlanByDate(now, out var id);
        if (id is null)
        {
            return plan is null
                ? Guid.Empty.ToString()
                : ProfileBlockHelpers.CreateScheduleRef(DateOnly.FromDateTime(now));
        }

        return id.Value.ToString();
    }
}
