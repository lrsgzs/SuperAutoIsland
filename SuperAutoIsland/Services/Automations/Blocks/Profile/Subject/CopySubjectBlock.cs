using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using ClassIsland.Shared.Helpers;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Subject;

/// <summary>
///     复制指定科目，并输出新科目的 GUID。
/// </summary>
public class CopySubjectBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.copySubject";
    public override string Name => "复制科目";
    public override string Tooltip => "复制指定科目，并输出新科目的 GUID。复制出的科目信息与原科目相同，名称不会自动修改。";
    public override string DataOutput => "SAI_Profile_Subject";

    public override void GetFields(FieldsRegister it)
    {
        it
            .AddField("Subject", ProfileFields.Subject(""));
    }

    public override Task<object> Handler(object? data)
    {
        var settings = JsonSerializer.SerializeToElement(data);
        var profile = IAppHost.GetService<IProfileService>().Profile;
        var source = ProfileBlockHelpers.Subject(settings);
        var copy = source is null ? null : ConfigureFileHelper.CopyObject(source);
        if (copy is null)
        {
            return Task.FromResult<object>(Guid.Empty.ToString());
        }

        var id = Guid.NewGuid();
        profile.Subjects.Add(id, copy);
        return Task.FromResult<object>(id.ToString());
    }
}