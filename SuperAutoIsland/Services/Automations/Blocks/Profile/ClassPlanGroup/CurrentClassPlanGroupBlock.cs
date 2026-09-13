using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Services.Automations;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlanGroup;

public class CurrentClassPlanGroupBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.currentClassPlanGroup";
    public override string Name => "当前启用的课表群";
    public override string DataOutput => "SAI_Profile_ClassPlanGroup";
    public override Task<object> Handler(object? data)
    {
        var profile = IAppHost.GetService<IProfileService>().Profile;
        var today = IAppHost.GetService<IExactTimeService>().GetCurrentLocalDateTime().Date;
        var groupId = profile is { IsTempClassPlanGroupEnabled: true, TempClassPlanGroupId: not null }
            && profile.TempClassPlanGroupExpireTime.Date >= today
            && profile.ClassPlanGroups.ContainsKey(profile.TempClassPlanGroupId.Value)
                ? profile.TempClassPlanGroupId.Value
                : profile.SelectedClassPlanGroupId;
        return Task.FromResult<object>(groupId.ToString());
    }
}
