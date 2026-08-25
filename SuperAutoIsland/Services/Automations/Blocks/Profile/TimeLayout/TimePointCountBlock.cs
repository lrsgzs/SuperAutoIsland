using System.Text.Json;
using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.TimeLayout;

/// <summary>
/// 获取时间表的时间点总数。
/// </summary>
public class TimePointCountBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.timePointCount";
    public override string Name => "时间点总数";
    public override string DataOutput => "Number";
    
    public override void GetFields(FieldsRegister it) => it
        .AddField("TimeLayout", ProfileFields.TimeLayout(""));

    public override Task<object> Handler(object? data)
    {
        var settings = JsonSerializer.SerializeToElement(data);
        var layout = ProfileBlockHelpers.TimeLayout(settings);
        return Task.FromResult<object>(layout?.Layouts.Count ?? 0);
    }
}
