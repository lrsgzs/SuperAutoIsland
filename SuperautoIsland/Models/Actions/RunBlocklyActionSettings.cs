using CommunityToolkit.Mvvm.ComponentModel;

namespace SuperAutoIsland.Models.Actions;

/// <summary>
/// 运行 Blockly 项目的设置
/// </summary>
public partial class RunBlocklyActionSettings : ObservableRecipient
{
    /// <summary>
    /// 项目 guid
    /// </summary>
    [ObservableProperty] private Guid _projectGuid = Guid.Empty;
}