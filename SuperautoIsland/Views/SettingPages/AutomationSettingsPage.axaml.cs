using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Attributes;
using ClassIsland.Shared;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Enums;
using SuperAutoIsland.Services;
using SuperAutoIsland.Services.BlocklyRunner;
using SuperAutoIsland.Shared;
using SuperAutoIsland.ViewModel.SettingPages;

namespace SuperAutoIsland.Views.SettingPages;

public struct ProjectTypeNode
{
    public ProjectsType Type { get; set; }
    public string Name { get; set; }
    public string IconGlyph { get; set; }
}

[HidePageTitle]
[FullWidthPage]
[SettingsPageInfo("sai.automation","SuperAutoIsland 自动化","\uEDC1","\uEDC0")]
public partial class AutomationSettingsPage : SettingsPageBase
{
    private bool IsPanelOpened { get; set; }
    private AutomationViewModel ViewModel { get; } = IAppHost.GetService<AutomationViewModel>();
    private BlocklyRunner _blocklyRunner = IAppHost.GetService<BlocklyRunner>();

    public static readonly ProjectTypeNode[] ProjectTypeNodes = [
        new()
        {
            Type = ProjectsType.BlocklyAction,
            Name = "Blockly 行动",
            IconGlyph = "\uE50A"
        }
    ];

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

    private void CreateBlocklyActionProjectButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.SelectedProject = ProjectsConfigManager.CreateProject(ProjectsType.BlocklyAction, "新项目");
    }

    private void OpenProjectEditorButton_Click(object? sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = $"http://localhost:{GlobalConstants.Configs.MainConfig!.Data.ServerPort}/?id={ViewModel.SelectedProject!.Id}",
            UseShellExecute = true
        });
    }

    private void RunProjectButton_Click(object? sender, RoutedEventArgs e)
    {
        _blocklyRunner.RunProject(ViewModel.SelectedProject!);
    }

    private void DeleteProjectButton_Click(object? sender, RoutedEventArgs e)
    {
        ProjectsConfigManager.DeleteProject(ViewModel.SelectedProject!);
        ViewModel.SelectedProject = null;
    }
}