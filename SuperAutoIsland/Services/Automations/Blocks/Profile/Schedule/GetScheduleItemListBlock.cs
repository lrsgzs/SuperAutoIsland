using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Services.Automations;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Schedule;

/// <summary>
/// 获取档案中所有课程（日程项目）的 GUID 列表。
/// </summary>
public class GetScheduleItemListBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.scheduleItemList";
    public override string Name => "日程列表";
    public override (string, string) Icon => ("日程", FluentIcons.CalendarRegular);
    public override string Tooltip => "获取档案中所有课程（日程项目）的 GUID 列表（按档案中的顺序，不只是当天生效的课程）。";
    public override string DataOutput => "Array";

    public override Task<object> Handler(object? data)
    {
        var scheduleItems = IAppHost.GetService<IProfileService>().Profile.ScheduleItems;
        return Task.FromResult<object>(scheduleItems.Keys.Select(x => x.ToString()).ToList());
    }
}
