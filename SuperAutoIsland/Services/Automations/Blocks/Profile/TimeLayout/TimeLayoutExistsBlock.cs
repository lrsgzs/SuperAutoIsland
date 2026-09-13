using System.Text.Json;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.TimeLayout;

public class TimeLayoutExistsBlock : RuleBlockBase
{
    public override string Id => "sai.profile.rules.timeLayoutExists";
    public override string Name => "时间表存在?";
    public override void GetFields(FieldsRegister it) => it.AddField("Value", ProfileFields.TimeLayout(""));
    public override bool Handler(global::ClassIsland.Core.Models.Ruleset.Rule rule)
    {
        var settings = JsonSerializer.SerializeToElement(rule.Settings);
        return ProfileBlockHelpers.TimeLayout(settings, "Value") is not null;
    }
}
