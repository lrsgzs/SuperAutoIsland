using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Models.Data;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlanGroup;

public class ClassPlanGroupByGuidBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.classPlanGroupByGuid";
    public override string Name => "课表群";
    public override (string, string) Icon => ("群", FluentIcons.GroupRegular);
    public override Type SettingsType => typeof(StringValueData);
    public override string DataOutput => "SAI_Profile_ClassPlanGroup";

    public override void GetFields(FieldsRegister it) => it
        .AddField("Value", BasicFields.DynamicDropdown("", "sai.profile.dd.classPlanGroups"));

    public override Task<object> Handler(object? data) =>
        Task.FromResult<object>(data is StringValueData settings
            ? settings.Value
            : Guid.Empty.ToString());
}
