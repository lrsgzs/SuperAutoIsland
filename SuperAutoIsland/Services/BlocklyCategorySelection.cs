using SuperAutoIsland.Models.Settings;
using SuperAutoIsland.Shared;

namespace SuperAutoIsland.Services;

/// <summary>
///     Blockly 分类的展示顺序与显示选择，供分类管理界面与 <see cref="SaiBlocksRegistry" /> 共用。
/// </summary>
public static class BlocklyCategorySelection
{
    /// <summary>
    ///     当前配置中的分类展示设置。
    /// </summary>
    private static BlocklyCategoriesModel? Model => GlobalConstants.Configs.MainConfig?.Data.BlocklyCategories;

    /// <summary>
    ///     解析不展示的分类名称。
    /// </summary>
    public static HashSet<string> HiddenNames()
    {
        return Model?.Hidden is { } hidden
                   ? [.. hidden]
                   : new HashSet<string>(StringComparer.Ordinal);
    }

    /// <summary>
    ///     分类管理列表中的行顺序：先按已保存的顺序，再把新注册的分类按注册顺序追加到后面。
    ///     <para>
    ///         已不存在的分类也会保留在列表中（由界面标黄提示），只有用户主动删除后才会消失。
    ///     </para>
    /// </summary>
    /// <param name="registered">当前已注册的分类名称，按注册顺序排列</param>
    public static List<string> BuildRows(IEnumerable<string> registered)
    {
        var rows = new List<string>();
        var seen = new HashSet<string>(StringComparer.Ordinal);

        if (Model?.Order is { } savedOrder)
        {
            foreach (var name in savedOrder)
            {
                if (seen.Add(name))
                {
                    rows.Add(name);
                }
            }
        }

        foreach (var name in registered)
        {
            if (seen.Add(name))
            {
                rows.Add(name);
            }
        }

        return rows;
    }

    /// <summary>
    ///     发送给 Blockly 编辑器的分类顺序：去掉已不存在的分类与用户取消展示的分类。
    /// </summary>
    /// <param name="registered">当前已注册的分类名称，按注册顺序排列</param>
    public static List<string> ResolveDisplayOrder(IReadOnlyList<string> registered)
    {
        var existing = registered.ToHashSet(StringComparer.Ordinal);
        var hidden = HiddenNames();

        return
        [
            .. BuildRows(registered).Where(name => existing.Contains(name) && !hidden.Contains(name))
        ];
    }
}
