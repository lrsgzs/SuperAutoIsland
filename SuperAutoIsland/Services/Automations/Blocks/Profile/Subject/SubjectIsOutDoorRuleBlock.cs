using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Subject;

public class SubjectIsOutDoorRuleBlock : RuleBlockBase
{
    public override string Id => "sai.profile.rules.subjectIsOutDoor";
    public override string Name => "科目";
    public override string Tooltip => "验证指定科目是否为户外课程。";
    public override bool InlineBlock => true;
    public override bool InlineField => true;

    public override void GetFields(FieldsRegister it) => it
        .AddField("Subject", ProfileFields.Subject(""))
        .AddDummy("是户外课程?");

    public override bool Handler(global::ClassIsland.Core.Models.Ruleset.Rule rule)
    {
        var settings = JsonSerializer.SerializeToElement(rule.Settings);
        return IAppHost.GetService<IProfileService>().Profile.Subjects
            .GetValueOrDefault(ProfileBlockHelpers.Guid(settings, "Subject"))?.IsOutDoor == true;
    }
}
