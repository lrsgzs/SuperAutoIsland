using System.Text.Json;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Subject;

/// <summary>
///     设置科目简称。
/// </summary>
public class SetSubjectInitialBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.setSubjectInitial";
    public override string Name => "设置科目简称";

    public override void GetFields(FieldsRegister it)
    {
        it
            .AddField("Subject", ProfileFields.Subject(""))
            .AddField("Initial", BasicFields.Text("简称"));
    }

    public override Task Handler(ActionItem actionItem)
    {
        var settings = JsonSerializer.SerializeToElement(actionItem.Settings);
        var subject = ProfileBlockHelpers.Subject(settings);
        if (subject != null)
        {
            subject.Initial = settings.GetProperty("Initial").GetString() ?? string.Empty;
        }

        return Task.CompletedTask;
    }
}