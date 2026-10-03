using System.Text.Json;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Subject;

/// <summary>
///     设置科目地点。
/// </summary>
public class SetSubjectLocationBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.setSubjectLocation";
    public override string Name => "设置科目地点";

    public override void GetFields(FieldsRegister it)
    {
        it
            .AddField("Subject", ProfileFields.Subject(""))
            .AddField("Location", BasicFields.Text("地点"));
    }

    public override Task Handler(ActionItem actionItem)
    {
        var settings = JsonSerializer.SerializeToElement(actionItem.Settings);
        var subject = ProfileBlockHelpers.Subject(settings);
        if (subject != null)
        {
            subject.Location = settings.GetProperty("Location").GetString() ?? string.Empty;
        }

        return Task.CompletedTask;
    }
}