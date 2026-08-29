using System.Text.Json;
using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.TimeLayout;

/// <summary>
/// 获取时间点（「时间表 GUID[序号]」）的开始时间，格式 HH:mm:ss。
/// </summary>
public class GetTimePointStartTimeBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.timePointStartTime";
    public override string Name => "时间点";
    public override string DataOutput => "SAI_Time";
    public override bool InlineBlock => true;
    public override bool InlineField => true;

    public override void GetFields(FieldsRegister it) => it
        .AddField("TimeLayoutItem", ProfileFields.TimeLayoutItem(""))
        .AddDummy("的开始时间");

    public override Task<object> Handler(object? data)
    {
        var settings = JsonSerializer.SerializeToElement(data);
        var item = ProfileBlockHelpers.TimePoint(settings);
        return Task.FromResult<object>(item?.StartTime.ToString(@"hh\:mm\:ss") ?? "00:00:00");
    }
}
