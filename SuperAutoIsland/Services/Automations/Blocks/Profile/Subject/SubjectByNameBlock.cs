using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Models.Data;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Subject;

public class SubjectByNameBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.subjectByName";
    public override string Name => "科目";
    public override (string, string) Icon => ("书", FluentIcons.BookRegular);

    public override Type SettingsType => typeof(StringValueData);
    public override string DataOutput => "SAI_Profile_Subject";

    public override void GetFields(FieldsRegister it) => it
        .AddField("Value", BasicFields.Text("名称"));

    public override Task<object> Handler(object? data)
    {
        if (data is not StringValueData settings)
            return Task.FromResult<object>(Guid.Empty.ToString());

        return Task.FromResult<object>(IAppHost.GetService<IProfileService>().Profile.Subjects
            .Where(x => x.Value.Name == settings.Value)
            .Select(x => x.Key)
            .FirstOrDefault(Guid.Empty)
            .ToString());
    }
}
