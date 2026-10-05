using System.Text.Json;
using ClassIsland.Core.Icons;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.TimePoint;

/// <summary>
///     设置时间点的开始时间。
/// </summary>
public class SetTimePointStartTimeBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.setTimePointStartTime";
    public override string Name => "设置时间点开始时间";
    public override (string, string) Icon => ("时间点", FluentIcons.ClockRegular);

    public override string Tooltip =>
        "设置时间点的开始时间。为了不丢失时间点本身的时长，会尽量保持时长整体平移；" +
        "放不下时向内钳制，不会与相邻的「上课/课间」时间点重叠，也不会让开始时间越过结束时间。" +
        "分割线与行动的结束时间会随开始时间一起移动。";

    public override void GetFields(FieldsRegister it)
    {
        it
            .AddField("TimeLayoutItem", ProfileFields.TimeLayoutItem(""))
            .AddField("StartTime", BasicFields.Time("开始时间", TimeSpan.FromHours(8)));
    }

    public override Task Handler(ActionItem actionItem)
    {
        var settings = JsonSerializer.SerializeToElement(actionItem.Settings);
        var target = ProfileBlockHelpers.WritableTimePoint(settings);
        if (target is not null)
        {
            var (layout, item) = target.Value;
            ProfileBlockHelpers.SetTimePointStartTime(layout, item, ProfileBlockHelpers.Time(settings, "StartTime"));
        }

        return Task.CompletedTask;
    }
}
