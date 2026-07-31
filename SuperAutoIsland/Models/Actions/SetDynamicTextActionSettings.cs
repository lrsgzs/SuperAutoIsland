using CommunityToolkit.Mvvm.ComponentModel;

namespace SuperAutoIsland.Models.Actions;

public partial class SetDynamicTextActionSettings : ObservableRecipient
{
    [ObservableProperty] private string _key = string.Empty;
    [ObservableProperty] private string _value = string.Empty;
}