using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using SuperAutoIsland.Models;
using SuperAutoIsland.Models.Settings;
using SuperAutoIsland.Shared;

namespace SuperAutoIsland.ViewModel.SettingPages;

/// <summary>
/// 「自动化」视图模型
/// </summary>
public partial class AutomationViewModel : ObservableRecipient
{
    [ObservableProperty] private Project? _selectedProject;
    [ObservableProperty] private bool _isPanelOpened;
    
    public ProjectConfigModel ProjectConfig { get; }
    public ObservableCollection<Project> Projects { get; set; }
    
    public AutomationViewModel()
    {
        ProjectConfig = GlobalConstants.Configs.ProjectConfig!.Data;
        Projects = ProjectConfig.Projects;
    }
}