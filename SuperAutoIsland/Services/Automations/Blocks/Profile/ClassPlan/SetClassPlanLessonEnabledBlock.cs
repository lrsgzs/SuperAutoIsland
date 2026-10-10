using System.Text.Json;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

/// <summary>
///     设置课表中第 N 节课（<c>ClassInfo</c>）是否启用。
///     第 N 节课按课表时间表中第 N 个「上课」时间点计数；仅对经典模式（有 GUID）的课表生效，
///     日程模式生成的临时课表不会被修改。
/// </summary>
public class SetClassPlanLessonEnabledBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.setClassPlanLessonEnabled";
    public override string Name => "课表";
    public override bool InlineBlock => true;
    public override bool InlineField => true;

    public override string Tooltip =>
        "设置课表中第 N 节课是否启用。第 N 节课按时间表中第 N 个「上课」时间点计数；仅对经典模式（有 GUID）的课表生效。" +
        "被禁用的课程在课表组件中显示为删除线，且不会进入上课流程。";

    public override void GetFields(FieldsRegister it)
    {
        it
            .AddField("ClassPlan", ProfileFields.ClassPlan("设置"))
            .AddField("Index", BasicFields.Number("第", 1))
            .AddField("Enabled", BasicFields.Boolean("节课启用", true));
    }

    public override Task Handler(ActionItem actionItem)
    {
        var s = JsonSerializer.SerializeToElement(actionItem.Settings);
        var plan = ProfileBlockHelpers.ClassicClassPlan(s);
        var index = ProfileBlockHelpers.Number(s, "Index") - 1;
        if (plan != null && index >= 0 && index < plan.Classes.Count)
            plan.Classes[index].IsEnabled = ProfileBlockHelpers.Bool(s, "Enabled");
        return Task.CompletedTask;
    }
}
