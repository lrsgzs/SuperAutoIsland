using SuperAutoIsland.Interface;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Server;
using SuperAutoIsland.Shared;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Services;

public class SaiServerBridger : ISaiServer
{
    private static readonly SaiServerInstance Instance = new();
    private readonly Logger<SaiServerBridger> _logger = new();

    public SaiServerBridger()
    {
        _ = Instance.LoadServer(GlobalConstants.Configs.MainConfig!.Data.ServerPort);
        _logger.Info("已初始化 SaiServerInstance！");
    }
    
    public void RegisterBlocks(string pluginName, RegisterData data)
    {
        Instance.RegisterBlocks(pluginName, data);
        _logger.Info($"{pluginName} 已注册 blocks");
    }

    public void RegisterWrapper(string id, ActionWrapper wrapper)
    {
        ActionAndRuleRunner.ActionWrappers[id] = wrapper;
        _logger.Info($"已注册 id 为 {id} 的 ActionWrapper");
    }

    public void RegisterWrapper(string id, RuleWrapper wrapper)
    {
        ActionAndRuleRunner.RuleWrappers[id] = wrapper;
        _logger.Info($"已注册 id 为 {id} 的 RuleWrapper");
    }
    
    public void Shutdown()
    {
        Instance.ShutdownServer();
    }
}