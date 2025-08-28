using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SuperAutoIsland.Models.Settings;

public partial class ProjectConfigModel : ObservableObject
{
    private ObservableCollection<Project> _projects = [];

    public ObservableCollection<Project> Projects
    {
        get => _projects;
        set
        {
            if (value == _projects) return;
            _projects = value;
            OnPropertyChanged();
            RegisterProjectsListeners(value);
        }
    }

    public ProjectConfigModel()
    {
        RegisterProjectsListeners(Projects);
    }

    private void RegisterProjectsListeners(ObservableCollection<Project> value)
    {
        value.CollectionChanged += (sender, args) =>
        {
            Console.WriteLine("PCM ==================== SAI CHANGED");
            OnPropertyChanged();
        };
    }
}
