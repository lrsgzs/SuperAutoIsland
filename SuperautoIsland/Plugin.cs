using System.Net;
using System.Runtime.InteropServices;
using ClassIsland.Core;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Extensions.Registry;
using ClassIsland.Core.Models.Automation;
using ClassIsland.Shared;
using DynamicData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SuperAutoIsland.Controls.ActionSettingsControls;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Server;
using SuperAutoIsland.Services;
using SuperAutoIsland.Services.Automations.Actions;
using SuperAutoIsland.Services.Automations.Wrappers;
using SuperAutoIsland.Services.BlocklyRunner;
using SuperAutoIsland.Services.Config;
using SuperAutoIsland.Shared;
using SuperAutoIsland.Shared.Logger;
using SuperAutoIsland.ViewModel;
using SuperAutoIsland.ViewModel.SettingPages;
using SuperAutoIsland.Views;
using SuperAutoIsland.Views.SettingPages;

namespace SuperAutoIsland;

[PluginEntrance]
public class Plugin : PluginBase
{
    private readonly Logger<Plugin> _logger = new();
    
    public override void Initialize(HostBuilderContext context, IServiceCollection services)
    {
        // ascii 字符画后续再补

        _logger.Info("欢迎使用 SuperAutoIsland");
        _logger.Info("初期化中...");
        
        _logger.Info("加载配置...");
        GlobalConstants.PluginFolder = Info.PluginFolderPath;
        GlobalConstants.PluginConfigFolder = PluginConfigFolder;
        GlobalConstants.Configs.MainConfig = new MainConfigHandler();
        GlobalConstants.Configs.ProjectConfig = new ProjectConfigHandler();
        ProjectsConfigManager.Initialization();
        
        _logger.Info("注册运行器...");
        services.AddSingleton<ActionAndRuleRunner>();
        services.AddSingleton<BlocklyRunner>();
        
        _logger.Info("注册服务器...");
        services.AddSingleton<ISaiServer, SaiServerBridger>();
        
        _logger.Info("注册视图模型...");
        services.AddTransient<SaiLogsViewModel>();
        services.AddTransient<AutomationViewModel>();
        
        _logger.Info("注册自动化元素...");
        services.AddAction<RunBlocklyAction, RunBlocklyActionSettingsControl>();
        IActionService.ActionMenuTree.Add(new ActionMenuTreeGroup("SAI 自动化", "\uF3AF"));
        IActionService.ActionMenuTree["SAI 自动化"].AddRange([
            new ActionMenuTreeItem("sai.actions.runBlockly", "运行 Blockly 项目", "\uE049")
        ]);
        
        _logger.Info("添加设置页面...");
        services.AddSingleton<SaiLogsWindow>();
        services.AddSettingsPage<MainSettingsPage>();
        services.AddSettingsPage<AutomationSettingsPage>();
        // services.AddSettingsPage<AboutSettingsPage>();

        AppBase.Current.AppStarted += (_, _) =>
        {
            var saiServerService = IAppHost.GetService<ISaiServer>();
            SaiServerSaver.Save(saiServerService);
            
            _logger.Debug("注册 SuperAutoIsland 元素...");
            saiServerService.RegisterWrapper("classisland.os.run.program", RunActionProgram.Wrapper);
            
            _logger.Debug("尝试运行 JS");
            var blocklyRunner = IAppHost.GetService<BlocklyRunner>();
            blocklyRunner.RunJavaScript("logger.Info('Hello from Javascript')");
        };
        
        AppBase.Current.AppStopping += (_,_) =>
        {
            var blocklyRunner = IAppHost.GetService<BlocklyRunner>();
            blocklyRunner.Dispose();
            
            var server = IAppHost.GetService<ISaiServer>();
            server.Shutdown();
            _logger.Info("已尝试关闭，3 秒后将会强行关闭 SuperAutoIsland.Server ...");
            
            new Thread(() => {
                Thread.Sleep(3000);
                _logger.Info("正在关闭...");
                Environment.Exit(0);
            }).Start();
        };
    }
}
