using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlanGroup;

public class ClassPlanGroupExistsBlock : RuleBlockBase
{
    public override string Id => "sai.profile.rules.classPlanGroupExists";
    public override string Name => "课表群存在?";
    public override void GetFields(FieldsRegister it) => it.AddField("Group", ProfileFields.ClassPlanGroup(""));
    public override bool Handler(global::ClassIsland.Core.Models.Ruleset.Rule rule)
    {
        var s = JsonSerializer.SerializeToElement(rule.Settings);
        return IAppHost.GetService<IProfileService>().Profile.ClassPlanGroups.ContainsKey(ProfileBlockHelpers.Guid(s, "Group"));
    }
}
