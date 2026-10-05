using System.Text.Json;
using ClassIsland.Core.Icons;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.TimePoint;

/// <summary>
///     设置时间点的类型。
/// </summary>
public class SetTimePointTypeBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.setTimePointType";
    public override string Name => "设置时间点类型";
    public override (string, string) Icon => ("类型", FluentIcons.TagRegular);

    public override string Tooltip =>
        "设置时间点的类型（0-上课，1-课间，2-分割线，3-行动）。" +
        "注意：类型转换会同步增删课表中的课程。把「上课」改成其他类型会移除该时间点对应课程的科目；" +
        "把其他类型改成「上课」会给所有使用该时间表的课表插入一节没有科目的课。" +
        "把「分割线」或「行动」改成「上课」或「课间」后时长为 0，需要再用「设置时间点结束时间」调整。";

    public override void GetFields(FieldsRegister it)
    {
        it
            .AddField("TimeLayoutItem", ProfileFields.TimeLayoutItem(""))
            .AddField("TimeType", ProfileFields.TimePointTypeInput("类型"));
    }

    public override Task Handler(ActionItem actionItem)
    {
        var settings = JsonSerializer.SerializeToElement(actionItem.Settings);
        var target = ProfileBlockHelpers.WritableTimePoint(settings);
        if (target is null)
            return Task.CompletedTask;

        var (_, item) = target.Value;
        var timeType = Math.Clamp(ProfileBlockHelpers.Number(settings, "TimeType"), 0, 3);
        if (item.TimeType == timeType)
            return Task.CompletedTask;

        item.TimeType = timeType;

        // 分割线与行动没有时长，转换后需要把结束时间对齐到开始时间。
        if (timeType is 2 or 3)
            item.EndTime = item.StartTime;

        return Task.CompletedTask;
    }
}
