using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Shared;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Services;

/// <summary>
/// 服务器桥接器
/// </summary>
public class SaiServerBridger : ISaiServer
{
    private readonly SaiServer _instance;
    private readonly Logger<SaiServerBridger> _logger = new();

    /// <summary>
    /// 构造函数
    /// <see cref="SaiServerBridger"/>
    /// </summary>
    public SaiServerBridger()
    {
        _instance = new SaiServer(GlobalConstants.Configs.MainConfig!.Data.ServerPort);
        _logger.Info($"服务器地址：{_instance.Url}");
        _ = _instance.Serve();

        _logger.Info("已初始化 SaiServer！");
    }

    /// <inheritdoc />
    public void RegisterBlocks(string categoryName, RegisterHandler handler)
    {
        // 原 v2 接口通过中间件转移到 v3 的分类提供方结构
        AddCategory(new MiddlewareCategoryProvider(categoryName, handler));
    }

    /// <inheritdoc />
    public void AddCategory<TProvider>() where TProvider : ICategoryProvider, new() => AddCategory(new TProvider());

    /// <inheritdoc />
    public void AddCategory(ICategoryProvider provider)
    {
        var name = provider.Metadata.Name;

        if (SaiBlocksRegistry.CategoryProviders.RemoveAll(p => p.Metadata.Name == name) > 0)
        {
            _logger.Warn($"分类 {name} 已存在，已覆盖原分类");
        }

        SaiBlocksRegistry.CategoryProviders.Add(provider);
        _logger.Info($"已注册分类 {name}");

        NotifyCategoryUpdated();
    }

    /// <inheritdoc />
    public void NotifyCategoryUpdated()
    {
        SaiBlocksRegistry.Rebuild();
    }

    /// <inheritdoc />
    public void AddPrefixHandler(string prefix, PrefixHandler handler)
    {
        SaiBlocksRegistry.PrefixHandlers[prefix] = handler;
        _logger.Info($"已注册前缀为 {prefix} 的 PrefixHandler");
    }

    /// <inheritdoc />
    public void RegisterDynamicDropdown(string id, DynamicDropdownHandler handler)
    {
        SaiBlocksRegistry.DynamicDropdowns[id] = handler;
        _logger.Info($"已注册 id 为 {id} 的 DynamicDropdownGetter");
    }

    /// <inheritdoc />
    public void Shutdown()
    {
        _instance.Shutdown();
    }
}