using System.Text.Json;
using Avalonia.Threading;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Enums;
using ClassIsland.Core.Models.Ruleset;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Services;

public class SaiBlockRunner(IActionService actionService, IRulesetService rulesetService)
{
    private readonly Logger<SaiBlockRunner> _logger = new();

    public async Task RunAction(string id, JsonElement settings)
    {
        _logger.Debug($"运行行动 {id}");

        if ((await RunPrefixHandlerAsync(BlockKind.Action, id, settings)).Handled)
        {
            _logger.Debug($"行动 {id} 已被前缀处理器处理");
            return;
        }

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

    public async Task<bool> RunRule(string id, JsonElement settings)
    {
        _logger.Debug($"运行规则 {id}");

        var prefixResult = await RunPrefixHandlerAsync(BlockKind.Rule, id, settings);
        if (prefixResult.Handled)
        {
            _logger.Debug($"规则 {id} 已被前缀处理器处理");

            if (prefixResult.Result is bool ruleResult)
            {
                return ruleResult;
            }

            _logger.Warn($"前缀处理器对规则 {id} 的返回结果不是布尔值，按 false 处理");
            return false;
        }

        var rule = new Rule
        {
            IsReversed = false,
            Id = id,
            Settings = settings.Deserialize<object>()
        };
        var block = SaiBlocksRegistry.Blocks.GetValueOrDefault(id) as RuleBlockBase;

        if (block != null)
        {
            rule = block.Wrapper(rule);
        }

        _logger.BaseLog("TRACE", $"Id: {rule.Id} Settings: {JsonSerializer.Serialize(rule.Settings)}");

        var result = await Dispatcher.UIThread.InvokeAsync(() =>
        {
            if (block != null)
            {
                return block.Handler(rule);
            }

            return rulesetService.IsRulesetSatisfied(new Ruleset
            {
                Mode = RulesetLogicalMode.And,
                IsReversed = false,
                Groups =
                [
                    new RuleGroup
                    {
                        Rules = [rule]
                    }
                ]
            });
        });

        _logger.Debug($"规则 {id} 运行完毕，结果：{result}");
        return result;
    }

    public async Task<object> RunData(string id, JsonElement settings)
    {
        _logger.Debug($"运行数据 {id}");

        var prefixResult = await RunPrefixHandlerAsync(BlockKind.Data, id, settings);
        if (prefixResult.Handled)
        {
            _logger.Debug($"数据 {id} 已被前缀处理器处理");
            _logger.Debug($"数据 {id} 运行完毕，结果：{prefixResult.Result}");
            return prefixResult.Result!;
        }

        if (SaiBlocksRegistry.Blocks.GetValueOrDefault(id) is not DataBlockBase block)
        {
            return "???";
        }

        var data = settings.Deserialize(block.SettingsType);
        var result = await Dispatcher.UIThread.InvokeAsync(async () =>
                                                               await block.Handler(data));

        _logger.Debug($"数据 {id} 运行完毕，结果：{result}");
        return result;
    }

    /// <summary>
    ///     尝试由前缀处理器处理积木调用（在 ui 线程运行）
    /// </summary>
    /// <param name="kind">积木类型</param>
    /// <param name="id">积木 id</param>
    /// <param name="settings">积木设置</param>
    /// <returns>处理结果，Handled 为 false 时表示未处理，应走原处理逻辑</returns>
    private static async Task<(bool Handled, object? Result)> RunPrefixHandlerAsync(
        BlockKind kind, string id, JsonElement settings)
    {
        var handler = SaiBlocksRegistry.ResolvePrefixHandler(id);
        if (handler == null)
        {
            return (false, null);
        }

        return await Dispatcher.UIThread.InvokeAsync(() => handler(kind, id, settings));
    }
}