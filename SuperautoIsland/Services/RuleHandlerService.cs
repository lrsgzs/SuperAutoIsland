using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using SuperAutoIsland.Models.Rules;

namespace SuperAutoIsland.Services;

public class RuleHandlerService
{
    private readonly IRulesetService _rulesetService = IAppHost.GetService<IRulesetService>();

    public RuleHandlerService()
    {
        _rulesetService.RegisterRuleHandler("sai.rules.runCiRuleset", settings =>
        {
            if (settings is not RunCiRulesetSettings s) return false;

            var ciRunner = IAppHost.GetService<CiRunner>();
            return ciRunner.RunRulesetProject(ProjectsConfigManager.GetProject(s.ProjectGuid));
        });
    }
}