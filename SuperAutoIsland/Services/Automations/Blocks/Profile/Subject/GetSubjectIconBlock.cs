using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Subject;

/// <summary>
/// 获取科目的图标。返回图标表达式，如「lucide("\uE551")」或「img("图片路径")」。
/// </summary>
public class GetSubjectIconBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.subjectIcon";
    public override string Name => "科目图标";
    public override (string, string) Icon => ("图标", FluentIcons.IconsRegular);
    public override string Tooltip => "获取科目的图标表达式。科目不存在或未设置图标时返回空文本。";
    public override string DataOutput => "SAI_Icon";

    public override void GetFields(FieldsRegister it) => it
        .AddField("Subject", ProfileFields.Subject(""));

    public override Task<object> Handler(object? data)
    {
        var settings = JsonSerializer.SerializeToElement(data);
        var icon = IAppHost.GetService<IProfileService>().Profile.Subjects
            .GetValueOrDefault(ProfileBlockHelpers.Guid(settings, "Subject"))?.Icon;
        return Task.FromResult<object>(icon ?? string.Empty);
    }
}
