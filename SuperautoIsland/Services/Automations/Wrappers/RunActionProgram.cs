using System.Text.Json;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Models.Actions;

namespace SuperAutoIsland.Services.Automations.Wrappers;

public static class RunActionProgram
{
    public static ActionItem Wrapper(ActionItem actionItem)
    {
        var settingsJson = JsonSerializer.Serialize(actionItem.Settings);
        var settings = JsonSerializer.Deserialize<RunActionSettings>(settingsJson)!;
        settings.RunType = RunActionSettings.RunActionRunType.Application;
        settingsJson = JsonSerializer.Serialize(settings);
        
        return new ActionItem
        {
            Id = "classisland.os.run",
            Settings = JsonSerializer.Deserialize<object>(settingsJson)
        };
    }
}