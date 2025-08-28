using SuperAutoIsland.Interface;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Server;

public class SaiServerInstance
{
    private SaiServer? _server;
    private readonly Logger<SaiServerInstance> _logger = new();
    
    public void RegisterBlocks(string pluginName, RegisterData data)
    {
        _server!.ExtraBlocks[pluginName] = data;
    }
    
    public async Task LoadServer(string port)
    {
        _logger.Info("服务器已经启动个毛");
        _server = new SaiServer(port);
        _logger.Info($"服务器地址：{_server.Url}");
        await _server.Serve();
    }

    public void ShutdownServer()
    {
        _server!.Shutdown();
    }
}