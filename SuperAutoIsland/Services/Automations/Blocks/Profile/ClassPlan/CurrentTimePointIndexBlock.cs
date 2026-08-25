using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Services.Automations;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

/// <summary>
/// 当前为第几个时间点（从 1 开始计数）。当前没有时间点时返回 0。
/// </summary>
public class CurrentTimePointIndexBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.currentTimePointIndex";
    public override string Name => "当前为第几个时间点";
    public override string Tooltip => "当前为第几个时间点（从 1 开始计数）。当前没有时间点时返回 0。";
    public override string DataOutput => "Number";

    public override Task<object> Handler(object? data)
    {
        var index = IAppHost.GetService<ILessonsService>().CurrentSelectedIndex;
        return Task.FromResult<object>(index >= 0 ? index + 1 : 0);
    }
}
