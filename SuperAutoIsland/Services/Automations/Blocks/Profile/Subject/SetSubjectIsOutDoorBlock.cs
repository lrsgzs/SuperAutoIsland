using System.Text.Json;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Subject;

/// <summary>
/// 设置科目是否为户外课程。
/// </summary>
public class SetSubjectIsOutDoorBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.setSubjectIsOutDoor";
    public override string Name => "设置科目是否户外";

    public override void GetFields(FieldsRegister it) => it
        .AddField("Subject", ProfileFields.Subject(""))
        .AddField("IsOutDoor", BasicFields.Boolean("是户外课程?"));

    public override Task Handler(ActionItem actionItem)
    {
        var settings = JsonSerializer.SerializeToElement(actionItem.Settings);
        var subject = ProfileBlockHelpers.Subject(settings);
        if (subject != null)
        {
            subject.IsOutDoor = ProfileBlockHelpers.Bool(settings, "IsOutDoor");
        }

        return Task.CompletedTask;
    }
}
