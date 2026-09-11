using System.Text.Json;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

public class GetClassPlanEnabledBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.classPlanEnabled";
    public override string Name => "获取是否自动启用";
    public override string DataOutput => "Boolean";
    public override void GetFields(FieldsRegister it) => it.AddField("ClassPlan", ProfileFields.ClassPlan(""));
    public override Task<object> Handler(object? data)
    {
        var settings = JsonSerializer.SerializeToElement(data);
        return Task.FromResult<object>(ProfileBlockHelpers.ClassPlan(settings)?.IsEnabled ?? false);
    }
}
