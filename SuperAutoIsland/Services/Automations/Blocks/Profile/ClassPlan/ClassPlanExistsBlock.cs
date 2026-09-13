using System.Text.Json;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

public class ClassPlanExistsBlock : RuleBlockBase
{
    public override string Id => "sai.profile.rules.classPlanExists";
    public override string Name => "课表存在?";
    public override void GetFields(FieldsRegister it) => it.AddField("ClassPlan", ProfileFields.ClassPlan(""));
    public override bool Handler(global::ClassIsland.Core.Models.Ruleset.Rule rule)
    {
        var settings = JsonSerializer.SerializeToElement(rule.Settings);
        return ProfileBlockHelpers.ClassPlan(settings) is not null;
    }
}
