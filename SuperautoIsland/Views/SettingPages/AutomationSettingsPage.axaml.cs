using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Enums;
using SuperAutoIsland.Services;
using SuperAutoIsland.Services.BlocklyRunner;
using SuperAutoIsland.Shared;
using SuperAutoIsland.Shared.Logger;
using SuperAutoIsland.ViewModel.SettingPages;

namespace SuperAutoIsland.Views.SettingPages;

/// <summary>
/// 项目类型节点
/// </summary>
public struct ProjectTypeNode
{
    /// <summary>
    /// 类型
    /// </summary>
    public ProjectsType Type { get; set; }
    
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// 图标
    /// </summary>
    public string IconGlyph { get; set; }
}

/// <summary>
/// 「SuperAutoIsland 自动化」视图
/// </summary>
[HidePageTitle]
[FullWidthPage]
[SettingsPageInfo("sai.automation","SuperAutoIsland 自动化","\uEDC1","\uEDC0")]
public partial class AutomationSettingsPage : SettingsPageBase
{
    private bool IsPanelOpened { get; set; }
    private AutomationViewModel ViewModel { get; } = IAppHost.GetService<AutomationViewModel>();
    private readonly BlocklyRunner _blocklyRunner = IAppHost.GetService<BlocklyRunner>();
    private readonly Logger<AutomationSettingsPage> _logger = new();

    public static readonly ProjectTypeNode[] ProjectTypeNodes = [
        new()
        {
            Type = ProjectsType.BlocklyAction,
            Name = "Blockly 行动",
            IconGlyph = "\uE50A"
        }
    ];

    /// <summary>
    /// 类型-字符串转换器
    /// </summary>
    public static readonly FuncValueConverter<ProjectsType, string> ProjectsTypeNameConverter = new(x => x switch
    {
        ProjectsType.BlocklyAction => "Blockly 行动",
        _ => "未知"
    });
    
    public AutomationSettingsPage()
    {
        InitializeComponent();
    }

    private void ProjectsListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        IsPanelOpened = true;
    }

    /// <summary>
    /// 创建项目点击事件
    /// </summary>
    private void CreateBlocklyActionProjectButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.SelectedProject = ProjectsConfigManager.CreateProject(ProjectsType.BlocklyAction, "新项目");
    }

    /// <summary>
    /// 打开项目编辑器点击事件
    /// </summary>
    private void OpenProjectEditorButton_Click(object? sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = $"http://localhost:{GlobalConstants.Configs.MainConfig!.Data.ServerPort}/?id={ViewModel.SelectedProject!.Id}",
            UseShellExecute = true
        });
    }

    /// <summary>
    /// 运行项目点击事件
    /// </summary>
    private void RunProjectButton_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            _blocklyRunner.RunProject(ViewModel.SelectedProject!);
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
    }
}