using System.Globalization;
using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using ClassIsland.Shared.Enums;
using ProfileClassPlan = ClassIsland.Shared.Models.Profile.ClassPlan;
using ProfileScheduleItem = ClassIsland.Shared.Models.Profile.ScheduleItem;
using ProfileClassPlanGroup = ClassIsland.Shared.Models.Profile.ClassPlanGroup;
using ProfileSubject = ClassIsland.Shared.Models.Profile.Subject;
using ProfileTimeLayout = ClassIsland.Shared.Models.Profile.TimeLayout;
using ProfileTimeLayoutItem = ClassIsland.Shared.Models.Profile.TimeLayoutItem;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

public static class ProfileBlockHelpers
{
    private const string ScheduleRefPrefix = "[sched]";

    public static Guid Guid(JsonElement settings, string name) =>
        System.Guid.TryParse(settings.GetProperty(name).GetString(), out var value) ? value : System.Guid.Empty;

    public static DateOnly Date(JsonElement settings, string name) =>
        settings.GetProperty(name).Deserialize<DateOnly>();

    public static int Number(JsonElement settings, string name) =>
        (int)settings.GetProperty(name).GetDouble();

    public static bool Bool(JsonElement settings, string name)
    {
        var value = settings.GetProperty(name);
        return value.ValueKind switch
        {
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.String => value.GetString() == "TRUE",
            _ => false
        };
    }

    public static JsonElement Settings(object? value) => JsonSerializer.SerializeToElement(value);

    public static ProfileClassPlan? GetClassPlan(JsonElement settings, string name = "ClassPlan") => ClassPlan(settings, name);

    /// <summary>
    /// 读取课表引用原始字符串（GUID 或「[sched]yyyy-MM-dd」）。
    /// </summary>
    public static string ClassPlanRef(JsonElement settings, string name = "ClassPlan") =>
        settings.GetProperty(name).GetString() ?? string.Empty;

    /// <summary>
    /// 判断引用是否为日程模式引用。
    /// </summary>
    public static bool IsScheduleRef(string? reference) =>
        reference?.StartsWith(ScheduleRefPrefix, StringComparison.Ordinal) == true;

    /// <summary>
    /// 构造某一日期的日程模式引用。
    /// </summary>
    public static string CreateScheduleRef(DateOnly date) =>
        $"{ScheduleRefPrefix}{date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}";

    /// <summary>
    /// 解析日程模式引用中的日期。
    /// </summary>
    public static bool TryParseScheduleRef(string? reference, out DateOnly date)
    {
        date = default;
        return IsScheduleRef(reference) && DateOnly.TryParseExact(
            reference![ScheduleRefPrefix.Length..],
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out date);
    }

    /// <summary>
    /// 当前档案的课程模式。
    /// </summary>
    public static ScheduleType CurrentScheduleType =>
        IAppHost.GetService<IProfileService>().Profile.ScheduleType;

    /// <summary>
    /// 当前档案是否处于日程模式。
    /// </summary>
    public static bool IsScheduleMode => CurrentScheduleType == ScheduleType.Schedule;

    /// <summary>
    /// 获取指定日期生效的课表引用。经典模式返回课表 GUID，日程模式返回「[sched]yyyy-MM-dd」引用。
    /// 当日没有课表时返回空 GUID。
    /// </summary>
    public static string ClassPlanRefByDate(DateTime date)
    {
        var plan = IAppHost.GetService<ILessonsService>().GetClassPlanByDate(date, out var id);
        if (IsScheduleMode)
            return plan is null ? System.Guid.Empty.ToString() : CreateScheduleRef(DateOnly.FromDateTime(date));

        return (id ?? System.Guid.Empty).ToString();
    }

    /// <summary>
    /// 解析课表引用。日程模式引用仅在档案处于日程模式时有效。
    /// </summary>
    public static ProfileClassPlan? ResolveClassPlan(string? reference)
    {
        if (string.IsNullOrEmpty(reference))
            return null;

        if (IsScheduleRef(reference))
        {
            if (!TryParseScheduleRef(reference, out var date) || !IsScheduleMode)
                return null;

            // 日程模式生成的课表没有 GUID，直接按日期取合成的课表。
            return IAppHost.GetService<ILessonsService>()
                .GetClassPlanByDate(date.ToDateTime(TimeOnly.MinValue), out _);
        }

        if (!System.Guid.TryParse(reference, out var guid))
            return null;
        return IAppHost.GetService<IProfileService>().Profile.ClassPlans.GetValueOrDefault(guid);
    }

    public static ProfileClassPlan? ClassPlan(JsonElement settings, string name = "ClassPlan") =>
        ResolveClassPlan(ClassPlanRef(settings, name));

    /// <summary>
    /// 仅解析经典模式课表（GUID）。用于写操作，避免误改日程模式生成的临时课表。
    /// </summary>
    public static ProfileClassPlan? ClassicClassPlan(JsonElement settings, string name = "ClassPlan")
    {
        var id = Guid(settings, name);
        return IAppHost.GetService<IProfileService>().Profile.ClassPlans.GetValueOrDefault(id);
    }

    /// <summary>
    /// 按 GUID 解析科目。用于写操作。
    /// </summary>
    public static ProfileSubject? Subject(JsonElement settings, string name = "Subject") =>
        IAppHost.GetService<IProfileService>().Profile.Subjects.GetValueOrDefault(Guid(settings, name));

    /// <summary>
    /// 按 GUID 解析时间表。用于写操作，日程模式引用不会被解析成日程模式生成的时间表。
    /// </summary>
    public static ProfileTimeLayout? ClassicTimeLayout(JsonElement settings, string name = "TimeLayout") =>
        IAppHost.GetService<IProfileService>().Profile.TimeLayouts.GetValueOrDefault(Guid(settings, name));

    /// <summary>
    /// 按 GUID 解析课表群。用于写操作。
    /// </summary>
    public static ProfileClassPlanGroup? ClassPlanGroup(JsonElement settings, string name = "ClassPlanGroup") =>
        IAppHost.GetService<IProfileService>().Profile.ClassPlanGroups.GetValueOrDefault(Guid(settings, name));

    /// <summary>
    /// 读取时间设置（「HH:mm:ss」文本）。
    /// </summary>
    public static TimeSpan Time(JsonElement settings, string name)
    {
        var raw = settings.GetProperty(name).GetString();
        return TimeSpan.TryParse(raw, CultureInfo.InvariantCulture, out var value) ? value : TimeSpan.Zero;
    }

    /// <summary>
    /// 按 GUID 解析课程（日程项目）。用于写操作。
    /// </summary>
    public static ProfileScheduleItem? ScheduleItem(JsonElement settings, string name = "ScheduleItem") =>
        IAppHost.GetService<IProfileService>().Profile.ScheduleItems.GetValueOrDefault(Guid(settings, name));

    /// <summary>
    /// 获取当天生效的课程（日程项目）。
    /// </summary>
    public static OrderedDictionary<Guid, ProfileScheduleItem> TodayScheduleItems() =>
        IAppHost.GetService<ILessonsService>().GetScheduleItemsByDate(
            DateOnly.FromDateTime(IAppHost.GetService<IExactTimeService>().GetCurrentLocalDateTime()));

    /// <summary>
    /// 获取当前正在进行中的课程（日程项目）。没有正在进行中的课程时返回 null。
    /// 有多个课程重叠时取开始时间最晚的一个。
    /// </summary>
    public static (Guid Id, ProfileScheduleItem Item)? CurrentScheduleItem()
    {
        var time = IAppHost.GetService<IExactTimeService>().GetCurrentLocalDateTime().TimeOfDay;
        (Guid Id, ProfileScheduleItem Item)? current = null;

        foreach (var (id, item) in TodayScheduleItems())
        {
            if (item.StartTime > time || item.EndTime < time)
                continue;

            if (current is null || item.StartTime > current.Value.Item.StartTime)
                current = (id, item);
        }

        return current;
    }

    /// <summary>
    /// 清理指向已被移除的课表群的引用（当前启用的课表群与临时课表群）。
    /// </summary>
    /// <param name="id">已被移除的课表群 GUID</param>
    public static void ClearClassPlanGroupReferences(Guid id)
    {
        var profileService = IAppHost.GetService<IProfileService>();
        var profile = profileService.Profile;

        if (profile.SelectedClassPlanGroupId == id)
        {
            profile.SelectedClassPlanGroupId = ProfileClassPlanGroup.DefaultGroupGuid;
        }

        if (profile.TempClassPlanGroupId == id)
        {
            profileService.ClearTempClassPlanGroup();
        }
    }

    public static Guid ClassSubjectId(ProfileClassPlan plan, int oneBasedIndex)
    {
        var classItems = plan.TimeLayout?.Layouts.Where(x => x.TimeType == 0).ToList() ?? [];
        var index = oneBasedIndex - 1;
        return index >= 0 && index < classItems.Count && index < plan.Classes.Count
            ? plan.Classes[index].SubjectId
            : System.Guid.Empty;
    }

    /// <summary>
    /// 读取时间表引用原始字符串（GUID 或「[sched]yyyy-MM-dd」）。
    /// </summary>
    public static string TimeLayoutRef(JsonElement settings, string name = "TimeLayout") =>
        settings.GetProperty(name).GetString() ?? string.Empty;

    /// <summary>
    /// 解析时间表引用。日程模式引用取该日合成课表的时间表。
    /// </summary>
    public static ProfileTimeLayout? ResolveTimeLayout(string? reference)
    {
        if (string.IsNullOrEmpty(reference))
            return null;

        if (IsScheduleRef(reference))
            return ResolveClassPlan(reference)?.TimeLayout;

        if (!System.Guid.TryParse(reference, out var id))
            return null;
        return IAppHost.GetService<IProfileService>().Profile.TimeLayouts.GetValueOrDefault(id);
    }

    public static ProfileTimeLayout? TimeLayout(JsonElement settings, string name = "TimeLayout") =>
        ResolveTimeLayout(TimeLayoutRef(settings, name));

    /// <summary>
    /// 从设置中读取「时间表引用[序号]」格式的时间点标识。
    /// </summary>
    public static (string Reference, int Index) TimeLayoutItem(JsonElement settings, string name = "TimeLayoutItem") =>
        ParseTimeLayoutItem(settings.GetProperty(name).GetString());

    /// <summary>
    /// 解析「时间表引用[序号]」格式的时间点标识。解析失败时返回 (空, 0)。
    /// </summary>
    public static (string Reference, int Index) ParseTimeLayoutItem(string? raw)
    {
        if (string.IsNullOrEmpty(raw))
            return (string.Empty, 0);
        var open = raw.LastIndexOf('[');
        if (open < 0 || !raw.EndsWith(']'))
            return (string.Empty, 0);
        var reference = raw[..open];
        var indexPart = raw[(open + 1)..^1];
        return (reference, int.TryParse(indexPart, out var index) ? index : 0);
    }

    /// <summary>
    /// 获取「时间表引用[序号]」所指向的时间点。序号越界或标识无效时返回 null。
    /// </summary>
    public static ProfileTimeLayoutItem? TimePoint(JsonElement settings, string name = "TimeLayoutItem")
    {
        var (reference, index) = TimeLayoutItem(settings, name);
        if (index < 1)
            return null;
        var layout = ResolveTimeLayout(reference);
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
