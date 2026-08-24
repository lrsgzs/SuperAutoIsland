using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Subject;

public class SubjectExistsBlock : RuleBlockBase
{
    public override string Id => "sai.profile.rules.subjectExists";
    public override string Name => "科目存在?";
    public override void GetFields(FieldsRegister it) => it.AddField("Value", ProfileFields.Subject(""));
    public override bool Handler(global::ClassIsland.Core.Models.Ruleset.Rule rule)
    {
        var settings = JsonSerializer.SerializeToElement(rule.Settings);
        return IAppHost.GetService<IProfileService>().Profile.Subjects.ContainsKey(ProfileBlockHelpers.Guid(settings, "Value"));
    }
}
