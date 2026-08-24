using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Models.Data;

namespace SuperAutoIsland.Services.Automations.Blocks;

public class GetDynamicTextBlock : DataBlockBase
{
    public override string Id => "sai.data.getDynamicText";
    public override string Name => "获取动态文本";
    public override (string, string) Icon => ("文本", FluentIcons.TextboxRegular);
    public override Type SettingsType => typeof(GetDynamicTextSettings);

    public override void GetFields(FieldsRegister it) => it
        .AddField("Key", BasicFields.Text("ID"));

    public override async Task<object> Handler(object? data)
    {
        if (data is not GetDynamicTextSettings settings)
            return Task.FromResult("???");
        
        var provider = IAppHost.GetService<DynamicTextProvider>();
        return Task.FromResult(provider.GetText(settings.Key) ?? "[未设置值]");
    }
}