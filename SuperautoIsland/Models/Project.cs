using CommunityToolkit.Mvvm.ComponentModel;
using SuperAutoIsland.Interface.Enums;

namespace SuperAutoIsland.Models;

public partial class Project : ObservableRecipient
{
    [ObservableProperty] private Guid _id = Guid.NewGuid();
    [ObservableProperty] private string _name = "新项目";
    [ObservableProperty] private ProjectsType _type = ProjectsType.BlocklyAction;
}