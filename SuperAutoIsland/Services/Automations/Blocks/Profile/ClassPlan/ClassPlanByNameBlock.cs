using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Models.Data;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

public class ClassPlanByNameBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.classPlanByName";
    public override string Name => "课表";
    public override (string, string) Icon => ("文档", FluentIcons.DocumentDataRegular);

    public override Type SettingsType => typeof(StringValueData);
    public override string DataOutput => "SAI_Profile_ClassPlan";

    public override void GetFields(FieldsRegister it) => it
        .AddField("Value", BasicFields.Text("名称"));

    public override Task<object> Handler(object? data)
    {
        if (data is not StringValueData settings)
            return Task.FromResult<object>(Guid.Empty.ToString());

        return Task.FromResult<object>(IAppHost.GetService<IProfileService>().Profile.ClassPlans
            .Where(x => x.Value.Name == settings.Value)
            .Select(x => x.Key)
            .FirstOrDefault(Guid.Empty)
            .ToString());
    }
}
