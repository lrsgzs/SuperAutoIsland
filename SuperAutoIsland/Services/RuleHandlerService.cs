using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using SuperAutoIsland.Models.Rules;
using SuperAutoIsland.Shared;

namespace SuperAutoIsland.Services;

public class RuleHandlerService
{
    private readonly IRulesetService _rulesetService = IAppHost.GetService<IRulesetService>();
    private readonly ILessonsService _lessonsService = IAppHost.GetService<ILessonsService>();

    public RuleHandlerService()
    {
        _rulesetService.RegisterRuleHandler("sai.rules.runCiRuleset", settings =>
        {
            if (settings is not RunCiRulesetSettings s) return false;

            var ciRunner = IAppHost.GetService<CiRunner>();
            
            if (s.ProjectGuid == GlobalConstants.Assets.ProjectNullGuid)
                return false;
            
            var project = ProjectsConfigManager.GetProject(s.ProjectGuid);
            if (project.RulesetState != null)
            {
                return project.RulesetState.Value;
            }
            
            var state = ciRunner.RunRulesetProject(project);
            project.RulesetState = state;
            _rulesetService.StatusUpdated += ClearState;
            
            return state;
            
            void ClearState(object? sender, EventArgs e)
            {
                project.RulesetState = null;
                _rulesetService.StatusUpdated -= ClearState;
            }
        });
    }
}