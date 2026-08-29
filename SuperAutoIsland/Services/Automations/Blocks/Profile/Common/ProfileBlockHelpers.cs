using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using ProfileClassPlan = ClassIsland.Shared.Models.Profile.ClassPlan;
using ProfileTimeLayout = ClassIsland.Shared.Models.Profile.TimeLayout;
using ProfileTimeLayoutItem = ClassIsland.Shared.Models.Profile.TimeLayoutItem;

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

    public static ProfileTimeLayout? TimeLayout(JsonElement settings, string name = "TimeLayout")
    {
        var id = Guid(settings, name);
        return IAppHost.GetService<IProfileService>().Profile.TimeLayouts.GetValueOrDefault(id);
    }

    /// <summary>
    /// 从设置中读取「时间表 GUID[序号]」格式的时间点标识。
    /// </summary>
    public static (Guid Guid, int Index) TimeLayoutItem(JsonElement settings, string name = "TimeLayoutItem") =>
        ParseTimeLayoutItem(settings.GetProperty(name).GetString());

    /// <summary>
    /// 解析「时间表 GUID[序号]」格式的时间点标识。解析失败时返回 (Guid.Empty, 0)。
    /// </summary>
    public static (Guid Guid, int Index) ParseTimeLayoutItem(string? raw)
    {
        if (string.IsNullOrEmpty(raw))
            return (System.Guid.Empty, 0);
        var open = raw.LastIndexOf('[');
        if (open < 0 || !raw.EndsWith(']'))
            return (System.Guid.Empty, 0);
        var guidPart = raw[..open];
        var indexPart = raw[(open + 1)..^1];
        return (System.Guid.TryParse(guidPart, out var guid) ? guid : System.Guid.Empty,
            int.TryParse(indexPart, out var index) ? index : 0);
    }

    /// <summary>
    /// 获取「时间表 GUID[序号]」所指向的时间点。序号越界或标识无效时返回 null。
    /// </summary>
    public static ProfileTimeLayoutItem? TimePoint(JsonElement settings, string name = "TimeLayoutItem")
    {
        var (guid, index) = TimeLayoutItem(settings, name);
        if (index < 1)
            return null;
        var layout = IAppHost.GetService<IProfileService>().Profile.TimeLayouts.GetValueOrDefault(guid);
        return layout?.Layouts.ElementAtOrDefault(index - 1);
    }

    /// <summary>
    /// 获取时间表中第 N 节课（类型为「上课」的时间点）在时间表中的实际位置（从 1 开始计数）。
    /// 不存在时返回 0。
    /// </summary>
    public static int ClassPeriodPosition(JsonElement settings, string name = "TimeLayout")
    {
        var layout = TimeLayout(settings, name);
        if (layout is null)
            return 0;
        var index = Number(settings, "Index") - 1;
        if (index < 0)
            return 0;
        var classItems = layout.Layouts.Where(x => x.TimeType == 0).ToList();
        return index < classItems.Count ? layout.Layouts.IndexOf(classItems[index]) + 1 : 0;
    }

    public static string GuidOutput(Guid id) => id.ToString();
}
