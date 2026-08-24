namespace SuperAutoIsland.Interface.Services;

/// <summary>
/// 积木注册委托
/// </summary>
public delegate void RegisterHandler(BlocksRegister register);

/// <summary>
/// 动态下拉框处理器
/// </summary>
public delegate Task<List<(string, string)>> DynamicDropdownHandler();

/// <summary>
/// 服务器接口
/// </summary>
public interface ISaiServer
{
    /// <summary>
    /// v2 注册积木
    /// </summary>
    /// <param name="categoryName">分类名称</param>
    /// <param name="handler">注册委托 (立即执行)</param>
    public void RegisterBlocks(string categoryName, RegisterHandler handler);
    
    /// <summary>
    /// 注册动态下拉框 getter
    /// </summary>
    /// <param name="id">动态下拉框 id</param>
    /// <param name="handler">获取函数</param>
    public void RegisterDynamicDropdown(string id, DynamicDropdownHandler handler);

    /// <summary>
    /// 结束服务器（好像不能用）
    /// </summary>
    public void Shutdown();
}