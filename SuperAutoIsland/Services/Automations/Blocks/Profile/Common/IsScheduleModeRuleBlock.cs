using SuperAutoIsland.Interface.Services.Automations;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

/// <summary>
/// 判断档案是否处于日程模式。等价于 <see cref="ProfileBlockHelpers.IsScheduleMode"/>。
/// </summary>
public class IsScheduleModeRuleBlock : RuleBlockBase
{
    public override string Id => "sai.profile.rules.isScheduleMode";
    public override string Name => "是否为日程模式?";
    public override string Tooltip => "判断当前档案是否处于日程模式。日程模式下仅支持读取课表、时间表信息。";

    public override bool Handler(global::ClassIsland.Core.Models.Ruleset.Rule rule) =>
        ProfileBlockHelpers.IsScheduleMode;
}
