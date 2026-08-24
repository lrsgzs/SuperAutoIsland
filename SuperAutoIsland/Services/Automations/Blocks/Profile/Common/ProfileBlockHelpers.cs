using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using ProfileClassPlan = ClassIsland.Shared.Models.Profile.ClassPlan;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

public static class ProfileBlockHelpers
{
    public static Guid Guid(JsonElement settings, string name) =>
        System.Guid.TryParse(settings.GetProperty(name).GetString(), out var value) ? value : System.Guid.Empty;

    public static DateOnly Date(JsonElement settings, string name) =>
        settings.GetProperty(name).Deserialize<DateOnly>();

    public static int Number(JsonElement settings, string name) =>
        (int)settings.GetProperty(name).GetDouble();

    public static JsonElement Settings(object? value) => JsonSerializer.SerializeToElement(value);

    public static ProfileClassPlan? GetClassPlan(JsonElement settings, string name = "ClassPlan") => ClassPlan(settings, name);

    public static Guid ClassSubjectId(ProfileClassPlan plan, int oneBasedIndex)
    {
        var classItems = plan.TimeLayout?.Layouts.Where(x => x.TimeType == 0).ToList() ?? [];
        var index = oneBasedIndex - 1;
        return index >= 0 && index < classItems.Count && index < plan.Classes.Count
            ? plan.Classes[index].SubjectId
            : System.Guid.Empty;
    }

    public static ProfileClassPlan? ClassPlan(JsonElement settings, string name = "ClassPlan")
    {
        var id = Guid(settings, name);
        return IAppHost.GetService<IProfileService>().Profile.ClassPlans.GetValueOrDefault(id);
    }

    public static string GuidOutput(Guid id) => id.ToString();
}
