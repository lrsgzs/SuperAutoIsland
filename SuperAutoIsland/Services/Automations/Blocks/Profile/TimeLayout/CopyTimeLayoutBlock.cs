using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using ClassIsland.Shared.Helpers;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.TimeLayout;

/// <summary>
/// 复制指定时间表，并输出新时间表的 GUID。
/// </summary>
public class CopyTimeLayoutBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.copyTimeLayout";
    public override string Name => "复制时间表";
    public override string Tooltip => "复制指定时间表，并输出新时间表的 GUID。复制出的时间表是普通时间表，不会带有临时层标记。";
    public override string DataOutput => "SAI_Profile_TimeLayout";

    public override void GetFields(FieldsRegister it) => it
        .AddField("TimeLayout", ProfileFields.TimeLayout(""));

    public override Task<object> Handler(object? data)
    {
        var settings = JsonSerializer.SerializeToElement(data);
        var profile = IAppHost.GetService<IProfileService>().Profile;
        var source = ProfileBlockHelpers.TimeLayout(settings);
        var copy = source is null ? null : ConfigureFileHelper.CopyObject(source);
        if (copy is null)
        {
            return Task.FromResult<object>(Guid.Empty.ToString());
        }

        copy.IsOverlay = false;
        copy.OverlaySourceId = null;
        var id = Guid.NewGuid();
        profile.TimeLayouts.Add(id, copy);
        return Task.FromResult<object>(id.ToString());
    }
}
