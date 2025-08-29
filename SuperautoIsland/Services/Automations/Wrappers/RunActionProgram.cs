using System.Text.Json;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Models.Actions;

namespace SuperAutoIsland.Services.Automations.Wrappers;

/// <summary>
/// 「运行应用程序」包装器
/// </summary>
public static class RunActionProgram
{
    /// <summary>
    /// 包装器本体
    /// </summary>
    /// <param name="actionItem">行动项目</param>
    /// <returns>修改后的行动项目</returns>
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