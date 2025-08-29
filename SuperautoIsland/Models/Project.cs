using CommunityToolkit.Mvvm.ComponentModel;
using SuperAutoIsland.Interface.Enums;

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
}