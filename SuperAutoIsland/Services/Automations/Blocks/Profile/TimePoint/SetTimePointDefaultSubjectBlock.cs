using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.TimePoint;

/// <summary>
///     设置时间点的默认科目。
/// </summary>
public class SetTimePointDefaultSubjectBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.setTimePointDefaultSubject";
    public override string Name => "设置时间点默认科目";
    public override (string, string) Icon => ("科目", FluentIcons.BookRegular);

    public override string Tooltip =>
        "设置「上课」时间点的默认科目。所有使用该时间表的课表中，该时间点对应课程的科目为空时会自动填入该科目，" +
        "已有科目不会被覆盖。科目留空则清除默认科目。";

    public override void GetFields(FieldsRegister it)
    {
        it
            .AddField("TimeLayoutItem", ProfileFields.TimeLayoutItem(""))
            .AddField("Subject", ProfileFields.Subject("默认科目"));
    }

    public override Task Handler(ActionItem actionItem)
    {
        var settings = JsonSerializer.SerializeToElement(actionItem.Settings);
        var target = ProfileBlockHelpers.WritableTimePoint(settings);
        if (target is null)
            return Task.CompletedTask;

        var profile = IAppHost.GetService<IProfileService>().Profile;
        var subjectId = ProfileBlockHelpers.Guid(settings, "Subject");
        if (subjectId != Guid.Empty && !profile.Subjects.ContainsKey(subjectId))
            return Task.CompletedTask;

        target.Value.Item.DefaultClassId = subjectId;
        return Task.CompletedTask;
    }
}
