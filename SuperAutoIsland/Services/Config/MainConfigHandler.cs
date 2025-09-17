using SuperAutoIsland.Abstractions;
using SuperAutoIsland.Models.Settings;
using SuperAutoIsland.Shared;

namespace SuperAutoIsland.Services.Config;

public class MainConfigHandler : ConfigHandler<MainConfigModel>
{
    public MainConfigHandler()
    {
        ConfigPath = Path.Combine(GlobalConstants.PluginConfigFolder!, "Main.json");
        InitializeConfig();
    }
}
