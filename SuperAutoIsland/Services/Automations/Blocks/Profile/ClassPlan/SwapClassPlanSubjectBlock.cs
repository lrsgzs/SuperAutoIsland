using System.Text.Json;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

public class SwapClassPlanSubjectBlock : ActionBlockBase
{
    public override string Id => "sai.profile.actions.swapClassPlanSubject";
    public override string Name => "交换课程";
    public override bool InlineBlock => true;
    public override bool InlineField => true;
    
    public override void GetFields(FieldsRegister it) => it
        .AddField("ClassPlan", ProfileFields.ClassPlan(""))
        .AddField("FirstIndex", BasicFields.Number("的第", 1))
        .AddField("SecondIndex", BasicFields.Number("节课和第", 2))
        .AddDummy("节课交换");
    
    public override Task Handler(ActionItem actionItem)
    {
        var s = JsonSerializer.SerializeToElement(actionItem.Settings);
        var plan = ProfileBlockHelpers.ClassPlan(s);
        var first = ProfileBlockHelpers.Number(s, "FirstIndex") - 1;
        var second = ProfileBlockHelpers.Number(s, "SecondIndex") - 1;
        if (plan != null && first >= 0 && second >= 0 && first < plan.Classes.Count && second < plan.Classes.Count)
            (plan.Classes[first].SubjectId, plan.Classes[second].SubjectId) = (plan.Classes[second].SubjectId, plan.Classes[first].SubjectId);
        return Task.CompletedTask;
    }
}
