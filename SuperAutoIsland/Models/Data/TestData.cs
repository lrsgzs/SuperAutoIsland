using CommunityToolkit.Mvvm.ComponentModel;

namespace SuperAutoIsland.Models.Data;

public partial class TestData : ObservableRecipient
{
    [ObservableProperty] private string _text = string.Empty;
}