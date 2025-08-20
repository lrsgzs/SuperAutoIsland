using ClassIsland.Core;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Extensions.Registry;
using ClassIsland.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SuperAutoIsland.ConfigHandlers;
using SuperAutoIsland.Controls.SettingPages;
using SuperAutoIsland.Controls.Windows;
using SuperAutoIsland.Interface;
using SuperAutoIsland.Server;
using SuperAutoIsland.Services;
using SuperAutoIsland.Shared;
using SuperAutoIsland.Shared.Logger;
using SuperAutoIsland.ViewModel;

namespace SuperAutoIsland;

[PluginEntrance]
public class Plugin : PluginBase
{
    private readonly Logger _logger = new("Plugin");
    
    public override void Initialize(HostBuilderContext context, IServiceCollection services)
    {
        // ascii 字符画后续再补
        
        _logger.Info("SuperAutoIsland ==> 初期化中...");
        
        _logger.Info("注册日志筛选器...");
        services.AddTransient<SaiLogsViewModel>();
        
        _logger.Info("加载配置...");
        GlobalConstants.PluginFolder = Info.PluginFolderPath;
        GlobalConstants.PluginConfigFolder = PluginConfigFolder;
        GlobalConstants.Configs.MainConfig = new MainConfigHandler();
        
        _logger.Info("注册服务器...");
        services.AddSingleton<ISaiServer, SaiServerBridger>();
        
        _logger.Info("注册不存在的自动化元素...");
        // 等待。
        
        _logger.Info("添加设置页面...");
        services.AddSingleton<SaiLogsWindow>();
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
            _logger.Info("已尝试关闭，5 秒后将会强行关闭 SuperAutoIsland.Server ...");
            
            new Thread(() => {
                Thread.Sleep(5000);
                _logger.Info("正在关闭...");
                Environment.Exit(0);
            }).Start();
        };
    }
}
