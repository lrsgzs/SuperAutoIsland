using SuperAutoIsland.Interface;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Models;
using SuperAutoIsland.Shared;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Services;

/// <summary>
/// 服务器桥接器
/// </summary>
public class SaiServerBridger : ISaiServer
{
    private readonly SaiServer _instance;
    private readonly Logger<SaiServerBridger> _logger = new();

    internal static readonly Dictionary<string, ActionHandler> ActionHandlers = [];
    internal static readonly Dictionary<string, RuleHandler> RuleHandlers = [];
    
    /// <summary>
    /// 构造函数
    /// <see cref="SaiServerBridger"/>
    /// </summary>
    public SaiServerBridger()
    {
        _instance = new SaiServer(GlobalConstants.Configs.MainConfig!.Data.ServerPort);
        _logger.Info($"服务器地址：{_instance.Url}");
        _ = _instance.Serve();
        
        _logger.Info("已初始化 SaiServer！");
    }
    
    /// <inheritdoc />
    public void RegisterBlocks(string pluginName, RegisterData data)
    {
        _instance.ExtraBlocks[pluginName] = data;
        _logger.Info($"{pluginName} 已注册 blocks");
    }
    
    /// <inheritdoc />
    public void RegisterWrapper(string id, ActionWrapper wrapper)
    {
        ActionAndRuleRunner.ActionWrappers[id] = wrapper;
        _logger.Info($"已注册 id 为 {id} 的 ActionWrapper");
    }

    /// <inheritdoc />
    public void RegisterWrapper(string id, RuleWrapper wrapper)
    {
        ActionAndRuleRunner.RuleWrappers[id] = wrapper;
        _logger.Info($"已注册 id 为 {id} 的 RuleWrapper");
    }

    public void RegisterDynamicDropdown(string id, DynamicDropdownGetter getter)
    {
        _instance.DynamicDropdowns[id] = getter;
        _logger.Info($"已注册 id 为 {id} 的 DynamicDropdownGetter");
    }

    public void RegisterDataHandler<T>(string id, DataHandler handler)
    {
        SaiDataRegistry.DataGetters[id] = new DataGetterItem
        {
            Type = typeof(T),
            Handler = handler
        };
        _logger.Info($"已注册 id 为 {id} 的 DataGetter");
    }

    public void RegisterActionHandler(string id, ActionHandler handler)
    {
        ActionHandlers[id] = handler;
        _logger.Info($"已注册 id 为 {id} 的 ActionHandler");
    }

    public void RegisterRuleHandler(string id, RuleHandler handler)
    {
        RuleHandlers[id] = handler;
        _logger.Info($"已注册 id 为 {id} 的 RuleHandler");
    }

    /// <inheritdoc />
    public void Shutdown()
    {
        _instance.Shutdown();
    }
}