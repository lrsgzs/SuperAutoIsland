using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;

namespace SuperAutoIsland.Services;

/// <summary>
///     分类提供方中间件：把 v2 的 <see cref="ISaiServer.RegisterBlocks" /> 适配到 v3 的分类提供方结构
/// </summary>
/// <param name="categoryName">分类名称</param>
/// <param name="handler">v2 注册委托</param>
public class MiddlewareCategoryProvider(string categoryName, RegisterHandler handler) : ICategoryProvider
{
    /// <inheritdoc />
    public CategoryMetadata Metadata { get; } = new(categoryName);

    /// <inheritdoc />
    public void Build(BlocksRegister it)
    {
        handler(it);
    }
}