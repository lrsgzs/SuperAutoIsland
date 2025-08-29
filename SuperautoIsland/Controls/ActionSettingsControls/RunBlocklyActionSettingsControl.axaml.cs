using System.Collections.ObjectModel;
using Avalonia.Interactivity;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Shared;
using DynamicData;
using DynamicData.Binding;
using SuperAutoIsland.Interface.Enums;
using SuperAutoIsland.Models;
using SuperAutoIsland.Models.Actions;
using SuperAutoIsland.Models.Settings;
using SuperAutoIsland.Services;
using SuperAutoIsland.Services.BlocklyRunner;
using SuperAutoIsland.Shared;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Controls.ActionSettingsControls;

/// <summary>
/// 「运行 Blockly 项目」行动
/// </summary>
public partial class RunBlocklyActionSettingsControl : ActionSettingsControlBase<RunBlocklyActionSettings>
{
    public ProjectConfigModel ProjectConfig { get; } = GlobalConstants.Configs.ProjectConfig!.Data;
    private readonly BlocklyRunner _blocklyRunner = IAppHost.GetService<BlocklyRunner>();
    private readonly Logger<RunBlocklyActionSettingsControl> _logger = new();
    private readonly ReadOnlyObservableCollection<Project> _filteredProjects;
    public ReadOnlyObservableCollection<Project> FilteredProjects => _filteredProjects;

    /// <summary>
    /// 构造函数，初始化组件并设置过滤后的项目集合
    /// <see cref="RunBlocklyActionSettingsControl"/>
    /// </summary>
    public RunBlocklyActionSettingsControl()
    {
        InitializeComponent();

        ProjectConfig.Projects
            .ToObservableChangeSet()
            .Filter(e => e.Type is ProjectsType.BlocklyAction)
            .Bind(out _filteredProjects)
            .DisposeMany()
            .Subscribe();
    }
    
    /// <summary>
    /// 处理运行项目按钮点击事件的方法
    /// </summary>
    private void RunProjectButton_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            var selectedProject = ProjectsConfigManager.GetProject(Settings.ProjectGuid);
            _blocklyRunner.RunProject(selectedProject);
        }
        catch (Exception exception)
        {
            _logger.FormatException(exception);
        }
    }
}
