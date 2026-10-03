using System.Text.Json;
using ClassIsland.Core.Models.Ruleset;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

public class ClassPlanOverlayRuleBlock : RuleBlockBase
{
    public override string Id => "sai.profile.rules.classPlanOverlay";
    public override string Name => "课表";
    public override bool InlineBlock => true;
    public override bool InlineField => true;

    public override void GetFields(FieldsRegister it)
    {
        it
            .AddField("ClassPlan", ProfileFields.ClassPlan(""))
            .AddDummy("是临时层?");
    }

    public override bool Handler(Rule rule)
    {
        var settings = JsonSerializer.SerializeToElement(rule.Settings);
        return ProfileBlockHelpers.ClassPlan(settings)?.IsOverlay == true;
    }
}