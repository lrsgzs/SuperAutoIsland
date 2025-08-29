using Avalonia.Interactivity;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Shared;
using SuperAutoIsland.Models.Actions;
using SuperAutoIsland.Models.Settings;
using SuperAutoIsland.Services;
using SuperAutoIsland.Services.BlocklyRunner;
using SuperAutoIsland.Shared;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Controls.ActionSettingsControls;

public partial class RunBlocklyActionSettingsControl : ActionSettingsControlBase<RunBlocklyActionSettings>
{
    public ProjectConfigModel ProjectConfig { get; } = GlobalConstants.Configs.ProjectConfig!.Data;
    private readonly BlocklyRunner _blocklyRunner = IAppHost.GetService<BlocklyRunner>();
    private readonly Logger<RunBlocklyActionSettingsControl> _logger = new();
    
    private readonly ReadOnlyObservableCollection<Project> _filteredProjects;
    public ReadOnlyObservableCollection<Project> FilteredProjects => _filteredProjects;

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