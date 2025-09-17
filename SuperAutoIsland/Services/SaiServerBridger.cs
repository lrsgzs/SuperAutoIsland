using SuperAutoIsland.Interface;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Server;
using SuperAutoIsland.Shared;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Services;

/// <summary>
/// 服务器桥接器
/// </summary>
public class SaiServerBridger : ISaiServer
{
    private static readonly SaiServerInstance Instance = new();
    private readonly Logger<SaiServerBridger> _logger = new();

    /// <summary>
    /// 构造函数
    /// <see cref="SaiServerBridger"/>
    /// </summary>
    public SaiServerBridger()
    {
        _ = Instance.LoadServer(GlobalConstants.Configs.MainConfig!.Data.ServerPort);
        _logger.Info("已初始化 SaiServerInstance！");
    }
    
    /// <inheritdoc />
    public void RegisterBlocks(string pluginName, RegisterData data)
    {
        Instance.RegisterBlocks(pluginName, data);
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
    
    /// <inheritdoc />
    public void Shutdown()
    {
        Instance.ShutdownServer();
    }
}