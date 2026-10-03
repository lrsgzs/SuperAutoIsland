using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using ProfileSubject = ClassIsland.Shared.Models.Profile.Subject;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Subject;

/// <summary>
/// 创建一个新科目，并输出新科目的 GUID。
/// </summary>
public class CreateSubjectBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.createSubject";
    public override string Name => "创建科目";
    public override string Tooltip => "创建一个新科目，并输出新科目的 GUID。";
    public override string DataOutput => "SAI_Profile_Subject";

    public override void GetFields(FieldsRegister it) => it
        .AddField("Name", BasicFields.Text("名称", "新科目"));

    public override Task<object> Handler(object? data)
    {
        var settings = JsonSerializer.SerializeToElement(data);
        var profile = IAppHost.GetService<IProfileService>().Profile;
        var subject = new ProfileSubject
        {
            Name = settings.GetProperty("Name").GetString() ?? string.Empty
        };
        var id = Guid.NewGuid();
        profile.Subjects.Add(id, subject);
        return Task.FromResult<object>(id.ToString());
    }
}
