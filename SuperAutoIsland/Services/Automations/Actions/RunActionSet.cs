using ClassIsland.Core.Abstractions.Automation;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using SuperAutoIsland.Models.Actions;
using SuperAutoIsland.Shared;

namespace SuperAutoIsland.Services.Automations.Actions;

/// <summary>
/// 「运行可复用的行动组」
/// </summary>
[ActionInfo("sai.actions.runActionSet", "运行可复用的行动组", FluentIcons.AirplaneTakeOffRegular, false)]
public class RunActionSet : ActionBase<RunActionSetSettings>
{
    private static readonly CiRunner Runner = IAppHost.GetService<CiRunner>();
    
    /// <summary>
    /// 行动被触发
    /// </summary>
    protected override async Task OnInvoke()
    {
        await base.OnInvoke();
        if (Settings.ProjectGuid == GlobalConstants.Assets.ProjectNullGuid)
            return;
        
        await Runner.RunActionSetProject(ProjectsConfigManager.GetProject(Settings.ProjectGuid));
    }
}