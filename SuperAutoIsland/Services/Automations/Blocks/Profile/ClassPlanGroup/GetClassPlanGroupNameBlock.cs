using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlanGroup;

public class GetClassPlanGroupNameBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.classPlanGroupName";
    public override string Name => "课表群名称";

    public override void GetFields(FieldsRegister it) => it
        .AddField("Group", ProfileFields.ClassPlanGroup(""));

    public override Task<object> Handler(object? data)
    {
        var settings = JsonSerializer.SerializeToElement(data);
        var name = IAppHost.GetService<IProfileService>().Profile.ClassPlanGroups
            .GetValueOrDefault(ProfileBlockHelpers.Guid(settings, "Group"))?.Name ?? string.Empty;
        return Task.FromResult<object>(name);
    }
}
