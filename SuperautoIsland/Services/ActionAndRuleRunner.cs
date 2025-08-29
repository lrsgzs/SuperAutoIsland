using System.Text.Json;
using Avalonia.Threading;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Enums;
using ClassIsland.Core.Models.Ruleset;
using ClassIsland.Shared;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Services;

/// <summary>
/// 行动、规则 运行器
/// </summary>
public class ActionAndRuleRunner
{
    public static readonly Dictionary<string, ActionWrapper> ActionWrappers = new();
    public static readonly Dictionary<string, RuleWrapper> RuleWrappers = new();

    private readonly Logger<ActionAndRuleRunner> _logger = new();

    private static ActionItem DefaultActionWrapper(ActionItem action) => action;
    private static Rule DefaultRuleWrapper(Rule rule) => rule;
    
    /// <summary>
    /// 行动项目获取方法
    /// </summary>
    /// <param name="id">行动 id</param>
    /// <param name="settings">行动 settings</param>
    /// <returns>行动项目</returns>
    private ActionItem? ActionGetter(string id, JsonElement settings)
    {
        var item = new
        {
            Id = id,
            Settings = settings.Deserialize<object>(),
        };
        var itemJson = JsonSerializer.Serialize(item);
        var action = JsonSerializer.Deserialize<ActionItem>(itemJson);

        if (action is null) return null;

        var actionWrapper = ActionWrappers.GetValueOrDefault(id, DefaultActionWrapper);
        action = actionWrapper(action);

        _logger.BaseLog("TRACE", $"Id: {action.Id} Settings: {JsonSerializer.Serialize(action.Settings)}");
        return action;
    }
    
    /// <summary>
    /// 规则项目获取方法
    /// </summary>
    /// <param name="id">规则 id</param>
    /// <param name="settings">规则 settings</param>
    /// <returns>规则项目</returns>
    private Rule? RuleGetter(string id, JsonElement settings)
    {
        var item = new
        {
            IsReversed = false,
            Id = id,
            Settings = settings.Deserialize<object>(),
        };
        var itemJson = JsonSerializer.Serialize(item);
        var rule = JsonSerializer.Deserialize<Rule>(itemJson);
        
        if (rule is null) return null;

        var ruleWrapper = RuleWrappers.GetValueOrDefault(id, DefaultRuleWrapper);
        rule = ruleWrapper(rule);
        
        _logger.BaseLog("TRACE", $"Id: {rule.Id} Settings: {JsonSerializer.Serialize(rule.Settings)}");
        return rule;
    }

    /// <summary>
    /// 运行行动
    /// </summary>
    /// <param name="id">行动 id</param>
    /// <param name="settings">行动 settings</param>
    public async Task RunAction(string id, JsonElement settings)
    {
        _logger.Debug($"运行行动 {id}");

        var actionItem = ActionGetter(id, settings);
        if (actionItem == null) return;
        _logger.BaseLog("TRACE", $"Action: {JsonSerializer.Serialize(actionItem)}");

        var actionService = IAppHost.GetService<IActionService>();
        
        // 显示提醒需要在 UIThread 完成。
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            await actionService.InvokeActionSetAsync(new ActionSet
            {
                Name = "SAI 临时行动组",
                ActionItems = [actionItem]
            });
        });
        
        _logger.Debug($"行动 {id} 运行完毕");
    }
    
    /// <summary>
    /// 获取规则状态
    /// </summary>
    /// <param name="id">规则 id</param>
    /// <param name="settings">规则 settings</param>
    /// <returns>规则状态</returns>
    public bool RunRule(string id, JsonElement settings)
    {
        _logger.Debug($"运行规则 {id}");
        
        var rule = RuleGetter(id, settings);
        if (rule == null) return false;

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
        
        _logger.Debug($"规则 {id} 运行完毕，结果：{result}");
        return result;
    }
}