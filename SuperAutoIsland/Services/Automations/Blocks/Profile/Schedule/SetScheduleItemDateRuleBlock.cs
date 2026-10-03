using System.Collections.ObjectModel;
using System.Text.Json;
using ClassIsland.Shared.Models.Automation;
using ClassIsland.Shared.Models.Profile;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Schedule;

/// <summary>
///     设置课程的触发规则为「某些日期」。
/// </summary>
public class SetScheduleItemDateRuleBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.setScheduleItemDateRule";
    public override string Name => "设置课程触发规则为";

    public override void GetFields(FieldsRegister it)
    {
        it
            .AddDummy("某天")
            .AddField("ScheduleItem", ProfileFields.ScheduleItem("课程"))
            .AddField("Dates", BasicFields.CreateInputField("启用日期", field =>
            {
                field.Check = "Array";
                field.ShadowBlockType = "lists_create_with";
            }));
    }

    public override Task Handler(ActionItem actionItem)
    {
        var settings = JsonSerializer.SerializeToElement(actionItem.Settings);
        var item = ProfileBlockHelpers.ScheduleItem(settings);
        if (item != null)
        {
            item.EnableRule.Type = TimeRule.TimeRuleType.Date;
            var dates = new ObservableCollection<DateOnly>();
            if (settings.TryGetProperty("Dates", out var values) && values.ValueKind == JsonValueKind.Array)
            {
                foreach (var value in values.EnumerateArray())
                {
                    if (value.ValueKind == JsonValueKind.String && DateOnly.TryParse(value.GetString(), out var date))
                    {
                        dates.Add(date);
                    }
                }
            }

            item.EnableRule.EnableDates = dates;
        }

        return Task.CompletedTask;
    }
}