using System.Text.Json;
using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.TimeLayout;

/// <summary>
/// 获取时间表第 N 个时间点的类型（0-上课，1-课间，2-分割线，3-行动）。
/// </summary>
public class GetTimePointItemTypeBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.timePointItemType";
    public override string Name => "时间点";
    public override string DataOutput => "SAI_Profile_TimeLayoutItemType";
    public override bool InlineBlock => true;
    public override bool InlineField => true;

    public override void GetFields(FieldsRegister it) => it
        .AddField("TimeLayout", ProfileFields.TimeLayout(""))
        .AddField("Index", BasicFields.Number("第", 1))
        .AddDummy("个时间点的类型");

    public override Task<object> Handler(object? data)
    {
        var settings = JsonSerializer.SerializeToElement(data);
        return Task.FromResult<object>(ProfileBlockHelpers.TimePoint(settings)?.TimeType ?? 0);
    }
}
