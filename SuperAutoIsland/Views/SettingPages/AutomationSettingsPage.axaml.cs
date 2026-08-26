using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using CommunityToolkit.Mvvm.Input;
using SuperAutoIsland.Enums;
using SuperAutoIsland.Services;
using SuperAutoIsland.Services.BlocklyRunner;
using SuperAutoIsland.Shared;
using SuperAutoIsland.Shared.Logger;
using SuperAutoIsland.ViewModel.SettingPages;

namespace SuperAutoIsland.Views.SettingPages;

/// <summary>
/// 项目类型节点
/// </summary>
public class ProjectTypeNode
{
    /// <summary>
    /// 类型
    /// </summary>
    public ProjectsType Type { get; set; } = ProjectsType.BlocklyAction;

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 图标
    /// </summary>
    public string IconGlyph { get; set; } = string.Empty;
    
    /// <summary>
    /// 工具提示
    /// </summary>
    public string ToolTip { get; set; } = string.Empty;
}

/// <summary>
/// 「SuperAutoIsland 自动化」视图
/// </summary>
[HidePageTitle]
[FullWidthPage]
[Group("sai.settings")]
[SettingsPageInfo("sai.settings.automation","自动化",FluentIcons.PlayCircleSparkleRegular,FluentIcons.PlayCircleSparkleFilled)]
public partial class AutomationSettingsPage : SettingsPageBase
{
    public AutomationViewModel ViewModel { get; } = IAppHost.GetService<AutomationViewModel>();
    private readonly Logger<AutomationSettingsPage> _logger = new();
    
    private readonly BlocklyRunner _blocklyRunner = IAppHost.GetService<BlocklyRunner>();
    private readonly CiRunner _ciRunner = IAppHost.GetService<CiRunner>();

    public ProjectTypeNode[] ProjectTypeNodes { get; } = [
        new()
        {
            Type = ProjectsType.BlocklyAction,
            Name = "Blockly 行动",
            IconGlyph = FluentIcons.AlignSpaceEvenlyVerticalRegular,
            ToolTip = "更自由的自动化行动",
        },
        new()
        {
            Type = ProjectsType.CiRuleset,
            Name = "可复用的规则集",
            IconGlyph = FluentIcons.TagMultipleRegular,
            ToolTip = "快速复用同套规则集",
        },
        new()
        {
            Type = ProjectsType.CiActionSet,
            Name = "可复用的行动组",
            IconGlyph = FluentIcons.AirplaneTakeOffRegular,
            ToolTip = "快速复用同套行动组",
        }
    ];

    /// <summary>
    /// 类型-字符串转换器
    /// </summary>
    public static readonly FuncValueConverter<ProjectsType, string> ProjectsTypeNameConverter = new(x => x switch
    {
        ProjectsType.BlocklyAction => "Blockly 行动",
        ProjectsType.CiRuleset => "可复用的规则集",
        ProjectsType.CiActionSet => "可复用的行动组",
        _ => "未知"
    });
    
    public AutomationSettingsPage()
    {
        if (GlobalConstants.Configs.MainConfig!.Data.EnableEasterEggs)
        {
            ProjectTypeNodes[0].ToolTip = "析构万理的 Blockly 先生";
        }
        
        DataContext = this;
        InitializeComponent();
    }

    private void ProjectsListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        ViewModel.IsPanelOpened = true;
    }
    
    /// <summary>
    /// 创建项目命令
    /// </summary>
    [RelayCommand]
    private void CreateProject(ProjectsType type)
    {
        switch (type)
        {
            case ProjectsType.BlocklyAction:
                ViewModel.SelectedProject = ProjectsConfigManager.CreateProject(ProjectsType.BlocklyAction, "新 Blockly 行动");
                break;
            case ProjectsType.CiRuleset:
                ViewModel.SelectedProject = ProjectsConfigManager.CreateProject(ProjectsType.CiRuleset, "新可复用的规则集");
                break;
            case ProjectsType.CiActionSet:
                ViewModel.SelectedProject = ProjectsConfigManager.CreateProject(ProjectsType.CiActionSet, "新可复用的行动组");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    /// <summary>
    /// 打开项目编辑器点击事件
    /// </summary>
    private void OpenProjectEditorButton_Click(object? sender, RoutedEventArgs e)
    {
        var uri = new Uri($"http://localhost:{GlobalConstants.Configs.MainConfig!.Data.ServerPort}/" +
                          $"?id={ViewModel.SelectedProject!.Id}");
        IAppHost.TryGetService<IUriNavigationService>()?.NavigateWrapped(uri);
    }

    /// <summary>
    /// 运行项目点击事件
    /// </summary>
    private async void RunProjectButton_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            switch (ViewModel.SelectedProject!.Type)
            {
                case ProjectsType.BlocklyAction:
                    await _blocklyRunner.RunActionProject(ViewModel.SelectedProject!);
                    break;
                case ProjectsType.CiRuleset:
                    _ciRunner.RunRulesetProject(ViewModel.SelectedProject);
                    break;
                case ProjectsType.CiActionSet:
                    await _ciRunner.RunActionSetProject(ViewModel.SelectedProject);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        catch (Exception exception)
        {
            _logger.FormatException(exception);
        }
    }

    /// <summary>
    /// 删除项目点击事件
    /// </summary>
    private void DeleteProjectButton_Click(object? sender, RoutedEventArgs e)
    {
        ProjectsConfigManager.DeleteProject(ViewModel.SelectedProject!);
        ViewModel.SelectedProject = null;
        ViewModel.IsPanelOpened = false;
    }
}