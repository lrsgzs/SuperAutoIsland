using Avalonia.Media;
using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Models.Actions;
using SuperAutoIsland.Models.Data;

namespace SuperAutoIsland.Services.Automations.Blocks.DynamicText;

public class GetDynamicTextColorBlock : DataBlockBase
{
    public override string Id => "sai.data.getDynamicTextColor";
    public override string Name => "获取动态文本颜色";
    public override (string, string) Icon => ("文本颜色", FluentIcons.TextColorRegular);
    public override string Tooltip => "未设置颜色时返回默认颜色（白色）。";
    public override string DataOutput => "SAI_Color";
    public override Type SettingsType => typeof(GetDynamicTextSettings);

    public override void GetFields(FieldsRegister it)
    {
        it
            .AddField("Key", BasicFields.Text("ID"));
    }

    public override Task<object> Handler(object? data)
    {
        Color? color = null;

        if (data is GetDynamicTextSettings settings)
        {
            var provider = IAppHost.GetService<DynamicTextProvider>();
            color = provider.GetText(settings.Key)?.Color;
        }

        return Task.FromResult<object>(FormatColor(color ?? SetDynamicTextColorActionSettings.DefaultColor));
    }

    private static string FormatColor(Color color)
    {
        return color.A == byte.MaxValue
                   ? $"#{color.R:X2}{color.G:X2}{color.B:X2}"
                   : $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
    }
}