using System.Text.Json;
using ClassIsland.Core.Icons;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.TimePoint;

/// <summary>
///     设置时间点的结束时间。
/// </summary>
public class SetTimePointEndTimeBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.setTimePointEndTime";
    public override string Name => "设置时间点结束时间";
    public override (string, string) Icon => ("时间点", FluentIcons.ClockRegular);

    public override string Tooltip =>
        "设置时间点的结束时间，并钳制到不与相邻的「上课/课间」时间点重叠、也不早于自身开始时间的范围内。" +
        "分割线与行动没有时长，不做修改。";

    public override void GetFields(FieldsRegister it)
    {
        it
            .AddField("TimeLayoutItem", ProfileFields.TimeLayoutItem(""))
            .AddField("EndTime", BasicFields.Time("结束时间", TimeSpan.FromHours(8) + TimeSpan.FromMinutes(40)));
    }

    public override Task Handler(ActionItem actionItem)
    {
        var settings = JsonSerializer.SerializeToElement(actionItem.Settings);
        var target = ProfileBlockHelpers.WritableTimePoint(settings);
        if (target is not null)
        {
            var (layout, item) = target.Value;
            ProfileBlockHelpers.SetTimePointEndTime(layout, item, ProfileBlockHelpers.Time(settings, "EndTime"));
        }

        return Task.CompletedTask;
    }
}
