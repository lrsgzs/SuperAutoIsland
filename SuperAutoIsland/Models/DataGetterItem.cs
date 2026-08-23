using SuperAutoIsland.Interface.Services;

namespace SuperAutoIsland.Models;

public class DataGetterItem
{
    public required Type Type { get; set; }
    public required DataHandler Handler { get; set; }
}