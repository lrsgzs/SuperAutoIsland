using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Subject;

public class GetSubjectLocationBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.subjectLocation";
    public override string Name => "科目地点";

    public override void GetFields(FieldsRegister it) => it
        .AddField("Subject", ProfileFields.Subject(""));

    public override Task<object> Handler(object? data)
    {
        var settings = JsonSerializer.SerializeToElement(data);
        var location = IAppHost.GetService<IProfileService>().Profile.Subjects
            .GetValueOrDefault(ProfileBlockHelpers.Guid(settings, "Subject"))?.Location ?? string.Empty;
        return Task.FromResult<object>(location);
    }
}
