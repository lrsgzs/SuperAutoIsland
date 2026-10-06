using System.Collections.ObjectModel;
using Avalonia.Interactivity;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Shared;
using DynamicData;
using DynamicData.Binding;
using SuperAutoIsland.Enums;
using SuperAutoIsland.Models;
using SuperAutoIsland.Models.Actions;
using SuperAutoIsland.Models.Settings;
using SuperAutoIsland.Services;
using SuperAutoIsland.Services.BlocklyRunner;
using SuperAutoIsland.Shared;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Controls.ActionSettingsControls;

/// <summary>
///     「运行 JavaScript 行动」行动
/// </summary>
public partial class RunJavaScriptActionSettingsControl : ActionSettingsControlBase<RunJavaScriptActionSettings>
{
    private readonly BlocklyRunner _blocklyRunner = IAppHost.GetService<BlocklyRunner>();
    private readonly ReadOnlyObservableCollection<Project> _filteredProjects;
    private readonly Logger<RunJavaScriptActionSettingsControl> _logger = new();

    /// <summary>
    ///     构造函数，初始化组件并设置过滤后的项目集合
    /// </summary>
    public RunJavaScriptActionSettingsControl()
    {
        InitializeComponent();

        ProjectConfig.Projects
                     .ToObservableChangeSet()
                     .Filter(e => e.Type is ProjectsType.JavaScriptAction)
                     .Bind(out _filteredProjects)
                     .DisposeMany()
                     .Subscribe();
    }

    public ProjectConfigModel ProjectConfig { get; } = GlobalConstants.Configs.ProjectConfig!.Data;
    public ReadOnlyObservableCollection<Project> FilteredProjects => _filteredProjects;

    /// <summary>
    ///     处理运行项目按钮点击事件的方法
    /// </summary>
    private void RunProjectButton_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            var selectedProject = ProjectsConfigManager.GetProject(Settings.ProjectGuid);
            _ = _blocklyRunner.RunJavaScriptProject(selectedProject);
        }
        catch (Exception exception)
        {
            _logger.FormatException(exception);
        }
    }
}
