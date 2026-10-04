using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Subject;
using SuperAutoIsland.Shared;

namespace SuperAutoIsland.Services.Automations.Categories.Profile;

/// <summary>
///     「档案 - 科目」分类提供方
/// </summary>
public class ProfileSubjectCategoryProvider : ICategoryProvider
{
    /// <inheritdoc />
    public CategoryMetadata Metadata => new("档案 - 科目")
    {
        Icon = ("科目", FluentIcons.BookRegular)
    };

    /// <inheritdoc />
    public void Build(BlocksRegister it)
    {
        var features = GlobalConstants.Configs.MainConfig!.Data.ProfileFeatures;

        it
            .AddBlock<EmptySubjectGuidBlock>()
            .AddBlock<GetSubjectListBlock>()
            .AddBlock<SubjectByGuidBlock>()
            .AddBlock<SubjectByNameBlock>()
            .AddBlock<SubjectExistsBlock>();

        if (features.Subject.Read)
            it.AddLabel("信息")
              .AddBlock<GetSubjectNameBlock>()
              .AddBlock<GetSubjectInitialBlock>()
              .AddBlock<GetSubjectTeacherNameBlock>()
              .AddBlock<GetSubjectLocationBlock>()
              .AddBlock<GetSubjectIconBlock>()
              .AddBlock<GetSubjectColorBlock>()
              .AddBlock<SubjectIsOutDoorRuleBlock>();

        if (features.Subject.Write)
        {
            it.AddLabel("操作")
              .AddBlock<CreateSubjectBlock>()
              .AddBlock<CopySubjectBlock>()
              .AddBlock<DeleteSubjectBlock>();

            it.AddLabel("信息编辑")
              .AddBlock<SetSubjectNameBlock>()
              .AddBlock<SetSubjectInitialBlock>()
              .AddBlock<SetSubjectTeacherNameBlock>()
              .AddBlock<SetSubjectLocationBlock>()
              .AddBlock<SetSubjectIconBlock>()
              .AddBlock<SetSubjectColorBlock>()
              .AddBlock<SetSubjectIsOutDoorBlock>();
        }
    }
}