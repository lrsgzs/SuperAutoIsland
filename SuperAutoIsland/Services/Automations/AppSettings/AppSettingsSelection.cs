using SuperAutoIsland.Models.Settings;
using SuperAutoIsland.Shared;

namespace SuperAutoIsland.Services.Automations.AppSettings;

/// <summary>
///     应用设置积木的展示选择。
/// </summary>
public static class AppSettingsSelection
{
    /// <summary>
    ///     按当前配置解析需要展示的设置项属性名。
    /// </summary>
    public static HashSet<string> Resolve()
    {
        return Resolve(GlobalConstants.Configs.MainConfig?.Data.AppSettingsBlocks);
    }

    /// <summary>
    ///     解析需要展示的设置项属性名。
    ///     <para>
    ///         配置中的 <c>Selected</c> 为 null 表示用户尚未自定义，使用默认选择
    ///         （全部标注了 <c>SettingsInfo</c> 的设置项）；为空列表表示用户主动全不选。
    ///     </para>
    /// </summary>
    /// <param name="model">应用设置积木配置</param>
    public static HashSet<string> Resolve(AppSettingsBlocksModel? model)
    {
        return model?.Selected == null
                   ? [.. ClassIslandSettingsAccessor.DefaultSelection]
                   : [.. model.Selected];
    }
}