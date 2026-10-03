using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Services.Automations.Blocks.Profile.TimeLayout;
using SuperAutoIsland.Shared;

namespace SuperAutoIsland.Services.Automations.Categories.Profile;

/// <summary>
///     「档案 - 时间表」分类提供方
/// </summary>
public class ProfileTimeLayoutCategoryProvider : ICategoryProvider
{
    /// <inheritdoc />
    public CategoryMetadata Metadata => new("档案 - 时间表")
    {
        Icon = ("时间表", FluentIcons.TableRegular)
    };

    /// <inheritdoc />
    public void Build(BlocksRegister it)
    {
        var features = GlobalConstants.Configs.MainConfig!.Data.ProfileFeatures;

        it
            .AddBlock<EmptyTimeLayoutGuidBlock>()
            .AddBlock<GetTimeLayoutListBlock>()
            .AddBlock<TimeLayoutByGuidBlock>()
            .AddBlock<TimeLayoutByNameBlock>()
            .AddBlock<TimeLayoutExistsBlock>();

        if (features.TimeLayout.Read)
            it.AddLabel("时间表 - 信息")
              .AddBlock<GetTimeLayoutNameBlock>();

        if (features.TimeLayout.Write)
        {
            it.AddLabel("时间表 - 操作")
              .AddBlock<CreateTimeLayoutBlock>()
              .AddBlock<CopyTimeLayoutBlock>()
              .AddBlock<DeleteTimeLayoutBlock>();

            it.AddLabel("时间表 - 信息编辑")
              .AddBlock<SetTimeLayoutNameBlock>();
        }
    }
}