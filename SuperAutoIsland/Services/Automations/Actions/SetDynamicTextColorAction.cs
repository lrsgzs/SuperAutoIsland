using ClassIsland.Core.Abstractions.Automation;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using SuperAutoIsland.Models.Actions;

namespace SuperAutoIsland.Services.Automations.Actions;

[ActionInfo("sai.actions.setDynamicTextColor", "设置动态文本颜色", FluentIcons.TextColorRegular, false)]
public class SetDynamicTextColorAction : ActionBase<SetDynamicTextColorActionSettings>
{
    private DynamicTextProvider _provider = IAppHost.GetService<DynamicTextProvider>();

    protected override async Task OnInvoke()
    {
        await base.OnInvoke();
        _provider.SetColor(Settings.Key, !Settings.UseDefaultColor, Settings.Color);
    }

    protected override async Task OnRevert()
    {
        await base.OnRevert();

        _provider.SetItem(Settings.Key, _provider.GetTextOldValue(Settings.Key));
    }
}
