using SuperAutoIsland.Interface.Shared;

namespace SuperAutoIsland.Interface;

public interface ISaiServer
{
    public void RegisterBlocks(string pluginName, RegisterData data);
}