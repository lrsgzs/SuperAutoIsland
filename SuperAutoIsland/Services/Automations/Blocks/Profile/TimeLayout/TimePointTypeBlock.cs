using System.Text.Json;
using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.TimeLayout;

/// <summary>
/// 时间点类型。下拉框选择，输出类型编号（0-上课，1-课间，2-分割线，3-行动）。
/// </summary>
public class TimePointTypeBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.timePointType";
    public override string Name => "时间点类型";
    public override (string, string) Icon => ("时钟", FluentIcons.ClockRegular);
    public override string DataOutput => "SAI_Profile_TimeLayoutItemType";
    public override bool InlineBlock => true;
    public override bool InlineField => true;

    public override void GetFields(FieldsRegister it) => it
        .AddField("Type", ProfileFields.TimePointType(""));

    public override Task<object> Handler(object? data)
    {
        var settings = JsonSerializer.SerializeToElement(data);
        var type = (int)settings.GetProperty("Type").GetDouble();
        return Task.FromResult<object>(type);
    }
}
