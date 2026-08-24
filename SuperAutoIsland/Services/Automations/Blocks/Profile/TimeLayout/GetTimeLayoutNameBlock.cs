using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.TimeLayout;

public class GetTimeLayoutNameBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.timeLayoutName";
    public override string Name => "时间表名称";

    public override void GetFields(FieldsRegister it) => it
        .AddField("TimeLayout", ProfileFields.TimeLayout(""));

    public override Task<object> Handler(object? data)
    {
        var settings = JsonSerializer.SerializeToElement(data);
        var name = IAppHost.GetService<IProfileService>().Profile.TimeLayouts
            .GetValueOrDefault(ProfileBlockHelpers.Guid(settings, "TimeLayout"))?.Name ?? string.Empty;
        return Task.FromResult<object>(name);
    }
}
