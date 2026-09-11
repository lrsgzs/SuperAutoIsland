using System.Text.Json;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

public class GetClassPlanGroupBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.classPlanGroup";
    public override string Name => "获取课表群";
    public override string DataOutput => "SAI_Profile_ClassPlanGroup";
    public override void GetFields(FieldsRegister it) => it.AddField("ClassPlan", ProfileFields.ClassPlan(""));
    public override Task<object> Handler(object? data)
    {
        var settings = JsonSerializer.SerializeToElement(data);
        return Task.FromResult<object>((ProfileBlockHelpers.ClassPlan(settings)?.AssociatedGroup ?? Guid.Empty).ToString());
    }
}
