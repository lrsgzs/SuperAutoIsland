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
        Icon = ("时间点", FluentIcons.ClockRegular)
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
            it.AddLabel("时间点 - 获取")
              .AddBlock<TimeLayoutItemBlock>()
              .AddBlock<GetClassPeriodItemBlock>()
              .AddBlock<TimeLayoutItemExistsRuleBlock>()
              .AddBlock<TimePointIndexBlock>()
              .AddBlock<TimePointClassIndexBlock>();

            it.AddLabel("时间点 - 信息")
              .AddBlock<GetTimePointItemTypeBlock>()
              .AddBlock<GetTimePointStartTimeBlock>()
              .AddBlock<GetTimePointEndTimeBlock>()
              .AddBlock<GetTimePointDurationBlock>()
              .AddBlock<GetTimePointBreakNameBlock>();
        }
    }
}