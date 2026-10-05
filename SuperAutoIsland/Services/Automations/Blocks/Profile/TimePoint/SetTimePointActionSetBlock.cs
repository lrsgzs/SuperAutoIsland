using System.Text.Json;
using ClassIsland.Core.Icons;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Models.Actions;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;
using SuperAutoIsland.Shared;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.TimePoint;

/// <summary>
///     给「行动」时间点挂上一个可复用的行动组。
/// </summary>
public class SetTimePointActionSetBlock : ActionBlockBase
{
    /// <summary>
    ///     「运行可复用的行动组」行动的 ID。
    /// </summary>
    private const string RunActionSetActionId = "sai.actions.runActionSet";

    public override string Id => "sai.profile.actions.setTimePointActionSet";
    public override string Name => "设置时间点行动组";
    public override (string, string) Icon => ("行动组", FluentIcons.PlayRegular);

    public override string Tooltip =>
        "给时间点挂上一个「可复用的行动组」行动。需要在时间点类型为「行动」时才会被触发。";

    public override void GetFields(FieldsRegister it)
    {
        it
            .AddField("TimeLayoutItem", ProfileFields.TimeLayoutItem(""))
            .AddField("ActionSet", ProfileFields.ActionSet("可复用的行动组"));
    }

    public override Task Handler(ActionItem actionItem)
    {
        var settings = JsonSerializer.SerializeToElement(actionItem.Settings);
        var target = ProfileBlockHelpers.WritableTimePoint(settings);
        if (target is null)
            return Task.CompletedTask;

        var (_, item) = target.Value;
        var projectId = ProfileBlockHelpers.Guid(settings, "ActionSet");

        // 空占位项表示清除已有的行动组。
        if (projectId == Guid.Empty || projectId == GlobalConstants.Assets.ProjectNullGuid)
        {
            item.ActionSet = null;
            return Task.CompletedTask;
        }

        var project = GlobalConstants.Configs.ProjectConfig?.Data.Projects.FirstOrDefault(x => x.Id == projectId);
        if (project is null)
            return Task.CompletedTask;

        // 只挂一个引用行动组的行动，行动本身的修改会直接作用到可复用的行动组上。
        item.ActionSet = new ActionSet
        {
            Name = $"时间点行动 - {project.Name}",
            ActionItems =
            [
                new ActionItem
                {
                    Id = RunActionSetActionId,
                    Settings = new RunActionSetSettings { ProjectGuid = projectId }
                }
            ]
        };

        return Task.CompletedTask;
    }
}
