using System.Text.Json;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

public class SetClassPlanSubjectBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.setClassPlanSubject";
    public override string Name => "课表";
    public override bool InlineBlock => true;
    public override bool InlineField => true;
    
    public override void GetFields(FieldsRegister it) => it
        .AddField("ClassPlan", ProfileFields.ClassPlan("设置"))
        .AddField("Index", BasicFields.Number("第", 1))
        .AddField("Subject", ProfileFields.Subject("节课的科目为"));
    
    public override Task Handler(ActionItem actionItem)
    {
        var s = JsonSerializer.SerializeToElement(actionItem.Settings);
        var plan = ProfileBlockHelpers.ClassPlan(s);
        var index = ProfileBlockHelpers.Number(s, "Index") - 1;
        if (plan != null && index >= 0 && index < plan.Classes.Count)
            plan.Classes[index].SubjectId = ProfileBlockHelpers.Guid(s, "Subject");
        return Task.CompletedTask;
    }
}
