using ClassIsland.Core;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Extensions.Registry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using SuperAutoIsland.Automations;
using SuperAutoIsland.ConfigHandlers;
using SuperAutoIsland.Controls.SettingPages;
using SuperAutoIsland.Shared;

namespace SuperAutoIsland;

[PluginEntrance]
public class Plugin : PluginBase
{
    public override void Initialize(HostBuilderContext context, IServiceCollection services)
    {
        Console.WriteLine("SuperAutoIsland is loading...");
        
        GlobalConstants.PluginConfigFolder = PluginConfigFolder;
        GlobalConstants.Configs.MainConfig = new MainConfigHandler();

        Register.Claim(services);
        services.AddSettingsPage<MainSettingsPage>();
        services.AddSettingsPage<AutomationSettingsPage>();
        services.AddSettingsPage<AboutSettingsPage>();
    }
}
