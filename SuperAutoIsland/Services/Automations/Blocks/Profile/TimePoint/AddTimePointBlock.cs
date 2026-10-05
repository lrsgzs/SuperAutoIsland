using System.Text.Json;
using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;
using ProfileTimeLayoutItem = ClassIsland.Shared.Models.Profile.TimeLayoutItem;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.TimePoint;

/// <summary>
///     在时间表末尾追加一个时间点，并输出新时间点的标识。
/// </summary>
public class AddTimePointBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.addTimePoint";
    public override string Name => "添加时间点";
    public override (string, string) Icon => ("添加", FluentIcons.AddRegular);

    public override string Tooltip =>
        "在时间表末尾追加一个时间点，输出新时间点的标识。「上课」时间点会同步给所有使用该时间表的课表增加一节课程。" +
        "只能追加到末尾，以保证时间点的顺序与脚本的顺序一致。" +
        "开始时间早于末尾时间点的结束时间时会被钳制到其结束时间；分割线和行动的时长为 0，不受「时长」影响。" +
        "修改仅在内存中生效，需要自行调用「保存档案」积木。";

    public override string DataOutput => "SAI_Profile_TimeLayoutItem";

    public override void GetFields(FieldsRegister it)
    {
        it
            .AddField("TimeLayout", ProfileFields.TimeLayout(""))
            .AddField("TimeType", ProfileFields.TimePointTypeInput("类型"))
            .AddField("StartTime", BasicFields.Time("开始时间", TimeSpan.FromHours(8)))
            .AddField("Duration", BasicFields.Time("时长", TimeSpan.FromMinutes(40)));
    }

    public override Task<object> Handler(object? data)
    {
        var settings = JsonSerializer.SerializeToElement(data);
        var reference = ProfileBlockHelpers.TimeLayoutRef(settings);
        var layout = ProfileBlockHelpers.ClassicTimeLayout(settings);
        if (layout is null)
            return Task.FromResult<object>($"{reference}[0]");

        var timeType = Math.Clamp(ProfileBlockHelpers.Number(settings, "TimeType"), 0, 3);
        var start = ProfileBlockHelpers.Time(settings, "StartTime");
        var duration = ProfileBlockHelpers.Time(settings, "Duration");

        // 分割线和行动的结束时间恒等于开始时间，因此不能设置 EndTime。
        var item = new ProfileTimeLayoutItem
        {
            TimeType = timeType,
            StartTime = start
        };
        if (timeType is not (2 or 3))
            item.EndTime = start + (duration > TimeSpan.Zero ? duration : TimeSpan.Zero);

        var index = ProfileBlockHelpers.AppendTimePoint(layout, item);
        return Task.FromResult<object>($"{reference}[{index + 1}]");
    }
}
