using System.Text.Json;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Schedule;

/// <summary>
///     设置课程的科目。
/// </summary>
public class SetScheduleItemSubjectBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.setScheduleItemSubject";
    public override string Name => "设置课程科目";

    public override void GetFields(FieldsRegister it)
    {
        it
            .AddField("ScheduleItem", ProfileFields.ScheduleItem(""))
            .AddField("Subject", ProfileFields.Subject(""));
    }

    public override Task Handler(ActionItem actionItem)
    {
        var settings = JsonSerializer.SerializeToElement(actionItem.Settings);
        var item = ProfileBlockHelpers.ScheduleItem(settings);
        if (item != null)
        {
            item.SubjectId = ProfileBlockHelpers.Guid(settings, "Subject");
        }

        return Task.CompletedTask;
    }
}