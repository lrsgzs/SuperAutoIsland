using System.Text.Json;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

public class GetClassPlanSubjectBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.classPlanSubject";
    public override string Name => "课表";
    public override string DataOutput => "SAI_Profile_Subject";
    public override bool InlineBlock => true;
    public override bool InlineField => true;
    
    public override void GetFields(FieldsRegister it) => it
        .AddField("ClassPlan", ProfileFields.ClassPlan("获取"))
        .AddField("Index", BasicFields.Number("第", 1))
        .AddDummy("节课的科目");
    
    public override Task<object> Handler(object? data)
    {
        var settings = JsonSerializer.SerializeToElement(data);
        var plan = ProfileBlockHelpers.ClassPlan(settings);
        var index = (int)settings.GetProperty("Index").GetDouble() - 1;
        var id = plan is not null && index >= 0 && index < plan.Classes.Count
            ? plan.Classes[index].SubjectId
            : Guid.Empty;
        return Task.FromResult<object>(id.ToString());
    }
}
