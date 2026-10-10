using System.Text.Json;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

/// <summary>
///     读取课表中第 N 节课（<c>ClassInfo</c>）是否启用。
///     第 N 节课按课表时间表中第 N 个「上课」时间点计数；课表或课程不存在时返回 false。
/// </summary>
public class GetClassPlanLessonEnabledBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.classPlanLessonEnabled";
    public override string Name => "课表";
    public override string DataOutput => "Boolean";
    public override bool InlineBlock => true;
    public override bool InlineField => true;

    public override string Tooltip =>
        "读取课表中第 N 节课是否启用。第 N 节课按时间表中第 N 个「上课」时间点计数；课表或课程不存在时返回「假」。";

    public override void GetFields(FieldsRegister it)
    {
        it
            .AddField("ClassPlan", ProfileFields.ClassPlan("获取"))
            .AddField("Index", BasicFields.Number("第", 1))
            .AddDummy("节课是否启用?");
    }

    public override Task<object> Handler(object? data)
    {
        var settings = JsonSerializer.SerializeToElement(data);
        var plan = ProfileBlockHelpers.ClassPlan(settings);
        var index = (int)settings.GetProperty("Index").GetDouble() - 1;
        var enabled = plan is not null
                      && index >= 0
                      && index < plan.Classes.Count
                      && plan.Classes[index].IsEnabled;
        return Task.FromResult<object>(enabled);
    }
}
