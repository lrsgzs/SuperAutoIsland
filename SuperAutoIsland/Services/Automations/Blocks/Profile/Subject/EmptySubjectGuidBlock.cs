using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Services.Automations;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Subject;

public class EmptySubjectGuidBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.emptySubjectGuid";
    public override string Name => "空科目";
    public override (string, string) Icon => ("书", FluentIcons.BookRegular);
    public override string DataOutput => "SAI_Profile_Subject";
    public override Task<object> Handler(object? data) => Task.FromResult<object>(Guid.Empty.ToString());
}
