using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Schedule;

/// <summary>
///     获取当前正在进行中的课程（日程项目）的 GUID。
///     有多个课程重叠时取开始时间最晚的一个；当前没有课程时返回空 GUID。
/// </summary>
public class CurrentScheduleItemBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.currentScheduleItem";
    public override string Name => "当前课程";
    public override (string, string) Icon => ("日程", FluentIcons.CalendarRegular);
    public override string Tooltip => "获取当前正在进行中的课程的 GUID。有多个课程重叠时取开始时间最晚的一个；当前没有课程时返回空 GUID。";
    public override string DataOutput => "SAI_Profile_ScheduleItem";

    public override Task<object> Handler(object? data)
    {
        return Task.FromResult<object>(
            ProfileBlockHelpers.CurrentScheduleItem()?.Id.ToString() ?? Guid.Empty.ToString());
    }
}