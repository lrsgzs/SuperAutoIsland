using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Services.Automations;
using ProfileClassPlan = ClassIsland.Shared.Models.Profile.ClassPlan;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

public class CreateEmptyClassPlanBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.createEmptyClassPlan";
    public override string Name => "创建空白课表";
    public override string DataOutput => "SAI_Profile_ClassPlan";

    public override Task<object> Handler(object? data)
    {
        var profile = IAppHost.GetService<IProfileService>().Profile;
        var plan = new ProfileClassPlan
        {
            AssociatedGroup = profile.SelectedClassPlanGroupId,
            TimeLayoutId = Guid.Empty
        };
        var id = Guid.NewGuid();
        profile.ClassPlans.Add(id, plan);
        return Task.FromResult<object>(id.ToString());
    }
}
