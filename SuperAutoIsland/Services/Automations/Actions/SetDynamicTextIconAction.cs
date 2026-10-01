using ClassIsland.Core.Abstractions.Automation;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using SuperAutoIsland.Models.Actions;

namespace SuperAutoIsland.Services.Automations.Actions;

[ActionInfo("sai.actions.setDynamicTextIcon", "设置动态文本图标", FluentIcons.IconsRegular, false)]
public class SetDynamicTextIconAction : ActionBase<SetDynamicTextIconActionSettings>
{
    private DynamicTextProvider _provider = IAppHost.GetService<DynamicTextProvider>();

    protected override async Task OnInvoke()
    {
        await base.OnInvoke();
        _provider.SetIcon(Settings.Key, Settings.IncludeIcon ? Settings.Icon : null);
    }

    protected override async Task OnRevert()
    {
        await base.OnRevert();
        _provider.RevertIcon(Settings.Key);
    }
}
