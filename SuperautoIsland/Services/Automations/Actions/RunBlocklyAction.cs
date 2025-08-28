using ClassIsland.Core.Abstractions.Automation;
using ClassIsland.Core.Attributes;
using ClassIsland.Shared;
using SuperAutoIsland.Models.Actions;

namespace SuperAutoIsland.Services.Automations.Actions;

[ActionInfo("sai.actions.runBlockly", "运行 Blockly 项目", "\uf44f", false)]
public class RunBlocklyAction : ActionBase<RunBlocklyActionSettings>
{
    private static readonly BlocklyRunner.BlocklyRunner Runner = IAppHost.GetService<BlocklyRunner.BlocklyRunner>();
    
    protected override async Task OnInvoke()
    {
        await base.OnInvoke();
        Runner.RunProject(ProjectsConfigManager.GetProject(Settings.ProjectGuid));
    }
}