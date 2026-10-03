using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Schedule;

/// <summary>
///     获取当天生效的课程的 GUID 列表。
/// </summary>
public class GetTodayScheduleItemListBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.todayScheduleItemList";
    public override string Name => "当天课程列表";
    public override (string, string) Icon => ("日程", FluentIcons.CalendarRegular);
    public override string Tooltip => "获取当天会生效的课程的 GUID 列表（按档案中的顺序）。";
    public override string DataOutput => "Array";

    public override Task<object> Handler(object? data)
    {
        return Task.FromResult<object>(ProfileBlockHelpers.TodayScheduleItems().Keys
                                                          .Select(x => x.ToString())
                                                          .ToList());
    }
}