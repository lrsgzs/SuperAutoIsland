using System.Text.Json;
using SuperAutoIsland.Interface.Shared;

namespace SuperAutoIsland.Interface;

public delegate Task ActionWrapper(JsonElement settings);

public delegate Task<bool> RuleWrapper(JsonElement settings);

public interface ISaiServer
{
    public void RegisterBlocks(string pluginName, RegisterData data);
    
    public void RegisterWrapper(string id, ActionWrapper wrapper);
    
    public void RegisterWrapper(string id, RuleWrapper wrapper);

    public void Shutdown();
}