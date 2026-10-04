using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Services.Automations.Categories;

namespace SuperAutoIsland.Services.Automations;

public static class SaiClassIslandRegistry
{
    private static ISaiServer SaiServer { get; } = IAppHost.GetService<ISaiServer>();

    public static void Register()
    {
        SaiServer.AddCategory<ClassIslandCategoryProvider>();

        SaiServer.RegisterDynamicDropdown("classisland.lessons.subjects", async () =>
                                              DynamicDropdownHelper.EnsureNotEmpty(
                                                  IAppHost.GetService<IProfileService>().Profile.Subjects
                                                          .Select(x => (x.Value.Name, x.Key.ToString()))
                                                          .ToList()));

        SaiServer.RegisterDynamicDropdown("classisland.settings.componentConfigs", async () =>
                                              IAppHost.GetService<IComponentsService>().ComponentConfigs
                                                      .Select(x => (x, x))
                                                      .ToList());
    }
}
