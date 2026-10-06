using ClassIsland.Core.Abstractions.Automation;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using SuperAutoIsland.Models.Actions;
using SuperAutoIsland.Shared;

namespace SuperAutoIsland.Services.Automations.Actions;

/// <summary>
///     「运行 JavaScript 行动」
/// </summary>
[ActionInfo("sai.actions.runJavaScript", "运行 JavaScript 行动", FluentIcons.JavascriptRegular, false)]
public class RunJavaScriptAction : ActionBase<RunJavaScriptActionSettings>
{
    private static readonly BlocklyRunner.BlocklyRunner Runner = IAppHost.GetService<BlocklyRunner.BlocklyRunner>();

    /// <summary>
    ///     行动被触发
    /// </summary>
    protected override async Task OnInvoke()
    {
        await base.OnInvoke();
        if (Settings.ProjectGuid == GlobalConstants.Assets.ProjectNullGuid)
            return;

        await Runner.RunJavaScriptProject(
            ProjectsConfigManager.GetProject(Settings.ProjectGuid),
            InterruptCancellationToken);
    }
}
