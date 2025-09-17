using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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
    /// <summary>
    /// 当前选中的项目
    /// </summary>
    [ObservableProperty] private Project? _selectedProject;
    
    public ProjectConfigModel ProjectConfig { get; } = GlobalConstants.Configs.ProjectConfig!.Data;
    public ObservableCollection<Project> Projects { get; set; }
    
    public AutomationViewModel()
    {
        Projects = ProjectConfig.Projects;

        Projects.CollectionChanged += OnProjectsChanged;
    }
    
    private void OnProjectsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(new PropertyChangedEventArgs("Projects"));
    }
}