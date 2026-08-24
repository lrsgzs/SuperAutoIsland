using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

public class EnableTempClassPlanBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.enableTempClassPlan";
    public override string Name => "启用临时课表";
    public override void GetFields(FieldsRegister it) => it
        .AddField("ClassPlan", ProfileFields.ClassPlan(""))
        .AddField("Date", BasicFields.Date("启用日期"));
    public override Task Handler(ActionItem actionItem)
    {
        var settings = JsonSerializer.SerializeToElement(actionItem.Settings);
        var profile = IAppHost.GetService<IProfileService>().Profile;
        profile.TempClassPlanId = ProfileBlockHelpers.Guid(settings, "ClassPlan");
        profile.TempClassPlanSetupTime = ProfileBlockHelpers.Date(settings, "Date").ToDateTime(TimeOnly.MinValue);
        return Task.CompletedTask;
    }
}
