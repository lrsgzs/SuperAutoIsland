using ClassIsland.Core.Models.Ruleset;
using ClassIsland.Shared.Models.Automation;

namespace SuperAutoIsland.Interface.Services;

public delegate ActionItem ActionWrapper(ActionItem action);

public delegate Rule RuleWrapper(Rule rule);

public interface ISaiServer
{
    public void RegisterBlocks(string pluginName, RegisterData data);
    
    public void RegisterWrapper(string id, ActionWrapper wrapper);
    
    public void RegisterWrapper(string id, RuleWrapper wrapper);

    public void Shutdown();
}