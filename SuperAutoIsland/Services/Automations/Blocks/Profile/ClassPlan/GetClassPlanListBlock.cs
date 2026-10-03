using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Services.Automations;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

/// <summary>
/// 获取档案中所有课表的 GUID 列表。
/// </summary>
public class GetClassPlanListBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.classPlanList";
    public override string Name => "课表列表";
    public override (string, string) Icon => ("课表", FluentIcons.CalendarLtrRegular);
    public override string Tooltip => "获取档案中所有课表的 GUID 列表（按档案中的顺序，包含临时层课表）。";
    public override string DataOutput => "Array";

    public override Task<object> Handler(object? data)
    {
        var classPlans = IAppHost.GetService<IProfileService>().Profile.ClassPlans;
        return Task.FromResult<object>(classPlans.Keys.Select(x => x.ToString()).ToList());
    }
}
