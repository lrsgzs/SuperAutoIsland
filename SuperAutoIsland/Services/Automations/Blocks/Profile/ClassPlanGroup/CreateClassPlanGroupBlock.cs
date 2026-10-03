using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using ProfileClassPlanGroup = ClassIsland.Shared.Models.Profile.ClassPlanGroup;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlanGroup;

/// <summary>
/// 创建一个新课表群，并输出新课表群的 GUID。
/// </summary>
public class CreateClassPlanGroupBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.createClassPlanGroup";
    public override string Name => "创建课表群";
    public override string Tooltip => "创建一个新课表群，并输出新课表群的 GUID。";
    public override string DataOutput => "SAI_Profile_ClassPlanGroup";

    public override void GetFields(FieldsRegister it) => it
        .AddField("Name", BasicFields.Text("名称", "新课表群"));

    public override Task<object> Handler(object? data)
    {
        var settings = JsonSerializer.SerializeToElement(data);
        var profile = IAppHost.GetService<IProfileService>().Profile;
        var group = new ProfileClassPlanGroup
        {
            Name = settings.GetProperty("Name").GetString() ?? string.Empty
        };
        var id = Guid.NewGuid();
        profile.ClassPlanGroups.Add(id, group);
        return Task.FromResult<object>(id.ToString());
    }
}
