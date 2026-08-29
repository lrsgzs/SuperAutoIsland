using System.Text.Json;
using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.TimeLayout;

/// <summary>
/// 获取时间表中的第 N 节课（类型为「上课」的时间点），输出「时间表 GUID[序号]」格式的时间点标识。
/// 序号为该节课在时间表中的实际位置；不存在时序号为 0。
/// </summary>
public class GetClassPeriodItemBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.timeLayoutClassPeriod";
    public override string Name => "时间点";
    public override (string, string) Icon => ("钟表", FluentIcons.ClockRegular);
    public override string Tooltip => "获取时间表中的第 N 节课（类型为「上课」的时间点），输出「时间表 GUID[序号]」格式的时间点标识。";
    public override string DataOutput => "SAI_Profile_TimeLayoutItem";
    public override bool InlineBlock => true;
    public override bool InlineField => true;

    public override void GetFields(FieldsRegister it) => it
        .AddField("TimeLayout", ProfileFields.TimeLayout(""))
        .AddField("Index", BasicFields.Number("中的第", 1))
        .AddDummy("节课");

    public override Task<object> Handler(object? data)
    {
        var settings = JsonSerializer.SerializeToElement(data);
        var guid = ProfileBlockHelpers.Guid(settings, "TimeLayout");
        var position = ProfileBlockHelpers.ClassPeriodPosition(settings);
        return Task.FromResult<object>($"{guid}[{position}]");
    }
}
