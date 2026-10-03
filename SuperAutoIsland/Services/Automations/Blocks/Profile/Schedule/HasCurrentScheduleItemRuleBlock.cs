using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Schedule;

/// <summary>
/// 判断当前是否有正在进行中的课程（日程项目）。
/// </summary>
public class HasCurrentScheduleItemRuleBlock : RuleBlockBase
{
    public override string Id => "sai.profile.rules.hasCurrentScheduleItem";
    public override string Name => "当前是否有课程?";
    public override (string, string) Icon => ("日程", FluentIcons.CalendarRegular);
    public override string Tooltip => "判断当前是否有正在进行中的课程。";

    public override bool Handler(global::ClassIsland.Core.Models.Ruleset.Rule rule) =>
        ProfileBlockHelpers.CurrentScheduleItem() is not null;
}
