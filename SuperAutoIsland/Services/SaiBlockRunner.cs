using System.Text.Json;
using Avalonia.Threading;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Enums;
using ClassIsland.Core.Models.Ruleset;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Services;

public class SaiBlockRunner(IActionService actionService, IRulesetService rulesetService)
{
    private readonly Logger<SaiBlockRunner> _logger = new();
    
    public async Task RunAction(string id, JsonElement settings)
    {
        _logger.Debug($"运行行动 {id}");
        
        var action = new ActionItem
        {
            Id = id,
            Settings = settings.Deserialize<object>()
        };
        var block = SaiBlocksRegistry.Blocks.GetValueOrDefault(id) as ActionBlockBase;
        
        if (block != null)
        {
            action = block.Wrapper(action);
        }
        
        _logger.BaseLog("TRACE", $"Id: {action.Id} Settings: {JsonSerializer.Serialize(action.Settings)}");
        
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            if (block != null)
            {
                await block.Handler(action);
                return;
            }
            
            await actionService.InvokeActionSetAsync(new ActionSet
            {
                Name = "SAI 临时行动组",
                ActionItems = [action]
            });
        });
        
        _logger.Debug($"行动 {id} 运行完毕");
    }

    public bool RunRule(string id, JsonElement settings)
    {
        _logger.Debug($"运行规则 {id}");

        var rule = new Rule
        {
            IsReversed = false,
            Id = id,
            Settings = settings.Deserialize<object>(),
        };
        var block = SaiBlocksRegistry.Blocks.GetValueOrDefault(id) as RuleBlockBase;

        if (block != null)
        {
            rule = block.Wrapper(rule);
        }
        
        _logger.BaseLog("TRACE", $"Id: {rule.Id} Settings: {JsonSerializer.Serialize(rule.Settings)}");

        bool result;
        if (block != null)
        {
            result = block.Handler(rule);
        }
        else
        {
            result = rulesetService.IsRulesetSatisfied(new Ruleset
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
        }
        
        _logger.Debug($"规则 {id} 运行完毕，结果：{result}");
        return result;
    }

    public async Task<object> RunData(string id, JsonElement settings)
    {
        _logger.Debug($"运行数据 {id}");

        var block = SaiBlocksRegistry.Blocks.GetValueOrDefault(id) as DataBlockBase;

        if (block == null)
        {
            return "???";
        }

        var data = settings.Deserialize(block.SettingsType);
        return await block.Handler(data);
    }
}