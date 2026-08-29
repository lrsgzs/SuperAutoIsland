using System.Text.Json;
using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.TimeLayout;

/// <summary>
/// 获取时间点（「时间表 GUID[序号]」）的课间名称。仅课间类型的时间点返回名称（未自定义时返回「课间休息」），其余类型返回空字符串。
/// </summary>
public class GetTimePointBreakNameBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.timePointBreakName";
    public override string Name => "时间点";
    public override string DataOutput => "String";
    public override bool InlineBlock => true;
    public override bool InlineField => true;

    public override void GetFields(FieldsRegister it) => it
        .AddField("TimeLayoutItem", ProfileFields.TimeLayoutItem(""))
        .AddDummy("的课间名称");

    public override Task<object> Handler(object? data)
    {
        var settings = JsonSerializer.SerializeToElement(data);
        var item = ProfileBlockHelpers.TimePoint(settings);
        return Task.FromResult<object>(item is { TimeType: 1 } ? item.BreakNameText : "上课时间");
    }
}
