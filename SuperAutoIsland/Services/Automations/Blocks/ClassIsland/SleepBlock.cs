using System.Text.Json;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;

namespace SuperAutoIsland.Services.Automations.Blocks.ClassIsland;

public class ClassIslandSleepBlock : ActionBlockBase
{
    public override string Id => "classisland_action_sleep";
    public override string Name => "等待时长";
    public override (string, string) Icon => ("沙漏", "\uE9AE");
    public override bool InlineBlock => true;
    public override bool InlineField => true;

    public override void GetFields(FieldsRegister it) => it
        .AddField("SECONDS", BasicFields.Number("", 5))
        .AddDummy("秒");

    public override ActionItem Wrapper(ActionItem actionItem)
    {
        var settings = JsonSerializer.SerializeToElement(actionItem.Settings);
        var seconds = settings.TryGetProperty("SECONDS", out var value)
            ? value.GetDouble()
            : 0;

        return new ActionItem
        {
            Id = "classisland.action.sleep",
            Settings = new { Value = seconds }
        };
    }
}
