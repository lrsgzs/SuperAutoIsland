using System.Text.Json;
using SuperAutoIsland.Interface.Metadata;

namespace SuperAutoIsland.Interface.Services;

/// <summary>
///     积木注册委托
/// </summary>
public delegate void RegisterHandler(BlocksRegister register);

/// <summary>
///     动态下拉框处理器
/// </summary>
public delegate Task<List<(string, string)>> DynamicDropdownHandler();

/// <summary>
///     前缀处理器，将会在 UI 线程运行
/// </summary>
/// <param name="kind">积木类型</param>
/// <param name="id">积木 id</param>
/// <param name="settings">积木设置</param>
/// <returns>
///     Handled 为 true 时表示此次调用已被处理，使用 Result 作为结果（行动忽略 Result，规则使用 Boolean，数据使用返回的 object）；
///     Handled 为 false 时走原处理逻辑。
/// </returns>
public delegate (bool Handled, object? Result) PrefixHandler(BlockKind kind, string id, JsonElement settings);

/// <summary>
///     服务器接口
/// </summary>
public interface ISaiServer
{
    /// <summary>
    ///     v2 注册积木（已兼容至 v3 分类提供方，通过中间件实现，注册后会立即构建分类）
    /// </summary>
    /// <param name="categoryName">分类名称</param>
    /// <param name="handler">注册委托 (立即执行)</param>
    public void RegisterBlocks(string categoryName, RegisterHandler handler);

    /// <summary>
    ///     v3 注册分类提供方
    /// </summary>
    /// <typeparam name="TProvider">分类提供方类型</typeparam>
    public void AddCategory<TProvider>() where TProvider : ICategoryProvider, new();

    /// <summary>
    ///     v3 注册分类提供方
    /// </summary>
    /// <param name="provider">分类提供方</param>
    public void AddCategory(ICategoryProvider provider);

    /// <summary>
    ///     通知 SAI 更新所有分类的积木和 Block 列表（重新构建所有已注册的分类提供方）
    /// </summary>
    public void NotifyCategoryUpdated();

    /// <summary>
    ///     注册前缀处理器（积木 id 以 prefix 开头时，由该处理器优先处理）
    /// </summary>
    /// <param name="prefix">积木 id 前缀</param>
    /// <param name="handler">处理器</param>
    public void AddPrefixHandler(string prefix, PrefixHandler handler);

    /// <summary>
    ///     注册动态下拉框 getter
    /// </summary>
    /// <param name="id">动态下拉框 id</param>
    /// <param name="handler">获取函数</param>
    public void RegisterDynamicDropdown(string id, DynamicDropdownHandler handler);

    /// <summary>
    ///     结束服务器（好像不能用）
    /// </summary>
    public void Shutdown();
}