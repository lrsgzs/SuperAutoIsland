using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

public class CreateTempClassPlanBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.createTempClassPlan";
    public override string Name => "创建临时层";
    public override string DataOutput => "SAI_Profile_ClassPlan";
    public override void GetFields(FieldsRegister it) => it.AddField("ClassPlan", ProfileFields.ClassPlan(""));
    public override Task<object> Handler(object? data)
    {
        var settings = JsonSerializer.SerializeToElement(data);
        var id = ProfileBlockHelpers.Guid(settings, "ClassPlan");
        var profileService = IAppHost.GetService<IProfileService>();
        if (!profileService.Profile.ClassPlans.ContainsKey(id))
            return Task.FromResult<object>(Guid.Empty.ToString());

        var result = profileService.CreateTempClassPlan(id);
        return Task.FromResult<object>((result ?? Guid.Empty).ToString());
    }
}
