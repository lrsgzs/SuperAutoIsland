using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using ClassIsland.Shared.Helpers;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

public class CopyClassPlanBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.copyClassPlan";
    public override string Name => "复制课表";
    public override string DataOutput => "SAI_Profile_ClassPlan";
    public override Type SettingsType => typeof(object);
    public override void GetFields(FieldsRegister it) => it.AddField("ClassPlan", ProfileFields.ClassPlan(""));
    public override Task<object> Handler(object? data)
    {
        var settings = JsonSerializer.SerializeToElement(data);
        var id = ProfileBlockHelpers.Guid(settings, "ClassPlan");
        var profile = IAppHost.GetService<IProfileService>().Profile;
        if (!profile.ClassPlans.TryGetValue(id, out var source)) return Task.FromResult<object>(Guid.Empty.ToString());
        var copy = ConfigureFileHelper.CopyObject(source);
        copy.IsOverlay = false;
        copy.OverlaySourceId = null;
        var newId = Guid.NewGuid();
        profile.ClassPlans.Add(newId, copy);
        return Task.FromResult<object>(newId.ToString());
    }
}
