using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;

namespace SuperAutoIsland.Services;

public class SaiBlocksRegistry
{
    public static readonly OrderedDictionary<string, List<BlockMetadata>> Categories = new();
    public static readonly Dictionary<string, BlockBase> Blocks = new();
    public static readonly Dictionary<string, DynamicDropdownHandler> DynamicDropdowns = new();
}