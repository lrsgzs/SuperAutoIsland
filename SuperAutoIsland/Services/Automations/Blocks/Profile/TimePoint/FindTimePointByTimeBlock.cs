using System.Text.Json;
using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.TimePoint;

/// <summary>
///     按时刻与类型查找时间点，输出「时间表 GUID[序号]」格式的时间点标识。
/// </summary>
public class FindTimePointByTimeBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.findTimePointByTime";
    public override string Name => "查找时间点";
    public override (string, string) Icon => ("查找", FluentIcons.SearchRegular);

    public override string Tooltip =>
        "查找时间区间包含指定时刻、类型匹配的第一个时间点，输出该时间点的标识。" +
        "比较精确到秒，毫秒被忽略；找不到时输出序号为 0 的标识。";

    public override string DataOutput => "SAI_Profile_TimeLayoutItem";

    public override void GetFields(FieldsRegister it)
    {
        it
            .AddField("TimeLayout", ProfileFields.TimeLayout(""))
            .AddField("TimeType", ProfileFields.TimePointTypeInput("类型"))
            .AddField("Time", BasicFields.Time("时刻", TimeSpan.FromHours(8)));
    }

    public override Task<object> Handler(object? data)
    {
        var settings = JsonSerializer.SerializeToElement(data);
        var reference = ProfileBlockHelpers.TimeLayoutRef(settings);
        var layout = ProfileBlockHelpers.TimeLayout(settings);
        if (layout is null)
            return Task.FromResult<object>($"{reference}[0]");

        var timeType = Math.Clamp(ProfileBlockHelpers.Number(settings, "TimeType"), 0, 3);
        var item = ProfileBlockHelpers.FindTimePoint(layout, ProfileBlockHelpers.Time(settings, "Time"), timeType);
        var index = item is null ? 0 : layout.Layouts.IndexOf(item) + 1;
        return Task.FromResult<object>($"{reference}[{index}]");
    }
}
