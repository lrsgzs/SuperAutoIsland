using System.Text.Json;
using ClassIsland.Core.Icons;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.TimePoint;

/// <summary>
///     设置时间点是否默认隐藏。
/// </summary>
public class SetTimePointHideDefaultBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.setTimePointHideDefault";
    public override string Name => "设置时间点默认隐藏";
    public override (string, string) Icon => ("隐藏", FluentIcons.EyeRegular);

    public override string Tooltip => "设置时间点是否在课表组件中默认隐藏。被选中的时间点仍会正常显示。";

    public override void GetFields(FieldsRegister it)
    {
        it
            .AddField("TimeLayoutItem", ProfileFields.TimeLayoutItem(""))
            .AddField("IsHideDefault", BasicFields.Boolean("默认隐藏"));
    }

    public override Task Handler(ActionItem actionItem)
    {
        var settings = JsonSerializer.SerializeToElement(actionItem.Settings);
        var target = ProfileBlockHelpers.WritableTimePoint(settings);
        if (target is not null)
        {
            target.Value.Item.IsHideDefault = ProfileBlockHelpers.Bool(settings, "IsHideDefault");
        }

        return Task.CompletedTask;
    }
}
