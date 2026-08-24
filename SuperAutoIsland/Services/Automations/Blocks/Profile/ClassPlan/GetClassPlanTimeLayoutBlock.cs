using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;
using System.Text.Json;
using SuperAutoIsland.Interface.Services;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

public class GetClassPlanTimeLayoutBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.classPlanTimeLayout";
    public override string Name => "获取时间表";
    public override string DataOutput => "SAI_Profile_TimeLayout";
    public override void GetFields(FieldsRegister it) => it.AddField("ClassPlan", ProfileFields.ClassPlan(""));
    public override Task<object> Handler(object? data)
    {
        var settings = JsonSerializer.SerializeToElement(data);
        return Task.FromResult<object>((ProfileBlockHelpers.ClassPlan(settings)?.TimeLayoutId ?? Guid.Empty).ToString());
    }
}
