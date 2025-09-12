using System.Collections.ObjectModel;
using ClassIsland.Core.Abstractions.Controls;
using DynamicData;
using DynamicData.Binding;
using SuperAutoIsland.Enums;
using SuperAutoIsland.Models;
using SuperAutoIsland.Models.Rules;
using SuperAutoIsland.Models.Settings;
using SuperAutoIsland.Shared;

namespace SuperAutoIsland.Controls.RuleSettingsControls;

/// <summary>
/// 「运行可复用的规则集」规则
/// </summary>
public partial class RunCiRulesetSettingsControl : RuleSettingsControlBase<RunCiRulesetSettings>
{
    public ProjectConfigModel ProjectConfig { get; } = GlobalConstants.Configs.ProjectConfig!.Data;
    private readonly ReadOnlyObservableCollection<Project> _filteredProjects;
    public ReadOnlyObservableCollection<Project> FilteredProjects => _filteredProjects;
    
    /// <summary>
    /// 构造函数，初始化组件并设置过滤后的项目集合
    /// <see cref="RunCiRulesetSettingsControl"/>
    /// </summary>
    public RunCiRulesetSettingsControl()
    {
        InitializeComponent();

        ProjectConfig.Projects
            .ToObservableChangeSet()
            .Filter(e => e.Type is ProjectsType.CiRuleset)
            .Bind(out _filteredProjects)
            .DisposeMany()
            .Subscribe();
    }
}