using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Services.Automations;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.TimeLayout;

/// <summary>
/// 获取档案中所有时间表的 GUID 列表。
/// </summary>
public class GetTimeLayoutListBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.timeLayoutList";
    public override string Name => "时间表列表";
    public override (string, string) Icon => ("表格", FluentIcons.TableRegular);
    public override string Tooltip => "获取档案中所有时间表的 GUID 列表（按档案中的顺序）。";
    public override string DataOutput => "Array";

    public override Task<object> Handler(object? data)
    {
        var timeLayouts = IAppHost.GetService<IProfileService>().Profile.TimeLayouts;
        return Task.FromResult<object>(timeLayouts.Keys.Select(x => x.ToString()).ToList());
    }
}
