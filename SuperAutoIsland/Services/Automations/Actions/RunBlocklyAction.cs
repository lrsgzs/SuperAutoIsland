using ClassIsland.Core.Abstractions.Automation;
using ClassIsland.Core.Attributes;
using ClassIsland.Shared;
using SuperAutoIsland.Models.Actions;

namespace SuperAutoIsland.Services.Automations.Actions;

/// <summary>
/// 「运行 Blockly 项目」
/// </summary>
[ActionInfo("sai.actions.runBlockly", "运行 Blockly 项目", "\uE049", false)]
public class RunBlocklyAction : ActionBase<RunBlocklyActionSettings>
{
    private static readonly BlocklyRunner.BlocklyRunner Runner = IAppHost.GetService<BlocklyRunner.BlocklyRunner>();
    
    /// <summary>
    /// 行动被触发
    /// </summary>
    protected override async Task OnInvoke()
    {
        await base.OnInvoke();
        Runner.RunActionProject(ProjectsConfigManager.GetProject(Settings.ProjectGuid));
    }
}