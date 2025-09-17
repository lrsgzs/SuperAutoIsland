using SuperAutoIsland.Interface;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Server;

/// <summary>
/// 服务器包装器 1 号
/// </summary>
public class SaiServerInstance
{
    private SaiServer? _server;
    private readonly Logger<SaiServerInstance> _logger = new();
    
    /// <summary>
    /// 注册积木
    /// </summary>
    /// <param name="pluginName">插件名称</param>
    /// <param name="data">积木数据</param>
    public void RegisterBlocks(string pluginName, RegisterData data)
    {
        _server!.ExtraBlocks[pluginName] = data;
    }
    
    /// <summary>
    /// 加载服务器
    /// </summary>
    /// <param name="port">服务器端口</param>
    public async Task LoadServer(string port)
    {
        _logger.Info("服务器已经启动个毛");
        _server = new SaiServer(port);
        _logger.Info($"服务器地址：{_server.Url}");
        await _server.Serve();
    }

    /// <summary>
    /// 结束服务器
    /// </summary>
    public void ShutdownServer()
    {
        _server!.Shutdown();
    }
}