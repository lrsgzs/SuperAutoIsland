using SuperAutoIsland.Interface.Services;

namespace SuperAutoIsland.Models;

public class DataGetterItem
{
    public required Type Type { get; set; }
    public required DataGetter Getter { get; set; }
}