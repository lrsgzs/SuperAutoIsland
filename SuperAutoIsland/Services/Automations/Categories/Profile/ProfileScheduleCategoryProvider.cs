using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Schedule;
using SuperAutoIsland.Shared;

namespace SuperAutoIsland.Services.Automations.Categories.Profile;

/// <summary>
///     「SAI 档案 - 日程」分类提供方
/// </summary>
public class ProfileScheduleCategoryProvider : ICategoryProvider
{
    /// <inheritdoc />
    public CategoryMetadata Metadata => new("档案 - 日程")
    {
        Icon = ("日程", FluentIcons.CalendarRegular),
        Colors = new CategoryColors("#9B8F80")
    };

    /// <inheritdoc />
    public void Build(BlocksRegister it)
    {
        var features = GlobalConstants.Configs.MainConfig!.Data.ProfileFeatures;

        it.AddBlock<GetScheduleItemListBlock>()
          .AddBlock<ScheduleItemByGuidBlock>();

        if (features.Schedule.Read)
            it.AddLabel("信息")
              .AddBlock<GetScheduleItemSubjectBlock>()
              .AddBlock<GetScheduleItemStartTimeBlock>()
              .AddBlock<GetScheduleItemEndTimeBlock>()
              .AddBlock<GetTodayScheduleItemListBlock>()
              .AddBlock<CurrentScheduleItemBlock>()
              .AddBlock<HasCurrentScheduleItemRuleBlock>();

        if (features.Schedule.Write)
        {
            it.AddLabel("操作")
              .AddBlock<CreateScheduleItemBlock>()
              .AddBlock<CopyScheduleItemBlock>()
              .AddBlock<DeleteScheduleItemBlock>();

            it.AddLabel("信息编辑")
              .AddBlock<SetScheduleItemSubjectBlock>()
              .AddBlock<SetScheduleItemStartTimeBlock>()
              .AddBlock<SetScheduleItemEndTimeBlock>();

            it.AddLabel("触发规则")
              .AddBlock<SetScheduleItemWeeklyRuleBlock>()
              .AddBlock<SetScheduleItemWeeklyCycleRuleBlock>()
              .AddBlock<SetScheduleItemDateRuleBlock>()
              .AddBlock<SetScheduleItemLoopRuleBlock>()
              .AddBlock<SetScheduleItemDateRangeRuleBlock>();
        }
    }
}