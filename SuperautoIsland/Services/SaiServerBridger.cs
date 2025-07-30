using SuperAutoIsland.Interface;
using SuperAutoIsland.Interface.Shared;
using SuperAutoIsland.Shared;

namespace SuperAutoIsland.Services;

public class SaiServerBridger : ISaiServer
{
    public static readonly SaiServerInstance Instance = new();

    public SaiServerBridger()
    {
        Instance.LoadServer(GlobalConstants.Configs.MainConfig.Data.ServerPort);
    }
    
    public void RegisterBlocks(string pluginName, RegisterData data)
    {
        Instance.RegisterBlocks(pluginName, data);
    }
}