using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Services.Automations;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlanGroup;

public class EmptyClassPlanGroupGuidBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.emptyClassPlanGroupGuid";
    public override string Name => "空课表群";
    public override (string, string) Icon => ("群", FluentIcons.GroupRegular);
    public override string DataOutput => "SAI_Profile_ClassPlanGroup";
    public override Task<object> Handler(object? data) => Task.FromResult<object>(Guid.Empty.ToString());
}
