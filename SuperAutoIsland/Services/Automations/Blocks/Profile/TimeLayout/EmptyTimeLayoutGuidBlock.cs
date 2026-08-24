using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Services.Automations;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.TimeLayout;

public class EmptyTimeLayoutGuidBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.emptyTimeLayoutGuid";
    public override string Name => "空时间表";
    public override (string, string) Icon => ("表格", FluentIcons.TableRegular);
    public override string DataOutput => "SAI_Profile_TimeLayout";
    public override Task<object> Handler(object? data) => Task.FromResult<object>(Guid.Empty.ToString());
}
