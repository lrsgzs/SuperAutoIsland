using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

public class GetClassPlanNameBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.classPlanName";
    public override string Name => "课表名称";

    public override void GetFields(FieldsRegister it) => it
        .AddField("ClassPlan", ProfileFields.ClassPlan(""));

    public override Task<object> Handler(object? data)
    {
        var settings = JsonSerializer.SerializeToElement(data);
        var name = IAppHost.GetService<IProfileService>().Profile.ClassPlans
            .GetValueOrDefault(ProfileBlockHelpers.Guid(settings, "ClassPlan"))?.Name ?? string.Empty;
        return Task.FromResult<object>(name);
    }
}
