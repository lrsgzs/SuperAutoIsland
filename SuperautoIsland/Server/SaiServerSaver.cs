using SuperAutoIsland.Interface;

namespace SuperAutoIsland.Server;

public static class SaiServerSaver
{
    private static ISaiServer? _saiServer;

    public static void Save(ISaiServer saiServer)
    {
        if (_saiServer != null) return;
        _saiServer = saiServer;
    }
}