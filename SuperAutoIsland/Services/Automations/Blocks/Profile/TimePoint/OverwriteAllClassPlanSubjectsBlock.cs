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
///     把时间点在所有使用同一时间表的课表中对应的课程科目，统一覆盖为指定科目。
/// </summary>
public class OverwriteAllClassPlanSubjectsBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.overwriteAllClassPlanSubjects";
    public override string Name => "覆盖所有课表的科目";
    public override (string, string) Icon => ("覆盖", FluentIcons.LayerRegular);

    public override string Tooltip =>
        "把该时间点在所有使用同一时间表的课表中对应课程的科目，统一覆盖为指定科目。" +
        "已有科目会被覆盖；科目留空则把对应课程清空为没有科目。科目对不上科目或时间点时不做修改。" +
        "修改仅在内存中生效，需要自行调用「保存档案」积木。";

    public override void GetFields(FieldsRegister it)
    {
        it
            .AddField("TimeLayoutItem", ProfileFields.TimeLayoutItem(""))
            .AddField("Subject", ProfileFields.Subject("科目"));
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

        var (layout, item) = target.Value;
        var layoutId = profile.TimeLayouts.FirstOrDefault(x => ReferenceEquals(x.Value, layout)).Key;
        if (layoutId == Guid.Empty)
            return Task.CompletedTask;

        profile.OverwriteAllClassPlanSubject(layoutId, item, subjectId);
        return Task.CompletedTask;
    }
}
