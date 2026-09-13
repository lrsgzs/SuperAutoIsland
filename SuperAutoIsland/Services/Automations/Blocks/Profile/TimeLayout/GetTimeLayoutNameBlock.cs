using System.Text.Json;
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
        var name = ProfileBlockHelpers.TimeLayout(settings)?.Name ?? string.Empty;
        return Task.FromResult<object>(name);
    }
}
