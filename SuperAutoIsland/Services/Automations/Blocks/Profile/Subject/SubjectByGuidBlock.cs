using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Models.Data;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Subject;

public class SubjectByGuidBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.subjectByGuid";
    public override string Name => "科目";
    public override (string, string) Icon => ("书", FluentIcons.BookRegular);
    public override Type SettingsType => typeof(StringValueData);
    public override string DataOutput => "SAI_Profile_Subject";

    public override void GetFields(FieldsRegister it) => it
        .AddField("Value", BasicFields.DynamicDropdown("", "sai.profile.dd.subjects"));

    public override Task<object> Handler(object? data) =>
        Task.FromResult<object>(data is StringValueData settings
            ? settings.Value
            : Guid.Empty.ToString());
}
