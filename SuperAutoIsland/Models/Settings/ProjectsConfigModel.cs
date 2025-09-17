using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SuperAutoIsland.Models.Settings;

/// <summary>
/// 项目设置模型
/// </summary>
public partial class ProjectConfigModel : ObservableObject
{
    private ObservableCollection<Project> _projects = [];

    /// <summary>
    /// 项目集合
    /// </summary>
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

    /// <summary>
    /// 构造函数
    /// <see cref="ProjectConfigModel"/>
    /// </summary>
    public ProjectConfigModel()
    {
        RegisterProjectsListeners(Projects);
    }

    /// <summary>
    /// 注册项目监听器
    /// </summary>
    /// <param name="value">项目集合类型</param>
    private void RegisterProjectsListeners(ObservableCollection<Project> value)
    {
        value.CollectionChanged += (sender, args) =>
        {
            Console.WriteLine("PCM ==================== SAI CHANGED");
            OnPropertyChanged();
        };
    }
}
