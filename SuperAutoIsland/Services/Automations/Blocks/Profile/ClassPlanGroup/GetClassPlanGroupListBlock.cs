using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Services.Automations;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlanGroup;

/// <summary>
/// 获取档案中所有课表群的 GUID 列表。
/// </summary>
public class GetClassPlanGroupListBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.classPlanGroupList";
    public override string Name => "课表群列表";
    public override (string, string) Icon => ("群", FluentIcons.GroupRegular);
    public override string Tooltip => "获取档案中所有课表群的 GUID 列表（按档案中的顺序）。";
    public override string DataOutput => "Array";

    public override Task<object> Handler(object? data)
    {
        var classPlanGroups = IAppHost.GetService<IProfileService>().Profile.ClassPlanGroups;
        return Task.FromResult<object>(classPlanGroups.Keys.Select(x => x.ToString()).ToList());
    }
}
