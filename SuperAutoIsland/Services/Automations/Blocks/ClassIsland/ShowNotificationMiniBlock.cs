using ClassIsland.Core.Icons;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;

namespace SuperAutoIsland.Services.Automations.Blocks.ClassIsland;

/// <summary>
/// 「显示提醒」的简洁版：字段与 <c>classisland.showNotification</c> 对齐，但去掉了「高级设置」部分。
/// 运行时通过 <see cref="Wrapper"/> 转发到真正的 <c>classisland.showNotification</c> 行动。
/// </summary>
public class ClassIslandShowNotificationMiniBlock : ActionBlockBase
{
    public override string Id => "classisland.showNotification.mini";
    public override string Name => "显示提醒";
    public override (string, string) Icon => ("提醒", "\uE02B");

    public override void GetFields(FieldsRegister it) => it
        .AddDummy("内容设置:")
        .AddField("Mask", BasicFields.Text("标题内容"))
        .AddField("MaskDurationSeconds", BasicFields.Number("标题持续时间(秒)", 5))
        .AddField("IsLeftIconEnabled", BasicFields.Boolean("启用标题左侧图标", true))
        .AddField("LeftIcon", BasicFields.Icon("标题左侧图标", $"lucide({LucideIcons.Info})"))
        .AddField("IsRightIconEnabled", BasicFields.Boolean("启用标题右侧图标", false))
        .AddField("RightIcon", BasicFields.Icon("标题右侧图标", $"lucide({LucideIcons.BellRing})"))
        .AddField("Content", BasicFields.Text("正文内容"))
        .AddField("ContentDurationSeconds", BasicFields.Number("正文持续时长(秒)", 10))
        .AddField("IsMaskSpeechEnabled", BasicFields.Boolean("启用标题语音", true))
        .AddField("IsContentSpeechEnabled", BasicFields.Boolean("启用正文语音", true))
        .AddField("IsWaitForCompleteEnabled", BasicFields.Boolean("等待提醒结束", false));

    public override ActionItem Wrapper(ActionItem actionItem) => new()
    {
        Id = "classisland.showNotification",
        Settings = actionItem.Settings
    };
}
