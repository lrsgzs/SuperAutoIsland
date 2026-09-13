using System.Text.Json;
using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.TimeLayout;

/// <summary>
/// 获取时间点（「时间表引用[序号]」）是第几节课（从 1 开始计数）。
/// 课间等非「上课」类型返回前一节课的序号；无效时返回 0。
/// </summary>
public class TimePointClassIndexBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.timePointClassIndex";
    public override string Name => "时间点";
    public override (string, string) Icon => ("钟表", FluentIcons.ClockRegular);
    public override string Tooltip => "获取时间点是第几节课（从 1 开始计数）。课间等非「上课」类型返回前一节课的序号；无效时返回 0。";
    public override string DataOutput => "Number";
    public override bool InlineBlock => true;
    public override bool InlineField => true;

    public override void GetFields(FieldsRegister it) => it
        .AddField("TimeLayoutItem", ProfileFields.TimeLayoutItem(""))
        .AddDummy("是第几节课?");

    public override Task<object> Handler(object? data)
    {
        var settings = JsonSerializer.SerializeToElement(data);
        var (reference, index) = ProfileBlockHelpers.TimeLayoutItem(settings);
        if (index < 1)
            return Task.FromResult<object>(0);

        var layout = ProfileBlockHelpers.ResolveTimeLayout(reference);
        if (layout is null || index > layout.Layouts.Count)
            return Task.FromResult<object>(0);

        return Task.FromResult<object>(layout.Layouts.Take(index).Count(x => x.TimeType == 0));
    }
}
