using System.Reactive.Linq;
using ClassIsland.Core;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Extensions.Registry;
using ClassIsland.Core.Models.Automation;
using ClassIsland.Shared;
using DynamicData;
using DynamicData.Binding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SuperAutoIsland.Controls.ActionSettingsControls;
using SuperAutoIsland.Controls.RuleSettingsControls;
using SuperAutoIsland.Enums;
using SuperAutoIsland.Interface;
using SuperAutoIsland.Interface.MetaData;
using SuperAutoIsland.Interface.MetaData.ArgsType;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Models.Rules;
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

/// <summary>
/// 插件本体
/// </summary>
[PluginEntrance]
public class Plugin : PluginBase
{
    private readonly Logger<Plugin> _logger = new();
    
    /// <summary>
    /// 初始化插件
    /// </summary>
    /// <param name="context">上下文</param>
    /// <param name="services">服务</param>
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
        services.AddSingleton<CiRunner>();
        
        _logger.Info("注册服务器...");
        services.AddSingleton<ISaiServer, SaiServerBridger>();
        
        _logger.Info("注册视图模型...");
        services.AddTransient<SaiLogsViewModel>();
        services.AddTransient<AutomationViewModel>();
        
        _logger.Info("注册自动化元素...");
        // 行动
        services.AddAction<RunBlocklyAction, RunBlocklyActionSettingsControl>();
        services.AddAction<RunActionSet, RunActionSetSettingsControl>();
        
        // 行动树
        IActionService.ActionMenuTree.Add(new ActionMenuTreeGroup("SAI 自动化", "\uF3AF"));
        IActionService.ActionMenuTree["SAI 自动化"].AddRange([
            new ActionMenuTreeItem("sai.actions.runBlockly", "运行 Blockly 项目", "\uE049"),
            new ActionMenuTreeItem("sai.actions.runActionSet", "运行可复用的行动组", "\uE01F")
        ]);

        // 规则
        services.AddRule<RunCiRulesetSettings, RunCiRulesetSettingsControl>("sai.rules.runCiRuleset", "运行可复用的规则集", "\uF17E");
        
        // 规则处理器
        services.AddSingleton<RuleHandlerService>();
        
        _logger.Info("添加设置页面...");
        services.AddSingleton<SaiLogsWindow>();
        services.AddSettingsPage<MainSettingsPage>();
        services.AddSettingsPage<AutomationSettingsPage>();
        // services.AddSettingsPage<AboutSettingsPage>();

        // 应用启动完毕
        AppBase.Current.AppStarted += (_, _) =>
        {
            _logger.BaseLog("TRACE", "保存 SaiServer 实例...");
            var saiServerService = IAppHost.GetService<ISaiServer>();
            SaiServerSaver.Save(saiServerService);
            
            _logger.Debug("初始化服务...");
            IAppHost.GetService<RuleHandlerService>();
            IAppHost.GetService<BlocklyRunner>();
            
            _logger.Debug("注册 SuperAutoIsland 元素...");
            saiServerService.RegisterBlocks("SuperAutoIsland", new RegisterData
            {
                Actions = [
                    new BlockMetadata
                    {
                        Id = "sai.actions.runBlockly",
                        Name = "运行 Blockly 项目",
                        Icon = ("Blockly 项目", "\uE049"),
                        Args = new Dictionary<string, MetaArgsBase>
                        {
                            ["ProjectGuid"] = new DropDownMetaArgs
                            {
                                Name = "",
                                Type = MetaType.dropdown,
                                Options = GlobalConstants.Configs.ProjectConfig.Data.Projects
                                    .Where(e => e.Type is ProjectsType.BlocklyAction)
                                    .Select(e => (e.Name, e.Id.ToString()))
                                    .ToList()
                            }
                        },
                        DropdownUseNumbers = false,
                        InlineField = false,
                        InlineBlock = false,
                        IsRule = false
                    },
                    new BlockMetadata
                    {
                        Id = "sai.actions.runActionSet",
                        Name = "运行可复用的行动组",
                        Icon = ("行动组", "\uE01F"),
                        Args = new Dictionary<string, MetaArgsBase>
                        {
                            ["ProjectGuid"] = new DropDownMetaArgs
                            {
                                Name = "",
                                Type = MetaType.dropdown,
                                Options = GlobalConstants.Configs.ProjectConfig.Data.Projects
                                    .Where(e => e.Type is ProjectsType.CiActionSet)
                                    .Select(e => (e.Name, e.Id.ToString()))
                                    .ToList()
                            }
                        },
                        DropdownUseNumbers = false,
                        InlineField = false,
                        InlineBlock = false,
                        IsRule = false
                    }
                ],
                Rules = [
                    new BlockMetadata
                    {
                        Id = "sai.rules.runCiRuleset",
                        Name = "运行可复用的规则集",
                        Icon = ("规则集", "\uF17E"),
                        Args = new Dictionary<string, MetaArgsBase>
                        {
                            ["ProjectGuid"] = new DropDownMetaArgs
                            {
                                Name = "",
                                Type = MetaType.dropdown,
                                Options = GlobalConstants.Configs.ProjectConfig.Data.Projects
                                    .Where(e => e.Type is ProjectsType.CiRuleset)
                                    .Select(e => (e.Name, e.Id.ToString()))
                                    .ToList()
                            }
                        },
                        DropdownUseNumbers = false,
                        InlineField = false,
                        InlineBlock = false,
                        IsRule = true
                    }
                ]
            });
        
            saiServerService.RegisterWrapper("classisland.os.run.program", RunActionProgram.Wrapper);
            
            // _logger.Debug("尝试运行 JS");
            // var blocklyRunner = IAppHost.GetService<BlocklyRunner>();
            // blocklyRunner.RunJavaScript("logger.Info('Hello from Javascript')");
        };
        
        // 应用退出
        AppBase.Current.AppStopping += (_,_) =>
        {
            _logger.Info("兜底：保存全部配置...");
            GlobalConstants.Configs.MainConfig.Save();
            GlobalConstants.Configs.ProjectConfig.Save();
            
            var blocklyRunner = IAppHost.GetService<BlocklyRunner>();
            blocklyRunner.Dispose();
            
            var server = IAppHost.GetService<ISaiServer>();
            server.Shutdown();
            _logger.Info("已尝试关闭，3 秒后将会强行关闭 SuperAutoIsland.Server...");
            
            new Thread(() => {
                Thread.Sleep(3000);
                _logger.Info("正在关闭...");
                Environment.Exit(0);
            }).Start();
        };
    }
}
