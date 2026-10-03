using System.Text.Json;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Subject;

/// <summary>
/// 设置科目科任老师。
/// </summary>
public class SetSubjectTeacherNameBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.setSubjectTeacherName";
    public override string Name => "设置科任老师";

    public override void GetFields(FieldsRegister it) => it
        .AddField("Subject", ProfileFields.Subject(""))
        .AddField("TeacherName", BasicFields.Text("老师姓名"));

    public override Task Handler(ActionItem actionItem)
    {
        var settings = JsonSerializer.SerializeToElement(actionItem.Settings);
        var subject = ProfileBlockHelpers.Subject(settings);
        if (subject != null)
        {
            subject.TeacherName = settings.GetProperty("TeacherName").GetString() ?? string.Empty;
        }

        return Task.CompletedTask;
    }
}
