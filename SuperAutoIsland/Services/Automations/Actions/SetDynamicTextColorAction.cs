using ClassIsland.Core.Abstractions.Automation;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using SuperAutoIsland.Models.Actions;

namespace SuperAutoIsland.Services.Automations.Actions;

[ActionInfo("sai.actions.setDynamicTextColor", "设置动态文本颜色", FluentIcons.TextColorRegular, false)]
public class SetDynamicTextColorAction : ActionBase<SetDynamicTextColorActionSettings>
{
    private readonly DynamicTextProvider _provider = IAppHost.GetService<DynamicTextProvider>();

    protected override async Task OnInvoke()
    {
        await base.OnInvoke();
        _provider.SetColor(Settings.Key, Settings.UseDefaultColor ? null : Settings.Color);
    }

    protected override async Task OnRevert()
    {
        await base.OnRevert();
        _provider.RevertColor(Settings.Key);
    }
}