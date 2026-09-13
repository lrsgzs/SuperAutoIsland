using System.Text.Json;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

public class SetClassPlanEnabledBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.setClassPlanEnabled";
    public override string Name => "设置自动启用";
    public override bool InlineBlock => true;
    public override bool InlineField => true;
    public override void GetFields(FieldsRegister it) => it
        .AddField("ClassPlan", ProfileFields.ClassPlan(""))
        .AddField("Enabled", BasicFields.Boolean("自动启用", true));
    public override Task Handler(ActionItem actionItem)
    {
        var s = JsonSerializer.SerializeToElement(actionItem.Settings);
        var plan = ProfileBlockHelpers.ClassicClassPlan(s);
        if (plan != null) plan.IsEnabled = ProfileBlockHelpers.Bool(s, "Enabled");
        return Task.CompletedTask;
    }
}
