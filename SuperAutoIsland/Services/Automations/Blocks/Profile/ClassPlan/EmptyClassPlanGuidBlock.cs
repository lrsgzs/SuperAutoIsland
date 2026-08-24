using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Services.Automations;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

public class EmptyClassPlanGuidBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.emptyClassPlanGuid";
    public override string Name => "空课表";
    public override (string, string) Icon => ("文档", FluentIcons.DocumentDataRegular);
    public override string DataOutput => "SAI_Profile_ClassPlan";
    public override Task<object> Handler(object? data) => Task.FromResult<object>(Guid.Empty.ToString());
}
