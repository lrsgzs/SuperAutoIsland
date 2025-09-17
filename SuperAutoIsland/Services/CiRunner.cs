using Avalonia.Threading;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Enums;
using SuperAutoIsland.Models;

namespace SuperAutoIsland.Services;

public class CiRunner
{
    private readonly IRulesetService _rulesetService = IAppHost.GetService<IRulesetService>();

    public bool RunRulesetProject(Project project)
    {
        if (project.Type == ProjectsType.CiRuleset)
        {
            return _rulesetService.IsRulesetSatisfied(project.Ruleset);
        }

        throw new NotSupportedException();
    }
    
    public async Task RunActionSetProject(Project project)
    {
        if (project.Type == ProjectsType.CiActionSet)
        {
            var actionService = IAppHost.GetService<IActionService>();
            
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await actionService.InvokeActionSetAsync(new ActionSet
                {
                    Name = $"SAI 行动组 - {project.Name}",
                    ActionItems = project.Actions
                });
            });
            
            return;
        }

        throw new NotSupportedException();
    }
}