using SuperAutoIsland.Interface.Shared;

namespace SuperAutoIsland.Server;

public class SaiServerInstance
{
    private SaiServer? _server;
    
    public void RegisterBlocks(string pluginName, RegisterData data)
    {
        _server!.ExtraBlocks[pluginName] = data;
    }

    public async Task LoadServer(string port)
    {
        Console.WriteLine("服务器已经启动个毛");
        _server = new SaiServer(port);
        Console.WriteLine($"服务器地址：{_server.Url}");
        await _server.Serve();
    }

    public void ShutdownServer()
    {
        _server!.Shutdown();
    }
}