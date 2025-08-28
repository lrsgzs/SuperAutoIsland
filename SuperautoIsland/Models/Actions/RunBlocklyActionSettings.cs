using CommunityToolkit.Mvvm.ComponentModel;

namespace SuperAutoIsland.Models.Actions;

public partial class RunBlocklyActionSettings : ObservableRecipient
{
    [ObservableProperty] private Guid _projectGuid = Guid.Empty;
}