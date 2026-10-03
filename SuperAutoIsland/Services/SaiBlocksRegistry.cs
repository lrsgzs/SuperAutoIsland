using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Services;

/// <summary>
///     SAI 积木注册表
/// </summary>
public class SaiBlocksRegistry
{
    private static readonly Logger<SaiBlocksRegistry> Logger = new();

    /// <summary>
    ///     分类提供方列表（分类顺序与注册顺序一致）
    /// </summary>
    public static List<ICategoryProvider> CategoryProviders { get; } = [];

    /// <summary>
    ///     分类名称 -> 分类下的积木元数据（由 <see cref="CategoryProviders" /> 构建，重建时整体替换）
    /// </summary>
    public static OrderedDictionary<string, List<BlockMetadata>> Categories { get; private set; } = new();

    /// <summary>
    ///     积木 id -> 积木实例（由 <see cref="CategoryProviders" /> 构建，重建时整体替换）
    /// </summary>
    public static Dictionary<string, BlockBase> Blocks { get; private set; } = new();

    /// <summary>
    ///     前缀 -> 前缀处理器
    /// </summary>
    public static Dictionary<string, PrefixHandler> PrefixHandlers { get; } = new();

    /// <summary>
    ///     动态下拉框 id -> getter
    /// </summary>
    public static Dictionary<string, DynamicDropdownHandler> DynamicDropdowns { get; } = new();

    /// <summary>
    ///     重新构建所有分类的积木和 Block 列表
    /// </summary>
    public static void Rebuild()
    {
        var categories = new OrderedDictionary<string, List<BlockMetadata>>();
        var blocks = new Dictionary<string, BlockBase>();

        foreach (var provider in CategoryProviders)
        {
            var name = provider.Metadata.Name;
            var register = new BlocksRegister(name);

            try
            {
                provider.Build(register);
            }
            catch (Exception e)
            {
                Logger.FormatException(e);
                Logger.Warn($"分类 {name} 构建失败，已跳过");
                continue;
            }

            categories[name] = register.Items;
            foreach (var (id, block) in register.Blocks)
            {
                blocks[id] = block;
            }
        }

        Categories = categories;
        Blocks = blocks;
        Logger.Info($"已更新所有分类：{string.Join("、", Categories.Keys)}");
    }

    /// <summary>
    ///     查找匹配积木 id 的前缀处理器（匹配到多个前缀时使用最长的一个）
    /// </summary>
    /// <param name="id">积木 id</param>
    /// <returns>前缀处理器，未找到时为 null</returns>
    public static PrefixHandler? ResolvePrefixHandler(string id)
    {
        PrefixHandler? result = null;
        var matchedLength = 0;

        foreach (var (prefix, handler) in PrefixHandlers)
        {
            if (prefix.Length > matchedLength && id.StartsWith(prefix, StringComparison.Ordinal))
            {
                result = handler;
                matchedLength = prefix.Length;
            }
        }

        return result;
    }
}