using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using ProfileTimeLayout = ClassIsland.Shared.Models.Profile.TimeLayout;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.TimeLayout;

/// <summary>
/// 创建一个新时间表，并输出新时间表的 GUID。
/// </summary>
public class CreateTimeLayoutBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.createTimeLayout";
    public override string Name => "创建时间表";
    public override string Tooltip => "创建一个空白的新时间表，并输出新时间表的 GUID。";
    public override string DataOutput => "SAI_Profile_TimeLayout";

    public override void GetFields(FieldsRegister it) => it
        .AddField("Name", BasicFields.Text("名称", "新时间表"));

    public override Task<object> Handler(object? data)
    {
        var settings = JsonSerializer.SerializeToElement(data);
        var profile = IAppHost.GetService<IProfileService>().Profile;
        var timeLayout = new ProfileTimeLayout
        {
            Name = settings.GetProperty("Name").GetString() ?? string.Empty
        };
        var id = Guid.NewGuid();
        profile.TimeLayouts.Add(id, timeLayout);
        return Task.FromResult<object>(id.ToString());
    }
}
