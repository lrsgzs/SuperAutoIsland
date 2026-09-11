using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Subject;

public class GetSubjectInitialBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.subjectInitial";
    public override string Name => "科目简称";

    public override void GetFields(FieldsRegister it) => it
        .AddField("Subject", ProfileFields.Subject(""));

    public override Task<object> Handler(object? data)
    {
        var settings = JsonSerializer.SerializeToElement(data);
        var name = IAppHost.GetService<IProfileService>().Profile.Subjects
            .GetValueOrDefault(ProfileBlockHelpers.Guid(settings, "Subject"))?.Initial ?? string.Empty;
        return Task.FromResult<object>(name);
    }
}
