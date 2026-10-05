using System.Text.Json;
using ClassIsland.Core.Icons;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.TimePoint;

/// <summary>
///     删除时间表末尾的最后一个时间点。
/// </summary>
public class DeleteTimePointBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.deleteTimePoint";
    public override string Name => "删除时间点";
    public override (string, string) Icon => ("删除", FluentIcons.DeleteRegular);

    public override string Tooltip =>
        "删除时间表末尾的最后一个时间点。删除「上课」时间点会同时移除所有使用该时间表的课表中对应课程的科目。" +
        "只能删除末尾，以保证时间点的顺序与脚本的顺序一致，也避免时间点标识的序号发生漂移。" +
        "修改仅在内存中生效，需要自行调用「保存档案」积木。";

    public override void GetFields(FieldsRegister it)
    {
        it
            .AddField("TimeLayout", ProfileFields.TimeLayout(""));
    }

    public override Task Handler(ActionItem actionItem)
    {
        var settings = JsonSerializer.SerializeToElement(actionItem.Settings);
        var layout = ProfileBlockHelpers.ClassicTimeLayout(settings);
        if (layout is not null)
        {
            ProfileBlockHelpers.RemoveLastTimePoint(layout);
        }

        return Task.CompletedTask;
    }
}
