using System.Reflection;

namespace SuperAutoIsland.Services.Automations.AppSettings;

/// <summary>
///     应用设置项的值类型分级，决定生成的积木字段与输出类型。
/// </summary>
public enum AppSettingValueKind
{
    /// <summary>
    ///     不支持：集合、字典等无法用单个积木表达的复杂结构。
    /// </summary>
    None,

    /// <summary>
    ///     布尔值。
    /// </summary>
    Boolean,

    /// <summary>
    ///     数值（int / double）。
    /// </summary>
    Number,

    /// <summary>
    ///     文本。
    /// </summary>
    Text,

    /// <summary>
    ///     颜色。
    /// </summary>
    Color,

    /// <summary>
    ///     下拉框选项下标（int + SettingsInfo.Enums，或 C# 枚举）。
    /// </summary>
    EnumIndex,

    /// <summary>
    ///     以 JSON 文本读写。
    /// </summary>
    Json
}

/// <summary>
///     设置项「选项参照」积木的选项来源。
/// </summary>
public enum AppSettingOptionSource
{
    /// <summary>
    ///     没有可列举的选项。
    /// </summary>
    None,

    /// <summary>
    ///     枚举型：选项来自 <c>SettingsInfo.Enums</c> 或 C# 枚举，参照积木输出选项下标（Number）。
    /// </summary>
    EnumIndex,

    /// <summary>
    ///     文本型选项：选项来自 ClassIsland 的组件配置方案列表，参照积木输出选项文本（String）。
    /// </summary>
    ComponentConfig
}

/// <summary>
///     单个 ClassIsland 应用设置项的描述信息。
/// </summary>
public class AppSettingDescriptor
{
    /// <summary>
    ///     对应的设置属性。
    /// </summary>
    public required PropertyInfo Property { get; init; }

    /// <summary>
    ///     属性名，同时作为积木 id 的一部分。
    /// </summary>
    public required string PropertyName { get; init; }

    /// <summary>
    ///     展示名称（有 <c>SettingsInfo</c> 时为其中文名，否则为属性名）。
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    ///     图标字形。
    /// </summary>
    public required string Glyph { get; init; }

    /// <summary>
    ///     排序序号，数字越大越靠后。
    /// </summary>
    public required double Order { get; init; }

    /// <summary>
    ///     去除可空包装后的属性类型。
    /// </summary>
    public required Type ValueType { get; init; }

    /// <summary>
    ///     值类型分级。
    /// </summary>
    public required AppSettingValueKind Kind { get; init; }

    /// <summary>
    ///     是否可写入。
    /// </summary>
    public required bool CanSet { get; init; }

    /// <summary>
    ///     是否被 ClassIsland 标记为废弃。
    /// </summary>
    public required bool IsObsolete { get; init; }

    /// <summary>
    ///     是否有明确的设置项信息（标注了 <c>SettingsInfo</c>）。
    /// </summary>
    public required bool IsSettingsInfoAttributed { get; init; }

    /// <summary>
    ///     下拉框选项名称。仅 <see cref="AppSettingValueKind.EnumIndex" /> 时可能非空。
    /// </summary>
    public string[]? EnumNames { get; init; }

    /// <summary>
    ///     「选项参照」积木的选项来源。非 <see cref="AppSettingOptionSource.None" /> 时会生成独立的选项参照积木。
    /// </summary>
    public AppSettingOptionSource OptionSource { get; init; } = AppSettingOptionSource.None;

    /// <summary>
    ///     附加说明（JSON 传递、废弃、仅可获取等），用于积木提示与设置页展示。
    /// </summary>
    public string Note { get; init; } = string.Empty;

    /// <summary>
    ///     是否可以生成积木。
    /// </summary>
    public bool IsSupported => Kind != AppSettingValueKind.None;

    /// <summary>
    ///     是否可以在设置页中被勾选。不支持的类型不可勾选。
    /// </summary>
    public bool CanSelect => IsSupported;

    /// <summary>
    ///     是否可以生成「选项参照」积木。
    /// </summary>
    public bool HasOptions => OptionSource != AppSettingOptionSource.None;
}