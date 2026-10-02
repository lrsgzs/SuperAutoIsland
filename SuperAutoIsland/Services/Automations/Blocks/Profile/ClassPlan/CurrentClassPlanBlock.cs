using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

public class CurrentClassPlanBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.currentClassPlan";
    public override string Name => "当前启用的课表";
    public override string DataOutput => "SAI_Profile_ClassPlan";
    public override Task<object> Handler(object? data)
    {
        var now = IAppHost.GetService<IExactTimeService>().GetCurrentLocalDateTime();
        return Task.FromResult<object>(ProfileBlockHelpers.ClassPlanRefByDate(now));
    }
}
