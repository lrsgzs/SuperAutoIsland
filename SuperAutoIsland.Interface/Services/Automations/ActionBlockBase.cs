using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Interface.Metadata;

namespace SuperAutoIsland.Interface.Services.Automations;

public abstract class ActionBlockBase : BlockBase
{
    public override BlockKind Kind => BlockKind.Action;
    
    public virtual ActionItem Wrapper(ActionItem actionItem)
    {
        return actionItem;
    }
    
    /// <summary>
    /// 会在 ui 线程运行，无需 Dispatcher
    /// </summary>
    /// <param name="actionItem">行动项</param>
    public virtual async Task Handler(ActionItem actionItem)
    {
        var actionService = IAppHost.GetService<IActionService>();
        await actionService.InvokeActionSetAsync(new ActionSet
        {
            Name = "SAI 临时行动组",
            ActionItems = [actionItem]
        });
    }
}