using System.Text.Json;
using Avalonia.Media;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Subject;

/// <summary>
/// 设置科目主题色。颜色无效时不做修改。
/// </summary>
public class SetSubjectColorBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.setSubjectColor";
    public override string Name => "设置科目主题色";
    public override string Tooltip => "设置科目的主题色。颜色无效时不做修改。";

    public override void GetFields(FieldsRegister it) => it
        .AddField("Subject", ProfileFields.Subject(""))
        .AddField("Color", BasicFields.Color("颜色"));

    public override Task Handler(ActionItem actionItem)
    {
        var settings = JsonSerializer.SerializeToElement(actionItem.Settings);
        var subject = ProfileBlockHelpers.Subject(settings);
        var colorHex = settings.GetProperty("Color").GetString();
        if (subject != null && Color.TryParse(colorHex, out _))
        {
            subject.ColorHex = colorHex!;
        }

        return Task.CompletedTask;
    }
}
