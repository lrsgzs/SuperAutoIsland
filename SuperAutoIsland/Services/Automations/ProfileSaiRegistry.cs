using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;
using SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlanGroup;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Subject;
using SuperAutoIsland.Services.Automations.Blocks.Profile.TimeLayout;

namespace SuperAutoIsland.Services.Automations;

public static class ProfileSaiRegistry
{
    private static ISaiServer SaiServer { get; } = IAppHost.GetService<ISaiServer>();
    private static IProfileService ProfileService { get; } = IAppHost.GetService<IProfileService>();

    public static void Register()
    {
        SaiServer.RegisterBlocks("SAI 档案操作", it => it
            .AddLabel("通用")
            .AddBlock<EmptyGuidBlock>()
            .AddBlock<SaveProfileBlock>()
            .AddLabel("科目")
            .AddBlock<EmptySubjectGuidBlock>()
            .AddBlock<SubjectByGuidBlock>()
            .AddBlock<SubjectByNameBlock>()
            .AddBlock<GetSubjectNameBlock>()
            .AddBlock<SubjectExistsBlock>()
            .AddLabel("时间表")
            .AddBlock<EmptyTimeLayoutGuidBlock>()
            .AddBlock<TimeLayoutByGuidBlock>()
            .AddBlock<TimeLayoutByNameBlock>()
            .AddBlock<GetTimeLayoutNameBlock>()
            .AddBlock<TimeLayoutExistsBlock>()
            .AddLabel("课表")
            .AddBlock<EmptyClassPlanGuidBlock>()
            .AddBlock<ClassPlanByGuidBlock>()
            .AddBlock<ClassPlanByNameBlock>()
            .AddBlock<GetClassPlanNameBlock>()
            .AddBlock<ClassPlanExistsBlock>()
            .AddLabel("课表 - 信息")
            .AddBlock<CurrentClassPlanBlock>()
            .AddBlock<CurrentClassIndexBlock>()
            .AddBlock<GetClassPlanTimeLayoutBlock>()
            .AddBlock<GetClassPlanSubjectBlock>()
            .AddBlock<UsingClassPlanRuleBlock>()
            .AddBlock<ClassPlanSubjectRuleBlock>()
            .AddBlock<ClassPlanOverlayRuleBlock>()
            .AddLabel("课表 - 操作")
            .AddBlock<CreateEmptyClassPlanBlock>()
            .AddBlock<CopyClassPlanBlock>()
            .AddBlock<CreateTempClassPlanBlock>()
            .AddBlock<CreateTempClassPlanWithDateBlock>()
            .AddBlock<DeleteClassPlanBlock>()
            .AddBlock<SetClassPlanSubjectBlock>()
            .AddBlock<SwapClassPlanSubjectBlock>()
            .AddLabel("课表 - 信息编辑")
            .AddBlock<SetClassPlanNameBlock>()
            .AddBlock<SetClassPlanTimeLayoutBlock>()
            .AddBlock<SetClassPlanGroupBlock>()
            .AddBlock<SetWeeklyRuleBlock>()
            .AddBlock<SetWeeklyCycleRuleBlock>()
            .AddBlock<SetDateRuleBlock>()
            .AddBlock<SetLoopRuleBlock>()
            .AddBlock<SetDateRangeRuleBlock>()
            .AddLabel("课表 - 临时")
            .AddBlock<ScheduleClassPlanBlock>()
            .AddBlock<ClearScheduledClassPlanBlock>()
            .AddBlock<EnableTempClassPlanBlock>()
            .AddBlock<ClearTempClassPlanBlock>()
            .AddBlock<ClearTempOverlayBlock>()
            .AddLabel("课表群")
            .AddBlock<EmptyClassPlanGroupGuidBlock>()
            .AddBlock<ClassPlanGroupByGuidBlock>()
            .AddBlock<ClassPlanGroupByNameBlock>()
            .AddBlock<GetClassPlanGroupNameBlock>()
            .AddBlock<ClassPlanGroupExistsBlock>()
            .AddLabel("课表群 - 临时")
            .AddBlock<CurrentClassPlanGroupBlock>()
            .AddBlock<SetCurrentClassPlanGroupBlock>()
            .AddBlock<SetupTempClassPlanGroupBlock>()
            .AddBlock<ClearTempClassPlanGroupBlock>());

        SaiServer.RegisterDynamicDropdown("sai.profile.dd.subjects", () =>
            Task.FromResult(ProfileService.Profile.Subjects
                .Select(x => (x.Value.Name, x.Key.ToString()))
                .ToList()));

        SaiServer.RegisterDynamicDropdown("sai.profile.dd.timeLayouts", () =>
            Task.FromResult(ProfileService.Profile.TimeLayouts
                .Select(x => (x.Value.Name, x.Key.ToString()))
                .ToList()));

        SaiServer.RegisterDynamicDropdown("sai.profile.dd.classPlans", () =>
            Task.FromResult(ProfileService.Profile.ClassPlans
                .Select(x => (x.Value.Name, x.Key.ToString()))
                .ToList()));

        SaiServer.RegisterDynamicDropdown("sai.profile.dd.classPlanGroups", () =>
            Task.FromResult(ProfileService.Profile.ClassPlanGroups
                .Select(x => (x.Value.Name, x.Key.ToString()))
                .ToList()));
    }
}
