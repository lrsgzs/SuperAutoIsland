using ClassIsland.Core;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Extensions.Registry;
using ClassIsland.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SuperAutoIsland.ConfigHandlers;
using SuperAutoIsland.Controls.SettingPages;
using SuperAutoIsland.Interface;
using SuperAutoIsland.Server;
using SuperAutoIsland.Services;
using SuperAutoIsland.Shared;

namespace SuperAutoIsland;

[PluginEntrance]
public class Plugin : PluginBase
{
    public override void Initialize(HostBuilderContext context, IServiceCollection services)
    {
        // ascii 字符画后续再补
        
        Console.WriteLine("SuperAutoIsland ==> 初期化中...");
        
        Console.WriteLine("SuperAutoIsland ==> 加载配置...");
        GlobalConstants.PluginFolder = Info.PluginFolderPath;
        GlobalConstants.PluginConfigFolder = PluginConfigFolder;
        GlobalConstants.Configs.MainConfig = new MainConfigHandler();
        
        Console.WriteLine("SuperAutoIsland ==> 注册服务器...");
        services.AddSingleton<ISaiServer, SaiServerBridger>();
        
        Console.WriteLine("SuperAutoIsland ==> 注册不存在的自动化元素...");
        // 等待。
        
        Console.WriteLine("SuperAutoIsland ==> 添加设置页面...");
        services.AddSettingsPage<MainSettingsPage>();
        services.AddSettingsPage<AutomationSettingsPage>();
        services.AddSettingsPage<AboutSettingsPage>();

        AppBase.Current.AppStarted += (_, _) =>
        {
            SaiServerSaver.Save(IAppHost.GetService<ISaiServer>());
        };
        
        AppBase.Current.AppStopping += (_,_) =>
        {
            var server = IAppHost.GetService<ISaiServer>();
            server.Shutdown();
            
            Console.WriteLine("已尝试关闭，5 秒后将会强行关闭 SuperAutoIsland.Server ...");
            new Thread(() => {
                Thread.Sleep(5000);
                Console.WriteLine("正在关闭...");
                Environment.Exit(0);
            }).Start();
        };
    }
}
