using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Services.Automations;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Subject;

/// <summary>
/// 获取档案中所有科目的 GUID 列表。
/// </summary>
public class GetSubjectListBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.subjectList";
    public override string Name => "科目列表";
    public override (string, string) Icon => ("书", FluentIcons.BookRegular);
    public override string Tooltip => "获取档案中所有科目的 GUID 列表（按档案中的顺序）。";
    public override string DataOutput => "Array";

    public override Task<object> Handler(object? data)
    {
        var subjects = IAppHost.GetService<IProfileService>().Profile.Subjects;
        return Task.FromResult<object>(subjects.Keys.Select(x => x.ToString()).ToList());
    }
}
