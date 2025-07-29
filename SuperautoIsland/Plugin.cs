using ClassIsland.Core;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Extensions.Registry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using SuperAutoIsland.Automations;
using SuperAutoIsland.Controls.SettingPages;

namespace SuperAutoIsland;

[PluginEntrance]
public class Plugin : PluginBase
{
    public override void Initialize(HostBuilderContext context, IServiceCollection services)
    {
        Console.WriteLine("SuperAutoIsland is loading...");

        Register.Claim(services);
        services.AddSettingsPage<MainSettingsPage>();
    }
}
