using System.Globalization;
using System.Text.Json;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Services.Automations.AppSettings;
using SuperAutoIsland.Services.Automations.Categories.AppSettings;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Services.Automations;

/// <summary>
///     「应用设置」积木注册。
///     <para>
///         积木的运行不走积木实例，而是通过前缀处理器直接处理：
///         反射读写是同步的，可以在 UI 线程上一次完成，无需 <c>InvokeActionSetAsync</c>。
///     </para>
/// </summary>
public class SaiAppSettingsRegistry
{
    private const string SetAction = "set";
    private const string GetAction = "get";
    private const string OptionAction = "option";

    private static readonly Logger<SaiAppSettingsRegistry> Logger = new();
    private static ISaiServer SaiServer { get; } = IAppHost.GetService<ISaiServer>();

    /// <summary>
    ///     注册应用设置分类与前缀处理器。
    /// </summary>
    public static void Register()
    {
        SaiServer.AddCategory<AppSettingsCategoryProvider>();
        SaiServer.AddPrefixHandler(AppSettingBlockFactory.IdPrefix, Handle);
    }

    /// <summary>
    ///     处理 <c>sai.appSettings.</c> 前缀的积木调用。
    /// </summary>
    /// <param name="kind">积木类型</param>
    /// <param name="id">积木 id</param>
    /// <param name="settings">积木设置</param>
    private static (bool Handled, object? Result) Handle(BlockKind kind, string id, JsonElement settings)
    {
        if (!id.StartsWith(AppSettingBlockFactory.IdPrefix, StringComparison.Ordinal))
        {
            return (false, null);
        }

        var rest = id[AppSettingBlockFactory.IdPrefix.Length..];
        var separator = rest.IndexOf('.');

        if (separator <= 0 || separator == rest.Length - 1)
        {
            return (false, null);
        }

        var action = rest[..separator];
        var propertyName = rest[(separator + 1)..];

        switch (action)
        {
            case SetAction when kind == BlockKind.Action:
                RunSet(propertyName, settings);
                return (true, null);

            case GetAction when kind == BlockKind.Data:
                return (true, RunGet(propertyName));

            case OptionAction when kind == BlockKind.Data:
                return (true, RunOption(propertyName, settings));

            default:
                Logger.Warn($"未知的应用设置积木调用：{kind} {id}");
                return (false, null);
        }
    }

    /// <summary>
    ///     修改设置项。
    /// </summary>
    private static void RunSet(string propertyName, JsonElement settings)
    {
        var descriptor = ClassIslandSettingsAccessor.Find(propertyName);
        if (descriptor == null)
        {
            Logger.Warn($"找不到应用设置项「{propertyName}」，已跳过。");
            return;
        }

        if (!TryGetField(settings, "Value", out var value))
        {
            Logger.Warn($"应用设置积木「{propertyName}」未提供值，已跳过。");
            return;
        }

        var (ok, error) = ClassIslandSettingsAccessor.SetValue(descriptor, value);
        if (!ok)
        {
            Logger.Warn($"修改应用设置「{descriptor.DisplayName}」失败：{error}");
        }
    }

    /// <summary>
    ///     读取设置项当前值。
    /// </summary>
    private static object RunGet(string propertyName)
    {
        var descriptor = ClassIslandSettingsAccessor.Find(propertyName);
        if (descriptor == null)
        {
            Logger.Warn($"找不到应用设置项「{propertyName}」。");
            return "???";
        }

        return ClassIslandSettingsAccessor.GetValue(descriptor) ?? "???";
    }

    /// <summary>
    ///     读取「选项参照」积木的下拉框内容。
    ///     <para>
    ///         枚举型输出选项下标（Number）；文本型选项（组件配置方案）输出选项文本（String）。
    ///     </para>
    /// </summary>
    /// <param name="propertyName">设置项属性名</param>
    /// <param name="settings">积木设置</param>
    private static object RunOption(string propertyName, JsonElement settings)
    {
        var descriptor = ClassIslandSettingsAccessor.Find(propertyName);

        if (descriptor?.OptionSource == AppSettingOptionSource.ComponentConfig)
        {
            return TryGetField(settings, "Value", out var text) && text.ValueKind == JsonValueKind.String
                       ? text.GetString() ?? string.Empty
                       : string.Empty;
        }

        if (!TryGetField(settings, "Value", out var value))
        {
            return 0d;
        }

        return value.ValueKind switch
        {
            JsonValueKind.Number => value.GetDouble(),
            JsonValueKind.String => double.TryParse(value.GetString(), NumberStyles.Any,
                                                    CultureInfo.InvariantCulture, out var parsed)
                                        ? parsed
                                        : 0d,
            _ => 0d
        };
    }

    private static bool TryGetField(JsonElement settings, string name, out JsonElement value)
    {
        value = default;

        return settings.ValueKind == JsonValueKind.Object && settings.TryGetProperty(name, out value);
    }
}