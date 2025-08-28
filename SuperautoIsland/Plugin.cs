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
        
        _logger.Info("检查并补全 ClearScripts 中...");
        ClearScriptPackages.Initialize();
        
        string platform;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            platform = "win";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            platform = "linux";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            platform = "osx";
        }
        else
        {
            _logger.Error("未知平台，退出 SuperAutoIsland.");
            return;
        }

        string arch;
        switch (RuntimeInformation.ProcessArchitecture)
        {
            case Architecture.X86:
                arch = "x86";
                break;
            case Architecture.X64:
                arch = "x64";
                break;
            case Architecture.Arm64:
                arch = "arm64";
                break;
            case Architecture.Arm:
            case Architecture.Wasm:
            case Architecture.S390x:
            case Architecture.LoongArch64:
            case Architecture.Armv6:
            case Architecture.Ppc64le:
            default:
                _logger.Error("未知架构，退出 SuperAutoIsland.");
                return;
        }

        var target = $"{platform}-{arch}";
        var packageInfo = ClearScriptPackages.Infos[target];
        var packagePath = $"{Info.PluginFolderPath}/runtimes/{target}/native/{packageInfo.FileName}";

        if (!File.Exists(packagePath))
        {
            _logger.Info("不存在依赖库！开始下载...");
            
            // 已过时
            // using var web = new WebClient();
            // web.DownloadFile(packageInfo.Url, packagePath);
            
            var httpClient = new HttpClient();
            using var response = httpClient.GetAsync(packageInfo.Url).Result;
            using var fs = File.Create(packagePath);
            response.Content.CopyToAsync(fs).Wait();
            
            _logger.Info("依赖库下载完毕！");
        }
        else
        {
            _logger.Info("存在依赖库！继续执行");
        }

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
            new ActionMenuTreeItem("sai.actions.runBlockly", "运行 Blockly 项目", "\uf44f")
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
