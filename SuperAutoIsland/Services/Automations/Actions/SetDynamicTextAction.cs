using ClassIsland.Core.Abstractions.Automation;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using SuperAutoIsland.Models.Actions;

namespace SuperAutoIsland.Services.Automations.Actions;

[ActionInfo("sai.actions.setDynamicText", "设置动态文本", FluentIcons.TextEditStyleRegular, false)]
public class SetDynamicTextAction : ActionBase<SetDynamicTextActionSettings>
{
    private DynamicTextProvider _provider = IAppHost.GetService<DynamicTextProvider>();
    
    protected override async Task OnInvoke()
    {
        await base.OnInvoke();
        _provider.SetText(Settings.Key, Settings.Value);
    }

    protected override async Task OnRevert()
    {
        await base.OnRevert();

        var oldValue = _provider.GetTextOldValue(Settings.Key);
        if (oldValue != null)
        {
            _provider.SetText(Settings.Key, oldValue);
        }
        else
        {
            _provider.RemoveText(Settings.Key);
        }
    }
}