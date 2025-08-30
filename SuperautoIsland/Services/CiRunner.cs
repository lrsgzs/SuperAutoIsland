using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
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
}