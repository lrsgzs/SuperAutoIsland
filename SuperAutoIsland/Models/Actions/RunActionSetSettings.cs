using CommunityToolkit.Mvvm.ComponentModel;

namespace SuperAutoIsland.Models.Actions;

/// <summary>
/// 运行可复用的行动组的设置
/// </summary>
public partial class RunActionSetSettings : ObservableRecipient
{
    /// <summary>
    /// 项目 guid
    /// </summary>
    [ObservableProperty] private Guid _projectGuid = Guid.Empty;
}