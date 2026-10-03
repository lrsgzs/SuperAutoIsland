using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;
using SuperAutoIsland.Shared;

namespace SuperAutoIsland.Services.Automations.Categories.Profile;

/// <summary>
/// 「档案 - 课表」分类提供方
/// </summary>
public class ProfileClassPlanCategoryProvider : ICategoryProvider
{
    /// <inheritdoc />
    public CategoryMetadata Metadata => new("档案 - 课表")
    {
        Icon = ("课表", FluentIcons.CalendarLtrRegular)
    };

    /// <inheritdoc />
    public void Build(BlocksRegister it)
    {
        var features = GlobalConstants.Configs.MainConfig!.Data.ProfileFeatures;

        it
            .AddBlock<EmptyClassPlanGuidBlock>()
            .AddBlock<ClassPlanByGuidBlock>()
            .AddBlock<ClassPlanByNameBlock>()
            .AddBlock<ClassPlanByDateBlock>()
            .AddBlock<ClassPlanExistsBlock>();

        if (features.ClassPlan.Read)
        {
            it.AddLabel("课表 - 信息")
                .AddBlock<GetClassPlanNameBlock>()
                .AddBlock<GetClassPlanTimeLayoutBlock>()
                .AddBlock<GetClassPlanGroupBlock>()
                .AddBlock<GetClassPlanEnabledBlock>()
                .AddBlock<GetClassPlanSubjectBlock>()
                .AddBlock<CurrentClassPlanBlock>()
                .AddBlock<CurrentClassIndexBlock>()
                .AddBlock<CurrentTimePointIndexBlock>()
                .AddBlock<UsingClassPlanRuleBlock>()
                .AddBlock<ClassPlanSubjectRuleBlock>()
                .AddBlock<ClassPlanOverlayRuleBlock>();
        }

        if (features.ClassPlan.Write)
        {
            it.AddLabel("课表 - 操作")
                .AddBlock<CreateEmptyClassPlanBlock>()
                .AddBlock<CopyClassPlanBlock>()
                .AddBlock<CreateTempClassPlanBlock>()
                .AddBlock<CreateTempClassPlanWithDateBlock>()
                .AddBlock<DeleteClassPlanBlock>()
                .AddLabel("课表 - 信息编辑")
                .AddBlock<SetClassPlanNameBlock>()
                .AddBlock<SetClassPlanTimeLayoutBlock>()
                .AddBlock<SetClassPlanGroupBlock>()
                .AddBlock<SetClassPlanEnabledBlock>()
                .AddBlock<SetWeeklyRuleBlock>()
                .AddBlock<SetWeeklyCycleRuleBlock>()
                .AddBlock<SetDateRuleBlock>()
                .AddBlock<SetLoopRuleBlock>()
                .AddBlock<SetDateRangeRuleBlock>()
                .AddLabel("课表 - 课程编辑")
                .AddBlock<SetClassPlanSubjectBlock>()
                .AddBlock<SwapClassPlanSubjectBlock>()
                .AddLabel("课表 - 临时")
                .AddBlock<ScheduleClassPlanBlock>()
                .AddBlock<ClearScheduledClassPlanBlock>()
                .AddBlock<EnableTempClassPlanBlock>()
                .AddBlock<ClearTempClassPlanBlock>()
                .AddBlock<ClearTempOverlayBlock>();
        }
    }
}
