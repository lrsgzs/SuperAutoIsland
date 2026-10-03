using System.Text.Json;
using Avalonia.Media;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Subject;

/// <summary>
///     获取科目的主题色。
/// </summary>
public class GetSubjectColorBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.subjectColor";
    public override string Name => "科目主题色";
    public override (string, string) Icon => ("颜色", FluentIcons.ColorRegular);
    public override string Tooltip => "获取科目的主题色。不透明颜色返回「#RRGGBB」，半透明颜色返回「#AARRGGBB」。科目不存在或颜色无效时返回空文本。";
    public override string DataOutput => "SAI_Color";

    public override void GetFields(FieldsRegister it)
    {
        it
            .AddField("Subject", ProfileFields.Subject(""));
    }

    public override Task<object> Handler(object? data)
    {
        var settings = JsonSerializer.SerializeToElement(data);
        var colorHex = IAppHost.GetService<IProfileService>().Profile.Subjects
                               .GetValueOrDefault(ProfileBlockHelpers.Guid(settings, "Subject"))?.ColorHex;
        if (string.IsNullOrWhiteSpace(colorHex) || !Color.TryParse(colorHex, out var color))
            return Task.FromResult<object>(string.Empty);

        return Task.FromResult<object>(FormatColor(color));
    }

    private static string FormatColor(Color color)
    {
        return color.A == byte.MaxValue
                   ? $"#{color.R:X2}{color.G:X2}{color.B:X2}"
                   : $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
    }
}