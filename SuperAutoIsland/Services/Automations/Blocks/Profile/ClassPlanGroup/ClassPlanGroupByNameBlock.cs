using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Models.Data;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlanGroup;

public class ClassPlanGroupByNameBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.classPlanGroupByName";
    public override string Name => "课表群";
    public override (string, string) Icon => ("群", FluentIcons.GroupRegular);

    public override Type SettingsType => typeof(StringValueData);
    public override string DataOutput => "SAI_Profile_ClassPlanGroup";

    public override void GetFields(FieldsRegister it) => it
        .AddField("Value", BasicFields.Text("名称"));

    public override Task<object> Handler(object? data)
    {
        if (data is not StringValueData settings)
            return Task.FromResult<object>(Guid.Empty.ToString());

        return Task.FromResult<object>(IAppHost.GetService<IProfileService>().Profile.ClassPlanGroups
            .Where(x => x.Value.Name == settings.Value)
            .Select(x => x.Key)
            .FirstOrDefault(Guid.Empty)
            .ToString());
    }
}
