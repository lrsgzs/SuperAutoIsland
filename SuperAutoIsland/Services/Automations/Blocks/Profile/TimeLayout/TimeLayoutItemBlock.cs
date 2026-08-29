using System.Text.Json;
using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.TimeLayout;

/// <summary>
/// 获取时间表中的第 N 个时间点，输出「时间表 GUID[序号]」格式的时间点标识。
/// </summary>
public class TimeLayoutItemBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.timeLayoutItem";
    public override string Name => "时间点";
    public override (string, string) Icon => ("钟表", FluentIcons.ClockRegular);
    public override string Tooltip => "获取时间表中的第 N 个时间点，输出「时间表 GUID[序号]」格式的时间点标识。";
    public override string DataOutput => "SAI_Profile_TimeLayoutItem";
    public override bool InlineBlock => true;
    public override bool InlineField => true;

    public override void GetFields(FieldsRegister it) => it
        .AddField("TimeLayout", ProfileFields.TimeLayout(""))
        .AddField("Index", BasicFields.Number("中的第", 1))
        .AddDummy("个时间点");

    public override Task<object> Handler(object? data)
    {
        var settings = JsonSerializer.SerializeToElement(data);
        var guid = ProfileBlockHelpers.Guid(settings, "TimeLayout");
        var index = Math.Max(1, (int)settings.GetProperty("Index").GetDouble());
        return Task.FromResult<object>($"{guid}[{index}]");
    }
}
