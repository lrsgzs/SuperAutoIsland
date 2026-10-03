using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;
using SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlanGroup;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Subject;
using SuperAutoIsland.Services.Automations.Blocks.Profile.TimeLayout;
using SuperAutoIsland.Services.Automations.Blocks.Profile.TimePoint;
using SuperAutoIsland.Shared;

namespace SuperAutoIsland.Services.Automations;

/// <summary>
/// 「SAI 档案操作」分类提供方。
/// 各板块的积木分为读取、写入两部分，由 <see cref="Models.Settings.ProfileFeaturesModel"/> 中的开关决定是否输出。
/// 子开关变化后调用 <see cref="ISaiServer.NotifyCategoryUpdated"/> 重新构建即可生效，无需重启。
/// </summary>
public class ProfileCategoryProvider : ICategoryProvider
{
    private static ISaiServer SaiServer { get; } = IAppHost.GetService<ISaiServer>();
    private static IProfileService ProfileService { get; } = IAppHost.GetService<IProfileService>();

    /// <inheritdoc />
    public CategoryMetadata Metadata => new("SAI 档案操作")
    {
        Icon = ("档案操作", FluentIcons.DocumentRegular)
    };

    /// <inheritdoc />
    public void Build(BlocksRegister it)
    {
        var features = GlobalConstants.Configs.MainConfig!.Data.ProfileFeatures;

        it.AddLabel("日程模式下，仅支持读取课表/时间表信息，暂不支持修改。");

        it.AddLabel("通用")
            .AddBlock<EmptyGuidBlock>()
            .AddBlock<SaveProfileBlock>();

        it.AddLabel("科目")
            .AddBlock<EmptySubjectGuidBlock>()
            .AddBlock<SubjectByGuidBlock>()
            .AddBlock<SubjectByNameBlock>()
            .AddBlock<SubjectExistsBlock>();
        
        if (features.Subject.Read)
        {
            it.AddLabel("科目 - 信息")
                .AddBlock<GetSubjectNameBlock>()
                .AddBlock<GetSubjectInitialBlock>()
                .AddBlock<GetSubjectTeacherNameBlock>()
                .AddBlock<GetSubjectLocationBlock>()
                .AddBlock<GetSubjectIconBlock>()
                .AddBlock<GetSubjectColorBlock>()
                .AddBlock<SubjectIsOutDoorRuleBlock>();
        }

        if (features.Subject.Write)
        {
            it.AddLabel("科目 - 操作")
                .AddBlock<CreateSubjectBlock>()
                .AddBlock<CopySubjectBlock>()
                .AddBlock<DeleteSubjectBlock>();

            it.AddLabel("科目 - 信息编辑")
                .AddBlock<SetSubjectNameBlock>()
                .AddBlock<SetSubjectInitialBlock>()
                .AddBlock<SetSubjectTeacherNameBlock>()
                .AddBlock<SetSubjectLocationBlock>()
                .AddBlock<SetSubjectIconBlock>()
                .AddBlock<SetSubjectColorBlock>()
                .AddBlock<SetSubjectIsOutDoorBlock>();
        }
        
        it.AddLabel("时间点")
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

        it.AddLabel("时间表")
            .AddBlock<EmptyTimeLayoutGuidBlock>()
            .AddBlock<TimeLayoutByGuidBlock>()
            .AddBlock<TimeLayoutByNameBlock>()
            .AddBlock<TimeLayoutExistsBlock>();

        if (features.TimeLayout.Read)
        {
            it.AddLabel("时间表 - 信息")
                .AddBlock<GetTimeLayoutNameBlock>();
        }

        if (features.TimeLayout.Write)
        {
            it.AddLabel("时间表 - 操作")
                .AddBlock<CreateTimeLayoutBlock>()
                .AddBlock<CopyTimeLayoutBlock>()
                .AddBlock<DeleteTimeLayoutBlock>();

            it.AddLabel("时间表 - 信息编辑")
                .AddBlock<SetTimeLayoutNameBlock>();
        }

        it.AddLabel("课表")
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
        
        it.AddLabel("课表群")
            .AddBlock<EmptyClassPlanGroupGuidBlock>()
            .AddBlock<ClassPlanGroupByGuidBlock>()
            .AddBlock<ClassPlanGroupByNameBlock>()
            .AddBlock<ClassPlanGroupExistsBlock>();

        if (features.ClassPlanGroup.Read)
        {
            it.AddLabel("课表群 - 信息")
                .AddBlock<GetClassPlanGroupNameBlock>()
                .AddBlock<CurrentClassPlanGroupBlock>();
        }

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

    public static void Register()
    {
        SaiServer.AddCategory<ProfileCategoryProvider>();

        SaiServer.RegisterDynamicDropdown("sai.profile.dd.subjects", () =>
            Task.FromResult(DynamicDropdownHelper.EnsureNotEmpty(ProfileService.Profile.Subjects
                .Select(x => (x.Value.Name, x.Key.ToString()))
                .ToList())));

        SaiServer.RegisterDynamicDropdown("sai.profile.dd.timeLayouts", () =>
            Task.FromResult(DynamicDropdownHelper.EnsureNotEmpty(ProfileService.Profile.TimeLayouts
                .Select(x => (x.Value.Name, x.Key.ToString()))
                .ToList())));

        SaiServer.RegisterDynamicDropdown("sai.profile.dd.classPlans", () =>
            Task.FromResult(DynamicDropdownHelper.EnsureNotEmpty(ProfileService.Profile.ClassPlans
                .Select(x => (x.Value.Name, x.Key.ToString()))
                .ToList())));

        SaiServer.RegisterDynamicDropdown("sai.profile.dd.classPlanGroups", () =>
            Task.FromResult(ProfileService.Profile.ClassPlanGroups
                .Select(x => (x.Value.Name, x.Key.ToString()))
                .ToList()));
    }
}
