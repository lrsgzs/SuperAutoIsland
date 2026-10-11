using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Services.Automations.Blocks.Files;

namespace SuperAutoIsland.Services.Automations.Categories;

/// <summary>
///     「SAI 文件操作」分类提供方：文本文件的读写与路径判断（实验性功能）。
/// </summary>
public class FileCategoryProvider : ICategoryProvider
{
    /// <inheritdoc />
    public CategoryMetadata Metadata => new("SAI 文件操作")
    {
        Icon = ("文件夹", FluentIcons.FolderRegular),
        Colors = new CategoryColors("#AE8C3D")
    };

    /// <inheritdoc />
    public void Build(BlocksRegister it)
    {
        it
            .AddBlock<ReadTextFileBlock>()
            .AddBlock<WriteTextFileBlock>()
            .AddBlock<ListDirectoryBlock>()
            .AddBlock<FileExistsBlock>()
            .AddBlock<DirectoryExistsBlock>();
    }
}