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

    /// <summary>
    ///     当前档案的课程模式。
    /// </summary>
    public static ScheduleType CurrentScheduleType =>
        IAppHost.GetService<IProfileService>().Profile.ScheduleType;

    /// <summary>
    ///     当前档案是否处于日程模式。
    /// </summary>
    public static bool IsScheduleMode => CurrentScheduleType == ScheduleType.Schedule;

    public static Guid Guid(JsonElement settings, string name)
    {
        return System.Guid.TryParse(settings.GetProperty(name).GetString(), out var value) ? value : System.Guid.Empty;
    }

    public static DateOnly Date(JsonElement settings, string name)
    {
        return settings.GetProperty(name).Deserialize<DateOnly>();
    }

    public static int Number(JsonElement settings, string name)
    {
        return (int)settings.GetProperty(name).GetDouble();
    }

    public static bool Bool(JsonElement settings, string name)
    {
        var value = settings.GetProperty(name);
        return value.ValueKind switch
        {
            JsonValueKind.True   => true,
            JsonValueKind.False  => false,
            JsonValueKind.String => value.GetString() == "TRUE",
            _                    => false
        };
    }

    public static JsonElement Settings(object? value)
    {
        return JsonSerializer.SerializeToElement(value);
    }

    public static ProfileClassPlan? GetClassPlan(JsonElement settings, string name = "ClassPlan")
    {
        return ClassPlan(settings, name);
    }

    /// <summary>
    ///     读取课表引用原始字符串（GUID 或「[sched]yyyy-MM-dd」）。
    /// </summary>
    public static string ClassPlanRef(JsonElement settings, string name = "ClassPlan")
    {
        return settings.GetProperty(name).GetString() ?? string.Empty;
    }

    /// <summary>
    ///     判断引用是否为日程模式引用。
    /// </summary>
    public static bool IsScheduleRef(string? reference)
    {
        return reference?.StartsWith(ScheduleRefPrefix, StringComparison.Ordinal) == true;
    }

    /// <summary>
    ///     构造某一日期的日程模式引用。
    /// </summary>
    public static string CreateScheduleRef(DateOnly date)
    {
        return $"{ScheduleRefPrefix}{date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}";
    }

    /// <summary>
    ///     解析日程模式引用中的日期。
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
    ///     获取指定日期生效的课表引用。经典模式返回课表 GUID，日程模式返回「[sched]yyyy-MM-dd」引用。
    ///     当日没有课表时返回空 GUID。
    /// </summary>
    public static string ClassPlanRefByDate(DateTime date)
    {
        var plan = IAppHost.GetService<ILessonsService>().GetClassPlanByDate(date, out var id);
        if (IsScheduleMode)
            return plan is null ? System.Guid.Empty.ToString() : CreateScheduleRef(DateOnly.FromDateTime(date));

        return (id ?? System.Guid.Empty).ToString();
    }

    /// <summary>
    ///     解析课表引用。日程模式引用仅在档案处于日程模式时有效。
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

    public static ProfileClassPlan? ClassPlan(JsonElement settings, string name = "ClassPlan")
    {
        return ResolveClassPlan(ClassPlanRef(settings, name));
    }

    /// <summary>
    ///     仅解析经典模式课表（GUID）。用于写操作，避免误改日程模式生成的临时课表。
    /// </summary>
    public static ProfileClassPlan? ClassicClassPlan(JsonElement settings, string name = "ClassPlan")
    {
        var id = Guid(settings, name);
        return IAppHost.GetService<IProfileService>().Profile.ClassPlans.GetValueOrDefault(id);
    }

    /// <summary>
    ///     按 GUID 解析科目。用于写操作。
    /// </summary>
    public static ProfileSubject? Subject(JsonElement settings, string name = "Subject")
    {
        return IAppHost.GetService<IProfileService>().Profile.Subjects.GetValueOrDefault(Guid(settings, name));
    }

    /// <summary>
    ///     按 GUID 解析时间表。用于写操作，日程模式引用不会被解析成日程模式生成的时间表。
    /// </summary>
    public static ProfileTimeLayout? ClassicTimeLayout(JsonElement settings, string name = "TimeLayout")
    {
        return IAppHost.GetService<IProfileService>().Profile.TimeLayouts.GetValueOrDefault(Guid(settings, name));
    }

    /// <summary>
    ///     按 GUID 解析课表群。用于写操作。
    /// </summary>
    public static ProfileClassPlanGroup? ClassPlanGroup(JsonElement settings, string name = "ClassPlanGroup")
    {
        return IAppHost.GetService<IProfileService>().Profile.ClassPlanGroups.GetValueOrDefault(Guid(settings, name));
    }

    /// <summary>
    ///     读取时间设置（「HH:mm:ss」文本）。
    /// </summary>
    public static TimeSpan Time(JsonElement settings, string name)
    {
        var raw = settings.GetProperty(name).GetString();
        return TimeSpan.TryParse(raw, CultureInfo.InvariantCulture, out var value) ? value : TimeSpan.Zero;
    }

    /// <summary>
    ///     按 GUID 解析课程（日程项目）。用于写操作。
    /// </summary>
    public static ProfileScheduleItem? ScheduleItem(JsonElement settings, string name = "ScheduleItem")
    {
        return IAppHost.GetService<IProfileService>().Profile.ScheduleItems.GetValueOrDefault(Guid(settings, name));
    }

    /// <summary>
    ///     获取当天生效的课程（日程项目）。
    /// </summary>
    public static OrderedDictionary<Guid, ProfileScheduleItem> TodayScheduleItems()
    {
        return IAppHost.GetService<ILessonsService>().GetScheduleItemsByDate(
            DateOnly.FromDateTime(IAppHost.GetService<IExactTimeService>().GetCurrentLocalDateTime()));
    }

    /// <summary>
    ///     获取当前正在进行中的课程（日程项目）。没有正在进行中的课程时返回 null。
    ///     有多个课程重叠时取开始时间最晚的一个。
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
    ///     清理指向已被移除的课表群的引用（当前启用的课表群与临时课表群）。
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
    ///     读取时间表引用原始字符串（GUID 或「[sched]yyyy-MM-dd」）。
    /// </summary>
    public static string TimeLayoutRef(JsonElement settings, string name = "TimeLayout")
    {
        return settings.GetProperty(name).GetString() ?? string.Empty;
    }

    /// <summary>
    ///     解析时间表引用。日程模式引用取该日合成课表的时间表。
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

    public static ProfileTimeLayout? TimeLayout(JsonElement settings, string name = "TimeLayout")
    {
        return ResolveTimeLayout(TimeLayoutRef(settings, name));
    }

    /// <summary>
    ///     从设置中读取「时间表引用[序号]」格式的时间点标识。
    /// </summary>
    public static (string Reference, int Index) TimeLayoutItem(JsonElement settings, string name = "TimeLayoutItem")
    {
        return ParseTimeLayoutItem(settings.GetProperty(name).GetString());
    }

    /// <summary>
    ///     解析「时间表引用[序号]」格式的时间点标识。解析失败时返回 (空, 0)。
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
    ///     获取「时间表引用[序号]」所指向的时间点。序号越界或标识无效时返回 null。
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
    ///     获取时间表中第 N 节课（类型为「上课」的时间点）在时间表中的实际位置（从 1 开始计数）。
    ///     不存在时返回 0。
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

    public static string GuidOutput(Guid id)
    {
        return id.ToString();
    }

    #region 时间点写入

    /// <summary>
    ///     一天中最晚的时间。与宿主拖动调整时间时使用的上限一致。
    /// </summary>
    private static readonly TimeSpan MaxTime = new(23, 59, 59);

    /// <summary>
    ///     解析「时间表 GUID[序号]」所指向的时间点及其所在时间表，用于写入。
    ///     日程模式引用（「[sched]日期」）不会被解析，避免误改日程模式生成的临时时间表。
    /// </summary>
    public static (ProfileTimeLayout Layout, ProfileTimeLayoutItem Item)? WritableTimePoint(
        JsonElement settings, string name = "TimeLayoutItem")
    {
        var (reference, index) = TimeLayoutItem(settings, name);
        if (index < 1 || !System.Guid.TryParse(reference, out var layoutId))
            return null;

        var layout = IAppHost.GetService<IProfileService>().Profile.TimeLayouts.GetValueOrDefault(layoutId);
        var item = layout?.Layouts.ElementAtOrDefault(index - 1);
        return item is null ? null : (layout!, item);
    }

    /// <summary>
    ///     在时间表末尾追加一个时间点，并返回新时间点的序号（从 0 开始）。
    ///     <para>
    ///         只能追加到末尾，从而保证时间点的顺序与脚本的顺序一致，也避免中间插入导致的序号漂移。
    ///         开始时间早于末尾时间点的结束时间时会被钳制到该结束时间；分割线与行动的时长为 0。
    ///     </para>
    /// </summary>
    public static int AppendTimePoint(ProfileTimeLayout layout, ProfileTimeLayoutItem item)
    {
        var last = layout.Layouts.Count > 0 ? layout.Layouts[^1] : null;
        var start = ClampTime(item.StartTime, last?.EndTime ?? TimeSpan.Zero, MaxTime);

        item.StartTime = start;
        if (item.TimeType is not (2 or 3))
            item.EndTime = ClampTime(item.EndTime, start, MaxTime);

        // 必须通过 InsertTimePoint 增删时间点，宿主据此同步课表中的课程列表。
        layout.InsertTimePoint(layout.Layouts.Count, item);
        return layout.Layouts.Count - 1;
    }

    /// <summary>
    ///     删除时间表的最后一个时间点。时间表为空时不做修改。
    /// </summary>
    public static bool RemoveLastTimePoint(ProfileTimeLayout layout)
    {
        if (layout.Layouts.Count == 0)
            return false;

        layout.RemoveTimePoint(layout.Layouts[^1]);
        return true;
    }

    /// <summary>
    ///     时间点在时间表中允许的时间范围。
    ///     上下界取相邻的「上课/课间」时间点（分割线不参与），与宿主的拖动钳制行为一致，
    ///     因此调整时间不会改变时间点之间的相对顺序。
    /// </summary>
    public static (TimeSpan Min, TimeSpan Max) AllowedRange(ProfileTimeLayout layout, ProfileTimeLayoutItem item)
    {
        var index = layout.Layouts.IndexOf(item);
        if (index < 0)
            return (TimeSpan.Zero, MaxTime);

        TimeSpan? min = null;
        TimeSpan? max = null;

        for (var i = index - 1; i >= 0; i--)
        {
            if (layout.Layouts[i].TimeType is 0 or 1)
            {
                min = layout.Layouts[i].EndTime;
                break;
            }
        }

        for (var i = index + 1; i < layout.Layouts.Count; i++)
        {
            if (layout.Layouts[i].TimeType is 0 or 1)
            {
                max = layout.Layouts[i].StartTime;
                break;
            }
        }

        return (min ?? TimeSpan.Zero, max ?? MaxTime);
    }

    /// <summary>
    ///     设置时间点的开始时间。
    ///     <para>
    ///         为不丢失时间点本身的信息，会尽量保持时长整体平移；放不下时向内钳制，
    ///         不会让开始时间越过结束时间，也不会与相邻时间点重叠。
    ///         分割线与行动没有时长，开始时间同时也是结束时间。
    ///     </para>
    /// </summary>
    public static void SetTimePointStartTime(ProfileTimeLayout layout, ProfileTimeLayoutItem item, TimeSpan value)
    {
        var (min, max) = AllowedRange(layout, item);
        var start = ClampTime(value, min, max);

        if (item.TimeType is 2 or 3)
        {
            item.StartTime = start;
            return;
        }

        var duration = item.EndTime - item.StartTime;
        if (duration < TimeSpan.Zero)
            duration = TimeSpan.Zero;

        var end = start + duration;
        if (end > max)
        {
            end = max;
            start = end - duration;
        }

        if (start < min)
        {
            start = min;
            end = ClampTime(start + duration, min, max);
        }

        item.StartTime = start;
        item.EndTime = end;
    }

    /// <summary>
    ///     设置时间点的结束时间，并钳制到与相邻时间点不重叠的范围。分割线与行动没有时长，不做修改。
    /// </summary>
    public static void SetTimePointEndTime(ProfileTimeLayout layout, ProfileTimeLayoutItem item, TimeSpan value)
    {
        if (item.TimeType is 2 or 3)
            return;

        var (min, max) = AllowedRange(layout, item);
        if (min < item.StartTime)
            min = item.StartTime;

        item.EndTime = ClampTime(value, min, max);
    }

    /// <summary>
    ///     查找时间区间包含指定时刻、类型匹配的第一个时间点。
    ///     比较精确到秒，毫秒被忽略；找不到时返回 null。
    /// </summary>
    public static ProfileTimeLayoutItem? FindTimePoint(ProfileTimeLayout layout, TimeSpan time, int? timeType)
    {
        time = TruncateToSecond(time);

        foreach (var item in layout.Layouts)
        {
            if (timeType is not null && item.TimeType != timeType)
                continue;

            // 分割线与行动的区间长度为 0，此时等价于「开始时间等于该时刻」。
            if (TruncateToSecond(item.StartTime) <= time && TruncateToSecond(item.EndTime) >= time)
                return item;
        }

        return null;
    }

    /// <summary>
    ///     截断到秒，忽略毫秒。
    /// </summary>
    public static TimeSpan TruncateToSecond(TimeSpan value)
    {
        return TimeSpan.FromSeconds(Math.Floor(value.TotalSeconds));
    }

    /// <summary>
    ///     把时间限制在指定范围内。范围无效时取下界。
    /// </summary>
    private static TimeSpan ClampTime(TimeSpan value, TimeSpan min, TimeSpan max)
    {
        if (max < min)
            max = min;

        return value < min ? min : value > max ? max : value;
    }

    #endregion
}