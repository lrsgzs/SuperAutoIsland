using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Categories.Profile;

/// <summary>
///     「SAI 档案操作」分类提供方。
/// </summary>
public class ProfileCommonCategoryProvider : ICategoryProvider
{
    /// <inheritdoc />
    public CategoryMetadata Metadata => new("SAI 档案操作")
    {
        Icon = ("通用", FluentIcons.DocumentRegular)
    };

    /// <inheritdoc />
    public void Build(BlocksRegister it)
    {
        it
            .AddLabel("日程模式下，仅支持读取课表/时间表信息，暂不支持修改。")
            .AddBlock<EmptyGuidBlock>()
            .AddBlock<IsScheduleModeRuleBlock>()
            .AddBlock<SaveProfileBlock>();
    }
}