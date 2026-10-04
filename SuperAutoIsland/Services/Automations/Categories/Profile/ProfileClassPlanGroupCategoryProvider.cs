using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlanGroup;
using SuperAutoIsland.Shared;

namespace SuperAutoIsland.Services.Automations.Categories.Profile;

/// <summary>
///     「档案 - 课表群」分类提供方
/// </summary>
public class ProfileClassPlanGroupCategoryProvider : ICategoryProvider
{
    /// <inheritdoc />
    public CategoryMetadata Metadata => new("档案 - 课表群")
    {
        Icon = ("课表群", FluentIcons.GroupRegular)
    };

    /// <inheritdoc />
    public void Build(BlocksRegister it)
    {
        var features = GlobalConstants.Configs.MainConfig!.Data.ProfileFeatures;

        it
            .AddBlock<EmptyClassPlanGroupGuidBlock>()
            .AddBlock<GetClassPlanGroupListBlock>()
            .AddBlock<ClassPlanGroupByGuidBlock>()
            .AddBlock<ClassPlanGroupByNameBlock>()
            .AddBlock<ClassPlanGroupExistsBlock>();

        if (features.ClassPlanGroup.Read)
            it.AddLabel("信息")
              .AddBlock<GetClassPlanGroupNameBlock>()
              .AddBlock<CurrentClassPlanGroupBlock>();

        if (features.ClassPlanGroup.Write)
        {
            it.AddLabel("课表群 - 操作")
              .AddBlock<CreateClassPlanGroupBlock>()
              .AddBlock<DisbandClassPlanGroupBlock>()
              .AddBlock<DeleteClassPlanGroupBlock>();

            it.AddLabel("课表群 - 信息编辑")
              .AddBlock<SetClassPlanGroupNameBlock>();

            it.AddLabel("课表群 - 临时")
              .AddBlock<SetCurrentClassPlanGroupBlock>()
              .AddBlock<SetupTempClassPlanGroupBlock>()
              .AddBlock<ClearTempClassPlanGroupBlock>();
        }
    }
}