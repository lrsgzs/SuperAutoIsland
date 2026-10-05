using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Services.Automations.Blocks.Profile.TimePoint;
using SuperAutoIsland.Shared;

namespace SuperAutoIsland.Services.Automations.Categories.Profile;

/// <summary>
///     「档案 - 时间点」分类提供方
/// </summary>
public class ProfileTimePointCategoryProvider : ICategoryProvider
{
    /// <inheritdoc />
    public CategoryMetadata Metadata => new("档案 - 时间点")
    {
        Icon = ("时间点", FluentIcons.ClockRegular),
        Colors = new CategoryColors("#9B8F80")
    };

    /// <inheritdoc />
    public void Build(BlocksRegister it)
    {
        var features = GlobalConstants.Configs.MainConfig!.Data.ProfileFeatures;

        it
            .AddBlock<TimePointTypeBlock>()
            .AddBlock<TimePointCountBlock>();

        if (features.TimePoint.Read)
        {
            it.AddLabel("获取")
              .AddBlock<TimeLayoutItemBlock>()
              .AddBlock<GetClassPeriodItemBlock>()
              .AddBlock<FindTimePointByTimeBlock>()
              .AddBlock<TimeLayoutItemExistsRuleBlock>()
              .AddBlock<TimePointIndexBlock>()
              .AddBlock<TimePointClassIndexBlock>();

            it.AddLabel("信息")
              .AddBlock<GetTimePointItemTypeBlock>()
              .AddBlock<GetTimePointStartTimeBlock>()
              .AddBlock<GetTimePointEndTimeBlock>()
              .AddBlock<GetTimePointDurationBlock>()
              .AddBlock<GetTimePointBreakNameBlock>();
        }

        if (features.TimePoint.Write)
        {
            it.AddLabel("操作")
              .AddBlock<AddTimePointBlock>()
              .AddBlock<DeleteTimePointBlock>();

            it.AddLabel("信息编辑")
              .AddBlock<SetTimePointStartTimeBlock>()
              .AddBlock<SetTimePointEndTimeBlock>()
              .AddBlock<SetTimePointTypeBlock>()
              .AddBlock<SetTimePointBreakNameBlock>()
              .AddBlock<SetTimePointDefaultSubjectBlock>()
              .AddBlock<SetTimePointHideDefaultBlock>()
              .AddBlock<SetTimePointActionSetBlock>()
              .AddBlock<OverwriteAllClassPlanSubjectsBlock>();
        }
    }
}