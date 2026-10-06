using ClassIsland.Shared;
using SuperAutoIsland.Enums;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Services.Automations.Categories;
using SuperAutoIsland.Shared;

namespace SuperAutoIsland.Services.Automations;

public static class SaiRegistry
{
    private static ISaiServer SaiServer { get; } = IAppHost.GetService<ISaiServer>();

    public static void Register()
    {
        SaiServer.AddCategory<SaiCategoryProvider>();

        SaiServer.RegisterDynamicDropdown("sai.actions.runBlockly.options", async () =>
                                              EnsureListHasItemOrDefaultListItem(
                                                  GlobalConstants.Configs.ProjectConfig!.Data.Projects
                                                                 .Where(e => e.Type is ProjectsType.BlocklyAction)
                                                                 .Select(e => (e.Name, e.Id.ToString()))
                                                                 .ToList(),
                                                  new ValueTuple<string, string>("???",
                                                      GlobalConstants.Assets.ProjectNullGuid.ToString())));

        SaiServer.RegisterDynamicDropdown("sai.actions.runJavaScript.options", async () =>
                                              EnsureListHasItemOrDefaultListItem(
                                                  GlobalConstants.Configs.ProjectConfig!.Data.Projects
                                                                 .Where(e => e.Type is ProjectsType.JavaScriptAction)
                                                                 .Select(e => (e.Name, e.Id.ToString()))
                                                                 .ToList(),
                                                  new ValueTuple<string, string>("???",
                                                      GlobalConstants.Assets.ProjectNullGuid.ToString())));

        SaiServer.RegisterDynamicDropdown("sai.actions.runActionSet.options", async () =>
                                              EnsureListHasItemOrDefaultListItem(
                                                  GlobalConstants.Configs.ProjectConfig!.Data.Projects
                                                                 .Where(e => e.Type is ProjectsType.CiActionSet)
                                                                 .Select(e => (e.Name, e.Id.ToString()))
                                                                 .ToList(),
                                                  new ValueTuple<string, string>("???",
                                                      GlobalConstants.Assets.ProjectNullGuid.ToString())));

        SaiServer.RegisterDynamicDropdown("sai.rules.runCiRuleset.options", async () =>
                                              EnsureListHasItemOrDefaultListItem(
                                                  GlobalConstants.Configs.ProjectConfig!.Data.Projects
                                                                 .Where(e => e.Type is ProjectsType.CiRuleset)
                                                                 .Select(e => (e.Name, e.Id.ToString()))
                                                                 .ToList(),
                                                  new ValueTuple<string, string>("???",
                                                      GlobalConstants.Assets.ProjectNullGuid.ToString())));
    }

    private static List<T> EnsureListHasItemOrDefaultListItem<T>(List<T> data, T defaultItem)
    {
        return data.Count > 0 ? data : [defaultItem];
    }
}
