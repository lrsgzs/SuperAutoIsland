using SuperAutoIsland.Interface.Metadata;

namespace SuperAutoIsland.Interface.Services;

/// <summary>
///     分类提供方
/// </summary>
public interface ICategoryProvider
{
    /// <summary>
    ///     分类元数据
    /// </summary>
    public CategoryMetadata Metadata { get; }

    /// <summary>
    ///     构建分类
    /// </summary>
    /// <param name="it">积木注册器（分类名称已由 <see cref="Metadata" /> 提供）</param>
    public void Build(BlocksRegister it);
}