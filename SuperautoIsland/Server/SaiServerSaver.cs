using SuperAutoIsland.Interface.Services;

namespace SuperAutoIsland.Server;

/// <summary>
/// 服务器保存器
/// </summary>
public static class SaiServerSaver
{
    private static ISaiServer? _saiServer;

    /// <summary>
    /// 保存服务器实例
    /// </summary>
    /// <param name="saiServer">服务器实例</param>
    public static void Save(ISaiServer saiServer)
    {
        if (_saiServer != null) return;
        _saiServer = saiServer;
    }
}