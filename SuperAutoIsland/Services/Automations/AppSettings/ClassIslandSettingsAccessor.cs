using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia.Media;
using ClassIsland.Core;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Converters;
using ClassIsland.Core.Icons;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Services.Automations.AppSettings;

/// <summary>
///     ClassIsland 应用设置访问器。
///     <para>
///         SAI 只引用 ClassIsland.PluginSdk（ClassIsland.Core / Shared），无法在编译期引用
///         <c>ClassIsland.Models.Settings</c>，因此这里全部通过反射访问。
///         读取走 <see cref="PropertyInfo.GetValue" />，写入走 <see cref="PropertyInfo.SetValue" />：
///         ClassIsland 的 <c>SettingsService</c> 订阅了 <c>Settings.PropertyChanged</c>，
///         反射写入同样会触发配置落盘与集控审计，与「应用设置」行动的最终效果一致。
///     </para>
/// </summary>
public class ClassIslandSettingsAccessor
{
    /// <summary>
    ///     组件配置方案设置项：ClassIsland 侧使用下拉框编辑，选项为组件配置方案列表。
    /// </summary>
    public const string ComponentConfigPropertyName = "CurrentComponentConfig";

    private static readonly Logger<ClassIslandSettingsAccessor> Logger = new();

    /// <summary>
    ///     与 ClassIsland「应用设置」行动保持一致的宽松 JSON 选项。
    /// </summary>
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        AllowOutOfOrderMetadataProperties = true,
        AllowTrailingCommas = true,
        Converters = { new JsonStringEnumConverter(), new ColorHexJsonConverter() },
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        WriteIndented = false
    };

    private static PropertyInfo? _settingsProperty;
    private static bool _settingsPropertyResolved;
    private static object? _settingsObject;

    private static List<AppSettingDescriptor>? _descriptors;
    private static Dictionary<string, AppSettingDescriptor>? _descriptorMap;
    private static JsonSerializerOptions? _indentedJsonOptions;

    /// <summary>
    ///     缩进输出的宽松 JSON 选项，用于「复制当前值」。
    /// </summary>
    public static JsonSerializerOptions IndentedJsonOptions =>
        _indentedJsonOptions ??= new JsonSerializerOptions(JsonOptions) { WriteIndented = true };

    /// <summary>
    ///     ClassIsland 应用设置对象。取不到时返回 null。
    /// </summary>
    public static object? SettingsObject
    {
        get
        {
            if (_settingsObject != null)
            {
                return _settingsObject;
            }

            var app = AppBase.Current;
            if (app == null)
            {
                return null;
            }

            if (!_settingsPropertyResolved)
            {
                _settingsPropertyResolved = true;
                _settingsProperty = app.GetType().GetProperty(
                    "Settings", BindingFlags.Public | BindingFlags.Instance);

                if (_settingsProperty == null)
                {
                    Logger.Warn("无法在 ClassIsland 应用对象上找到 Settings 属性，应用设置积木不可用。");
                }
            }

            _settingsObject = _settingsProperty?.GetValue(app);
            return _settingsObject;
        }
    }

    /// <summary>
    ///     全部应用设置项（按属性声明顺序，未排序）。
    /// </summary>
    public static IReadOnlyList<AppSettingDescriptor> Descriptors => _descriptors ??= BuildDescriptors();

    /// <summary>
    ///     默认展示的设置项：全部标注了 <c>SettingsInfo</c> 的设置项。
    /// </summary>
    public static string[] DefaultSelection =>
        Descriptors.Where(d => d.IsSettingsInfoAttributed && d.IsSupported)
                   .Select(d => d.PropertyName)
                   .ToArray();

    /// <summary>
    ///     按属性名查找设置项。
    /// </summary>
    /// <param name="propertyName">属性名</param>
    public static AppSettingDescriptor? Find(string propertyName)
    {
        if (_descriptors == null)
        {
            _ = Descriptors;
        }

        _descriptorMap ??= Descriptors.ToDictionary(d => d.PropertyName, StringComparer.Ordinal);
        return _descriptorMap.GetValueOrDefault(propertyName);
    }

    /// <summary>
    ///     读取设置项的当前值，并转换为可传给 Blockly 的形式。
    /// </summary>
    /// <param name="descriptor">设置项</param>
    public static object? GetValue(AppSettingDescriptor descriptor)
    {
        var settings = SettingsObject;
        if (settings == null)
        {
            return null;
        }

        try
        {
            return ToBlocklyValue(descriptor, descriptor.Property.GetValue(settings));
        }
        catch (Exception e)
        {
            Logger.FormatException(e);
            return null;
        }
    }

    /// <summary>
    ///     读取设置项的当前值的预览文本。
    /// </summary>
    /// <param name="descriptor">设置项</param>
    public static string GetPreview(AppSettingDescriptor descriptor)
    {
        var settings = SettingsObject;
        if (settings == null)
        {
            return "[不可用]";
        }

        try
        {
            var raw = descriptor.Property.GetValue(settings);
            return descriptor.Kind switch
            {
                AppSettingValueKind.Boolean   => raw is true ? "开" : "关",
                AppSettingValueKind.Number    => FormatNumber(raw),
                AppSettingValueKind.EnumIndex => FormatEnum(descriptor, raw),
                AppSettingValueKind.Text      => raw?.ToString() ?? "[空]",
                AppSettingValueKind.Color     => raw is Color color ? ToHex(color) : "[空]",
                AppSettingValueKind.Json => raw == null
                                                ? "null"
                                                : JsonSerializer.Serialize(raw, JsonOptions),
                _ => "[不支持]"
            };
        }
        catch (Exception e)
        {
            Logger.FormatException(e);
            return "[读取失败]";
        }
    }

    /// <summary>
    ///     读取设置项的当前值，并转换为可直接粘贴到积木字段中的文本。
    ///     <para>
    ///         一般类型直接输出字面值（<c>true</c> / <c>false</c> / 数值 / 文本 / 颜色，枚举输出选项下标）；
    ///     JSON 类型先用缩进序列化再输出。
    ///     </para>
    /// </summary>
    /// <param name="descriptor">设置项</param>
    public static string GetCopyValue(AppSettingDescriptor descriptor)
    {
        var settings = SettingsObject;
        if (settings == null || !descriptor.IsSupported)
        {
            return string.Empty;
        }

        try
        {
            var raw = descriptor.Property.GetValue(settings);

            return descriptor.Kind switch
            {
                AppSettingValueKind.Boolean   => raw is true ? "true" : "false",
                AppSettingValueKind.Number    => FormatNumberInvariant(raw),
                AppSettingValueKind.EnumIndex => ToIndex(descriptor, raw).ToString(CultureInfo.InvariantCulture),
                AppSettingValueKind.Text      => raw?.ToString() ?? string.Empty,
                AppSettingValueKind.Color     => raw is Color color ? ToHex(color) : string.Empty,
                AppSettingValueKind.Json => raw == null
                                                ? "null"
                                                : JsonSerializer.Serialize(raw, IndentedJsonOptions),
                _ => string.Empty
            };
        }
        catch (Exception e)
        {
            Logger.FormatException(e);
            return string.Empty;
        }
    }

    /// <summary>
    ///     写入设置项。
    /// </summary>
    /// <param name="descriptor">设置项</param>
    /// <param name="value">来自 Blockly 的值</param>
    /// <returns>是否成功，失败时附带原因</returns>
    public static (bool Ok, string? Error) SetValue(AppSettingDescriptor descriptor, JsonElement value)
    {
        if (!descriptor.CanSet)
        {
            return (false, $"设置项 {descriptor.PropertyName} 不支持修改。");
        }

        var settings = SettingsObject;
        if (settings == null)
        {
            return (false, "无法获取 ClassIsland 应用设置对象。");
        }

        try
        {
            var converted = ToPropertyValue(descriptor, value);
            descriptor.Property.SetValue(settings, converted);
            Logger.Debug($"已将应用设置 {descriptor.PropertyName} 修改为 {converted}");
            return (true, null);
        }
        catch (Exception e)
        {
            Logger.Warn($"写入应用设置 {descriptor.PropertyName} 失败：{e.Message}");
            return (false, e.Message);
        }
    }

    #region 描述构建

    private static List<AppSettingDescriptor> BuildDescriptors()
    {
        var settings = SettingsObject;
        if (settings == null)
        {
            Logger.Warn("未能获取 ClassIsland 应用设置对象，应用设置积木分类将为空。");
            return [];
        }

        var result = new List<AppSettingDescriptor>();

        // 与 ClassIsland 的 SettingsService.SettingsPropertiesFlags 保持一致
        foreach (var property in settings.GetType().GetProperties(
                     BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            if (property.GetIndexParameters().Length > 0)
            {
                continue; // 索引器
            }

            if (property.GetMethod is not { IsPublic: true } || property.GetMethod.IsStatic)
            {
                continue;
            }

            result.Add(CreateDescriptor(property));
        }

        Logger.Info($"已枚举 {result.Count} 个 ClassIsland 应用设置项");
        return result;
    }

    private static AppSettingDescriptor CreateDescriptor(PropertyInfo property)
    {
        var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
        var info = property.GetCustomAttribute<SettingsInfo>();
        var isObsolete = property.GetCustomAttribute<ObsoleteAttribute>() != null;
        var canSet = property.SetMethod is { IsPublic: true };

        var enumNames = type.IsEnum
                            ? GetEnumNames(type)
                            : type == typeof(int) && info?.Enums is { Length: > 0 }
                                ? info.Enums
                                : null;

        var kind = Classify(type, enumNames);
        var notes = new List<string>();

        // 「选项参照」积木的选项来源：枚举型取枚举名，组件配置方案取 ClassIsland 的组件配置方案列表
        var optionSource = kind == AppSettingValueKind.EnumIndex && enumNames is { Length: > 0 }
                               ? AppSettingOptionSource.EnumIndex
                               : property.Name == ComponentConfigPropertyName
                                   ? AppSettingOptionSource.ComponentConfig
                                   : AppSettingOptionSource.None;

        if (kind == AppSettingValueKind.Json)
        {
            notes.Add("该设置项以 JSON 文本形式读写，请自行保证格式正确。");
        }

        if (!canSet)
        {
            notes.Add("该设置项不可修改，只会生成获取积木。");
        }

        if (isObsolete)
        {
            notes.Add("该设置项已被 ClassIsland 标记为废弃。");
        }

        return new AppSettingDescriptor
        {
            Property = property,
            PropertyName = property.Name,
            DisplayName = info?.Name is { Length: > 0 } name ? name : property.Name,
            Glyph = info?.Glyph is { Length: > 0 } glyph ? glyph : FluentIcons.SettingsRegular,
            Order = info?.Order ?? 10,
            ValueType = type,
            Kind = kind,
            CanSet = canSet,
            IsObsolete = isObsolete,
            IsSettingsInfoAttributed = info != null,
            EnumNames = kind == AppSettingValueKind.EnumIndex ? enumNames : null,
            OptionSource = optionSource,
            Note = string.Join("\n", notes)
        };
    }

    private static AppSettingValueKind Classify(Type type, string[]? enumNames)
    {
        if (type.IsEnum)
        {
            return AppSettingValueKind.EnumIndex;
        }

        if (type == typeof(bool))
        {
            return AppSettingValueKind.Boolean;
        }

        if (type == typeof(int) || type == typeof(double))
        {
            return enumNames is { Length: > 0 } ? AppSettingValueKind.EnumIndex : AppSettingValueKind.Number;
        }

        if (type == typeof(string))
        {
            return AppSettingValueKind.Text;
        }

        if (type == typeof(Color))
        {
            return AppSettingValueKind.Color;
        }

        // 集合、字典、数组等无法用单个积木表达
        return IsCollectionLike(type) ? AppSettingValueKind.None : AppSettingValueKind.Json;
    }

    private static bool IsCollectionLike(Type type)
    {
        return type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type);
    }

    private static string[] GetEnumNames(Type enumType)
    {
        return Enum.GetValues(enumType)
                   .Cast<object>()
                   .Select(value => enumType.GetField(value.ToString()!)?
                                        .GetCustomAttribute<DescriptionAttribute>()?
                                        .Description ?? value.ToString()!)
                   .ToArray();
    }

    #endregion

    #region 值转换

    private static object ToBlocklyValue(AppSettingDescriptor descriptor, object? raw)
    {
        switch (descriptor.Kind)
        {
            case AppSettingValueKind.Boolean:
                return raw is true;
            case AppSettingValueKind.Number:
                return raw switch
                {
                    int i    => (object)i,
                    double d => d,
                    float f  => (double)f,
                    long l   => l,
                    _        => 0
                };
            case AppSettingValueKind.Text:
                return raw?.ToString() ?? string.Empty;
            case AppSettingValueKind.Color:
                return raw is Color color ? ToHex(color) : "#000000";
            case AppSettingValueKind.EnumIndex:
                return ToIndex(descriptor, raw);
            case AppSettingValueKind.Json:
                return raw == null ? "null" : JsonSerializer.Serialize(raw, JsonOptions);
            default:
                return string.Empty;
        }
    }

    private static object? ToPropertyValue(AppSettingDescriptor descriptor, JsonElement value)
    {
        var type = descriptor.Property.PropertyType;
        var underlying = descriptor.ValueType;

        switch (descriptor.Kind)
        {
            case AppSettingValueKind.Boolean:
                return ReadBoolean(value);
            case AppSettingValueKind.Number:
                // 注意：两个分支必须显式转成 object，否则条件表达式的类型会被推断为 double，
                // 导致 int 型设置项写入时报「Double 无法转换为 Int32」。
                return underlying == typeof(int) ? (object)ReadInt32(value) : ReadDouble(value);
            case AppSettingValueKind.Text:
                return ReadString(value);
            case AppSettingValueKind.Color:
                return FromHex(ReadString(value));
            case AppSettingValueKind.EnumIndex:
                var index = ReadInt32(value);
                if (!underlying.IsEnum)
                {
                    return index;
                }

                var values = Enum.GetValues(underlying);
                if (index < 0 || index >= values.Length)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value), $"{index} 不是 {descriptor.PropertyName} 的有效选项下标。");
                }

                return values.GetValue(index);
            case AppSettingValueKind.Json:
                var raw = value.ValueKind == JsonValueKind.String
                              ? value.GetString() ?? "null"
                              : value.GetRawText();
                return JsonSerializer.Deserialize(raw, underlying, JsonOptions);
            default:
                throw new NotSupportedException($"设置项 {descriptor.PropertyName} 不支持写入。");
        }
    }

    private static int ToIndex(AppSettingDescriptor descriptor, object? raw)
    {
        if (raw == null)
        {
            return 0;
        }

        if (descriptor.ValueType.IsEnum)
        {
            return Math.Max(Array.IndexOf(Enum.GetValues(descriptor.ValueType), raw), 0);
        }

        return raw switch
        {
            int i    => i,
            double d => (int)Math.Round(d),
            _        => 0
        };
    }

    private static bool ReadBoolean(JsonElement value)
    {
        return value.ValueKind switch
        {
            JsonValueKind.True   => true,
            JsonValueKind.False  => false,
            JsonValueKind.Number => Math.Abs(value.GetDouble()) > double.Epsilon,
            JsonValueKind.String => ReadBooleanFromString(value.GetString()),
            _                    => false
        };
    }

    private static bool ReadBooleanFromString(string? raw)
    {
        return raw?.Trim().ToUpperInvariant() switch
        {
            "TRUE" or "1" => true,
            _             => false
        };
    }

    private static int ReadInt32(JsonElement value)
    {
        return (int)Math.Round(ReadDouble(value));
    }

    private static double ReadDouble(JsonElement value)
    {
        switch (value.ValueKind)
        {
            case JsonValueKind.Number:
                return value.GetDouble();
            case JsonValueKind.String:
                return double.TryParse(value.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture,
                                       out var parsed)
                           ? parsed
                           : 0d;
            case JsonValueKind.True:
                return 1d;
            case JsonValueKind.False:
                return 0d;
            default:
                return 0d;
        }
    }

    private static string ReadString(JsonElement value)
    {
        return value.ValueKind switch
        {
            JsonValueKind.String                          => value.GetString() ?? string.Empty,
            JsonValueKind.Null or JsonValueKind.Undefined => string.Empty,
            _                                             => value.GetRawText()
        };
    }

    /// <summary>
    ///     转换为「#RRGGBB」或「#RRGGBBAA」形式的颜色文本。与 ClassIsland 的
    ///     <c>ColorHexJsonConverter</c> 一样采用尾部 Alpha。
    /// </summary>
    private static string ToHex(Color color)
    {
        return color.A == 0xFF
                   ? $"#{color.R:X2}{color.G:X2}{color.B:X2}"
                   : $"#{color.R:X2}{color.G:X2}{color.B:X2}{color.A:X2}";
    }

    private static Color FromHex(string raw)
    {
        var text = raw.Trim().TrimStart('#');

        if (text.Length == 6)
        {
            text += "FF";
        }

        if (text.Length == 8 && byte.TryParse(text[..2], NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                                              out var r) &&
            byte.TryParse(text[2..4], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var g) &&
            byte.TryParse(text[4..6], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var b) &&
            byte.TryParse(text[6..8], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var a))
        {
            return Color.FromArgb(a, r, g, b);
        }

        if (Color.TryParse(raw, out var color))
        {
            return color;
        }

        throw new FormatException($"无法解析颜色“{raw}”。");
    }

    private static string FormatNumber(object? raw)
    {
        return raw switch
        {
            double d => d.ToString("0.0#####", CultureInfo.InvariantCulture).TrimEnd('0').TrimEnd('.') +
                        (Math.Abs(d % 1) < double.Epsilon ? ".0" : string.Empty),
            _ => raw?.ToString() ?? "0"
        };
    }

    /// <summary>
    ///     以固定区域性格式化数值，供「复制当前值」使用（0.85 复制为 0.85，而不是 0.8500000000000001）。
    /// </summary>
    private static string FormatNumberInvariant(object? raw)
    {
        return raw switch
        {
            int i     => i.ToString(CultureInfo.InvariantCulture),
            long l    => l.ToString(CultureInfo.InvariantCulture),
            short s   => s.ToString(CultureInfo.InvariantCulture),
            byte b    => b.ToString(CultureInfo.InvariantCulture),
            double d  => d.ToString(CultureInfo.InvariantCulture),
            float f   => f.ToString(CultureInfo.InvariantCulture),
            decimal m => m.ToString(CultureInfo.InvariantCulture),
            _         => Convert.ToString(raw, CultureInfo.InvariantCulture) ?? "0"
        };
    }

    private static string FormatEnum(AppSettingDescriptor descriptor, object? raw)
    {
        var index = ToIndex(descriptor, raw);
        var names = descriptor.EnumNames;
        return names != null && index >= 0 && index < names.Length ? names[index] : index.ToString();
    }

    #endregion
}