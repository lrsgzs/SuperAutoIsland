using System.Text.Json;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Subject;

/// <summary>
///     设置科目图标。图标以图标表达式（如「lucide("\uE551")」）的形式保存。
/// </summary>
public class SetSubjectIconBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.setSubjectIcon";
    public override string Name => "设置科目图标";
    public override string Tooltip => "设置科目的图标。图标以图标表达式（如「lucide(\"\uE551\")」）的形式保存，留空可以清除图标。";

    public override void GetFields(FieldsRegister it)
    {
        it
            .AddField("Subject", ProfileFields.Subject(""))
            .AddField("Icon", BasicFields.Icon("图标"));
    }

    public override Task Handler(ActionItem actionItem)
    {
        var settings = JsonSerializer.SerializeToElement(actionItem.Settings);
        var subject = ProfileBlockHelpers.Subject(settings);
        if (subject != null)
        {
            subject.Icon = settings.GetProperty("Icon").GetString() ?? string.Empty;
        }

        return Task.CompletedTask;
    }
}