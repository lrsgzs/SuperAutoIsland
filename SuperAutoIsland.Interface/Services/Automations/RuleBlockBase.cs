using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Enums;
using ClassIsland.Core.Models.Ruleset;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Metadata;

namespace SuperAutoIsland.Interface.Services.Automations;

public abstract class RuleBlockBase : BlockBase
{
    public override BlockKind Kind => BlockKind.Rule;
    
    public virtual Rule Wrapper(Rule rule)
    {
        return rule;
    }
    
    /// <summary>
    /// 不一定会在 ui 线程运行
    /// </summary>
    /// <param name="rule">规则项</param>
    public virtual bool Handler(Rule rule)
    {
        var rulesetAction = IAppHost.GetService<IRulesetService>();
        var result = rulesetAction.IsRulesetSatisfied(new Ruleset
        {
            Mode = RulesetLogicalMode.And,
            IsReversed = false,
            Groups =
            [
                new RuleGroup
                {
                    Rules = [rule],
                }
            ]
        });
        
        return result;  
    }
}