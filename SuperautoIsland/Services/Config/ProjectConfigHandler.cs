using SuperAutoIsland.Abstractions;
using SuperAutoIsland.Models.Settings;
using SuperAutoIsland.Shared;

namespace SuperAutoIsland.Services.Config;

public class ProjectConfigHandler : ConfigHandler<ProjectConfigModel>
{
    public ProjectConfigHandler()
    {
        ConfigPath = Path.Combine(GlobalConstants.PluginConfigFolder!, "Projects.json");
        InitializeConfig();
    }
}