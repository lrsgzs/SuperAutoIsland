using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Services.Automations;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

public class EmptyGuidBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.emptyGuid";
    public override string Name => "空GUID";
    public override (string, string) Icon => ("星星", FluentIcons.StarRegular);

    public override Task<object> Handler(object? data) =>
        Task.FromResult<object>(Guid.Empty.ToString());
}
