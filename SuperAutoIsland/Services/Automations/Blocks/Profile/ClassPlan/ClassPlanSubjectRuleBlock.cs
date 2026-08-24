using System.Text.Json;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

public class ClassPlanSubjectRuleBlock : RuleBlockBase
{
    public override string Id => "sai.profile.rules.classPlanSubject";
    public override string Name => "课表";
    public override bool InlineBlock => true;
    public override bool InlineField => true;
    
    public override void GetFields(FieldsRegister it) => it
        .AddField("ClassPlan", ProfileFields.ClassPlan(""))
        .AddField("Index", BasicFields.Number("第", 1))
        .AddField("Subject", ProfileFields.Subject("节课是"));
    
    public override bool Handler(global::ClassIsland.Core.Models.Ruleset.Rule rule)
    {
        var settings = JsonSerializer.SerializeToElement(rule.Settings);
        var plan = ProfileBlockHelpers.ClassPlan(settings);
        return plan is not null
            && ProfileBlockHelpers.ClassSubjectId(plan, ProfileBlockHelpers.Number(settings, "Index")) == ProfileBlockHelpers.Guid(settings, "Subject");
    }
}
