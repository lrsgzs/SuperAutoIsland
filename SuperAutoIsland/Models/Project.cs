using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using ClassIsland.Core.Enums;
using ClassIsland.Core.Models.Ruleset;
using ClassIsland.Shared.Models.Automation;
using CommunityToolkit.Mvvm.ComponentModel;
using SuperAutoIsland.Enums;

namespace SuperAutoIsland.Models;

/// <summary>
/// 项目模型
/// </summary>
public partial class Project : ObservableRecipient
{
    /// <summary>
    /// 项目 Guid
    /// </summary>
    [ObservableProperty] private Guid _id = Guid.NewGuid();
    
    /// <summary>
    /// 项目名称
    /// </summary>
    [ObservableProperty] private string _name = "新项目";
    
    /// <summary>
    /// 项目类型
    /// </summary>
    [ObservableProperty] private ProjectsType _type = ProjectsType.BlocklyAction;

    /// <summary>
    /// 规则集（仅在 ProjectsType.CiRuleset 下可用）。
    /// </summary>
    [ObservableProperty] private Ruleset _ruleset = new()
    {
        Mode = RulesetLogicalMode.Or
    };
    private bool? _rulesetState = null;
    
    /// <summary>
    /// 规则集状态（仅在 ProjectsType.CiRuleset 下可用）。
    /// </summary>
    [JsonIgnore]
    public bool? RulesetState
    {
        get => _rulesetState;
        set
        {
            OnPropertyChanging();
            _rulesetState = value;
            OnPropertyChanged();
        }
    }
    
    /// <summary>
    /// 行动组（仅在 ProjectsType.CiActionSet 下可用）。
    /// </summary>
    [ObservableProperty] private ObservableCollection<ActionItem> _actions = [];
}