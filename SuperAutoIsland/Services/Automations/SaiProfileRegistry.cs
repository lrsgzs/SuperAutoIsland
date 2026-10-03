using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using ClassIsland.Shared.Models.Profile;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Services.Automations.Categories.Profile;

namespace SuperAutoIsland.Services.Automations;

public class SaiProfileRegistry
{
    private static readonly string[] WeekDayNames = ["周日", "周一", "周二", "周三", "周四", "周五", "周六"];
    private static ISaiServer SaiServer { get; } = IAppHost.GetService<ISaiServer>();
    private static IProfileService ProfileService { get; } = IAppHost.GetService<IProfileService>();

    public static void Register()
    {
        SaiServer.AddCategory<ProfileCommonCategoryProvider>();
        SaiServer.AddCategory<ProfileSubjectCategoryProvider>();
        SaiServer.AddCategory<ProfileTimePointCategoryProvider>();
        SaiServer.AddCategory<ProfileTimeLayoutCategoryProvider>();
        SaiServer.AddCategory<ProfileClassPlanCategoryProvider>();
        SaiServer.AddCategory<ProfileClassPlanGroupCategoryProvider>();
        SaiServer.AddCategory<ProfileScheduleCategoryProvider>();

        SaiServer.RegisterDynamicDropdown("sai.profile.dd.subjects", () =>
                                              Task.FromResult(DynamicDropdownHelper.EnsureNotEmpty(
                                                                  ProfileService.Profile.Subjects
                                                                      .Select(x => (x.Value.Name, x.Key.ToString()))
                                                                      .ToList())));

        SaiServer.RegisterDynamicDropdown("sai.profile.dd.timeLayouts", () =>
                                              Task.FromResult(DynamicDropdownHelper.EnsureNotEmpty(
                                                                  ProfileService.Profile.TimeLayouts
                                                                      .Select(x => (x.Value.Name, x.Key.ToString()))
                                                                      .ToList())));

        SaiServer.RegisterDynamicDropdown("sai.profile.dd.classPlans", () =>
                                              Task.FromResult(DynamicDropdownHelper.EnsureNotEmpty(
                                                                  ProfileService.Profile.ClassPlans
                                                                      .Select(x => (x.Value.Name, x.Key.ToString()))
                                                                      .ToList())));

        SaiServer.RegisterDynamicDropdown("sai.profile.dd.classPlanGroups", () =>
                                              Task.FromResult(ProfileService.Profile.ClassPlanGroups
                                                                            .Select(x => (x.Value.Name,
                                                                                            x.Key.ToString()))
                                                                            .ToList()));

        SaiServer.RegisterDynamicDropdown("sai.profile.dd.scheduleItems", () =>
                                              Task.FromResult(DynamicDropdownHelper.EnsureNotEmpty(
                                                                  ProfileService.Profile.ScheduleItems
                                                                      .Select(x => (DescribeScheduleItem(x.Value),
                                                                                              x.Key.ToString()))
                                                                      .ToList())));
    }

    /// <summary>
    ///     生成课程（日程项目）在下拉框中的显示名称，格式类似于「数学 08:00-08:45 周一」。
    /// </summary>
    private static string DescribeScheduleItem(ScheduleItem item)
    {
        var subject = ProfileService.Profile.Subjects.GetValueOrDefault(item.SubjectId)?.Name;
        var name = string.IsNullOrWhiteSpace(subject) ? "未指定科目" : subject;
        var time = $"{item.StartTime:hh\\:mm}-{item.EndTime:hh\\:mm}";
        return item.EnableRule.Type == TimeRule.TimeRuleType.Weekly
                   ? $"{name} {time} {WeekDayNames[Math.Clamp(item.EnableRule.WeekDay, 0, 6)]}"
                   : $"{name} {time} {item.EnableRule.TypeString}";
    }
}