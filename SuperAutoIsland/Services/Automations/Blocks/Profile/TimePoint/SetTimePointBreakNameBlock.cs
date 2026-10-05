using System.Text.Json;
using ClassIsland.Core.Icons;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.TimePoint;

/// <summary>
///     设置时间点的课间名称。
/// </summary>
public class SetTimePointBreakNameBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.setTimePointBreakName";
    public override string Name => "设置时间点课间名称";
    public override (string, string) Icon => ("课间", FluentIcons.TextFontRegular);

    public override string Tooltip =>
        "设置时间点的自定义课间名称。仅「课间」类型的时间点会显示该名称，留空时显示为「课间休息」。";

    public override void GetFields(FieldsRegister it)
    {
        it
            .AddField("TimeLayoutItem", ProfileFields.TimeLayoutItem(""))
            .AddField("BreakName", BasicFields.Text("课间名称"));
    }

    public override Task Handler(ActionItem actionItem)
    {
        var settings = JsonSerializer.SerializeToElement(actionItem.Settings);
        var target = ProfileBlockHelpers.WritableTimePoint(settings);
        if (target is not null)
        {
            var (_, item) = target.Value;
            item.BreakName = settings.GetProperty("BreakName").GetString() ?? string.Empty;
        }

        return Task.CompletedTask;
    }
}
