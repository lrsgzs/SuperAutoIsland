using ClassIsland.Core;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Extensions.Registry;
using ClassIsland.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using SuperAutoIsland.Automations;
using SuperAutoIsland.ConfigHandlers;
using SuperAutoIsland.Controls.SettingPages;
using SuperAutoIsland.Interface;
using SuperAutoIsland.Services;
using SuperAutoIsland.Shared;

namespace SuperAutoIsland;

[PluginEntrance]
public class Plugin : PluginBase
{
    public override void Initialize(HostBuilderContext context, IServiceCollection services)
    {
        Console.WriteLine("SuperAutoIsland ==> 初期化中...");
        
        Console.WriteLine("SuperAutoIsland ==> 加载配置...");
        GlobalConstants.PluginConfigFolder = PluginConfigFolder;
        GlobalConstants.Configs.MainConfig = new MainConfigHandler();
        
        Console.WriteLine("SuperAutoIsland ==> 注册服务器...");
        services.AddSingleton<ISaiServer, SaiServerBridger>();
        
        Console.WriteLine("SuperAutoIsland ==> 注册自动化元素...");
        Register.Claim(services);
        
        Console.WriteLine("SuperAutoIsland ==> 添加设置页面...");
        services.AddSettingsPage<MainSettingsPage>();
        services.AddSettingsPage<AutomationSettingsPage>();
        services.AddSettingsPage<AboutSettingsPage>();

        AppBase.Current.AppStarted += (_, _) =>
        {
            SaiServerSaver.Save(IAppHost.GetService<ISaiServer>());
        };
    }
}
