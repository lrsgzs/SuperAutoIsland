using SuperAutoIsland.Interface;
using SuperAutoIsland.Interface.Shared;
using SuperAutoIsland.Server;
using SuperAutoIsland.Shared;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Services;

public class SaiServerBridger : ISaiServer
{
    public static readonly SaiServerInstance Instance = new();
    private readonly Logger _logger = new("SaiServerBridger");

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
        Instance.RegisterWrapper(id, wrapper);
        _logger.Info($"已注册 id 为 {id} 的 ActionWrapper");
    }

    public void RegisterWrapper(string id, RuleWrapper wrapper)
    {
        Instance.RegisterWrapper(id, wrapper);
        _logger.Info($"已注册 id 为 {id} 的 RuleWrapper");
    }
    
    public void Shutdown()
    {
        Instance.ShutdownServer();
    }
}