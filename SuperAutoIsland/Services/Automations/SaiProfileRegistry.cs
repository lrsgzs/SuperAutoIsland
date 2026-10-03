using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Services.Automations.Categories.Profile;

namespace SuperAutoIsland.Services.Automations;

public class SaiProfileRegistry
{
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