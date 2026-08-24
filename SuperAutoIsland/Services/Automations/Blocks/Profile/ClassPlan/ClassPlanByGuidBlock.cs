using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Models.Data;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

public class ClassPlanByGuidBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.classPlanByGuid";
    public override string Name => "课表";
    public override (string, string) Icon => ("文档", FluentIcons.DocumentDataRegular);
    public override Type SettingsType => typeof(StringValueData);
    public override string DataOutput => "SAI_Profile_ClassPlan";

    public override void GetFields(FieldsRegister it) => it
        .AddField("Value", BasicFields.DynamicDropdown("", "sai.profile.dd.classPlans"));

    public override Task<object> Handler(object? data) =>
        Task.FromResult<object>(data is StringValueData settings
            ? settings.Value
            : Guid.Empty.ToString());
}
