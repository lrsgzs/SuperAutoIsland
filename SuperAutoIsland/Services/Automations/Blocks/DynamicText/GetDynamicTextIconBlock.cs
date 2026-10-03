using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Models.Actions;
using SuperAutoIsland.Models.Data;

namespace SuperAutoIsland.Services.Automations.Blocks.DynamicText;

public class GetDynamicTextIconBlock : DataBlockBase
{
    public override string Id => "sai.data.getDynamicTextIcon";
    public override string Name => "获取动态文本图标";
    public override (string, string) Icon => ("图标", FluentIcons.IconsRegular);
    public override string Tooltip => $"未设置图标时返回默认图标 {SetDynamicTextIconActionSettings.DefaultIcon}。";
    public override string DataOutput => "SAI_Icon";
    public override Type SettingsType => typeof(GetDynamicTextSettings);

    public override void GetFields(FieldsRegister it)
    {
        it
            .AddField("Key", BasicFields.Text("ID"));
    }

    public override Task<object> Handler(object? data)
    {
        if (data is not GetDynamicTextSettings settings)
            return Task.FromResult<object>(SetDynamicTextIconActionSettings.DefaultIcon);

        var provider = IAppHost.GetService<DynamicTextProvider>();
        var iconExpression = provider.GetText(settings.Key)?.Icon;

        return Task.FromResult<object>(
            string.IsNullOrWhiteSpace(iconExpression)
                ? SetDynamicTextIconActionSettings.DefaultIcon
                : iconExpression);
    }
}