using CommunityToolkit.Mvvm.ComponentModel;

namespace SuperAutoIsland.Models.Actions;

/// <summary>
///     运行 JavaScript 行动的设置
/// </summary>
public partial class RunJavaScriptActionSettings : ObservableRecipient
{
    /// <summary>
    ///     项目 guid
    /// </summary>
    [ObservableProperty]
    private Guid _projectGuid = Guid.Empty;
}
