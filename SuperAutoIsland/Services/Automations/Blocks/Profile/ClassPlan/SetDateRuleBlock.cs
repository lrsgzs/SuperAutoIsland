using System.Text.Json;
using System.Collections.ObjectModel;
using ClassIsland.Shared.Models.Automation;
using ClassIsland.Shared.Models.Profile;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

public class SetDateRuleBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.setDateRule";
    public override string Name => "设置课表触发规则为";
    public override void GetFields(FieldsRegister it) => it
        .AddDummy("某天")
        .AddField("ClassPlan", ProfileFields.ClassPlan("课表"))
        .AddField("Dates", BasicFields.CreateInputField("启用日期", field =>
        {
            field.Check = "Array";
            field.ShadowBlockType = "lists_create_with";
        }));
    public override Task Handler(ActionItem actionItem)
    {
        var s = JsonSerializer.SerializeToElement(actionItem.Settings);
        var plan = ProfileBlockHelpers.ClassPlan(s);
        if (plan != null)
        {
            plan.TimeRule.Type = TimeRule.TimeRuleType.Date;
            var dates = new ObservableCollection<DateOnly>();
            if (s.TryGetProperty("Dates", out var values) && values.ValueKind == JsonValueKind.Array)
            {
                foreach (var value in values.EnumerateArray())
                {
                    if (value.ValueKind == JsonValueKind.String && DateOnly.TryParse(value.GetString(), out var date))
                    {
                        dates.Add(date);
                    }
                }
            }
            plan.TimeRule.EnableDates = dates;
        }
        return Task.CompletedTask;
    }
}
