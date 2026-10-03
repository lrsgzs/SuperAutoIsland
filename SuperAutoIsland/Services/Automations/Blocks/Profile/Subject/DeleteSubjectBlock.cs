using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Subject;

/// <summary>
/// 删除指定科目。
/// </summary>
public class DeleteSubjectBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.deleteSubject";
    public override string Name => "删除科目";
    public override string Tooltip => "删除指定科目。课表中仍引用该科目的课程会失去科目信息。";

    public override void GetFields(FieldsRegister it) => it
        .AddField("Subject", ProfileFields.Subject(""));

    public override Task Handler(ActionItem actionItem)
    {
        var settings = JsonSerializer.SerializeToElement(actionItem.Settings);
        var profile = IAppHost.GetService<IProfileService>().Profile;
        var id = ProfileBlockHelpers.Guid(settings, "Subject");
        if (id == Guid.Empty)
        {
            return Task.CompletedTask;
        }

        profile.Subjects.Remove(id);
        return Task.CompletedTask;
    }
}
