using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.TimeLayout;

/// <summary>
/// 验证「时间表 GUID[序号]」所指向的时间点是否存在。
/// </summary>
public class TimeLayoutItemExistsRuleBlock : RuleBlockBase
{
    public override string Id => "sai.profile.rules.timeLayoutItemExists";
    public override string Name => "时间点";
    public override string Tooltip => "验证「时间表 GUID[序号]」所指向的时间点是否存在。";
    public override bool InlineBlock => true;
    public override bool InlineField => true;

    public override void GetFields(FieldsRegister it) => it
        .AddField("TimeLayoutItem", ProfileFields.TimeLayoutItem(""))
        .AddDummy("存在?");

    public override bool Handler(global::ClassIsland.Core.Models.Ruleset.Rule rule)
    {
        var settings = JsonSerializer.SerializeToElement(rule.Settings);
        var (guid, index) = ProfileBlockHelpers.TimeLayoutItem(settings);
        if (index < 1)
            return false;
        var layout = IAppHost.GetService<IProfileService>().Profile.TimeLayouts.GetValueOrDefault(guid);
        return layout is not null && index <= layout.Layouts.Count;
    }
}
