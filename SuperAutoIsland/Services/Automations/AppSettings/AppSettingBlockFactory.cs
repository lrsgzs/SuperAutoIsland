using System.Globalization;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;

namespace SuperAutoIsland.Services.Automations.AppSettings;

/// <summary>
///     应用设置积木工厂：把设置项描述转换为 SAI 积木元数据。
/// </summary>
public static class AppSettingBlockFactory
{
    /// <summary>
    ///     设置积木 id 前缀。
    /// </summary>
    public const string SetIdPrefix = "sai.appSettings.set.";

    /// <summary>
    ///     获取积木 id 前缀。
    /// </summary>
    public const string GetIdPrefix = "sai.appSettings.get.";

    /// <summary>
    ///     选项参照积木 id 前缀。
    /// </summary>
    public const string OptionIdPrefix = "sai.appSettings.option.";

    /// <summary>
    ///     全部积木 id 的公共前缀。
    /// </summary>
    public const string IdPrefix = "sai.appSettings.";

    /// <summary>
    ///     构建「应用设置」分类。
    /// </summary>
    /// <param name="it">积木注册器</param>
    public static void Build(BlocksRegister it)
    {
        var selected = AppSettingsSelection.Resolve();

        // 与「选择要展示的设置项」界面使用同一套排序（分组 → Order → 名称），保证两侧顺序一致
        var chosen = AppSettingGrouping.SortForDisplay(
            ClassIslandSettingsAccessor.Descriptors
                                       .Where(d => d.IsSupported && selected.Contains(d.PropertyName)));

        if (chosen.Count == 0)
        {
            it.AddLabel("尚未选择任何设置项。请在「SuperAutoIsland 主设置 - 应用设置积木」中选择。");
            return;
        }

        // 1. 选项参照积木：只有下拉框，置于分类最顶端（不加分组标签），用于和获取积木比较
        foreach (var descriptor in chosen.Where(d => d.HasOptions))
        {
            if (CreateOptionBlock(descriptor) is { } optionBlock)
            {
                it.AddBlock(optionBlock);
            }
        }

        // 2. 设置积木
        var settable = chosen.Where(d => d.CanSet).ToList();
        if (settable.Count > 0)
        {
            it.AddLabel("设置");

            foreach (var descriptor in settable)
            {
                it.AddBlock(CreateSetBlock(descriptor));
            }
        }

        // 3. 获取积木
        it.AddLabel("获取设置");

        foreach (var descriptor in chosen)
        {
            it.AddBlock(CreateGetBlock(descriptor));
        }
    }

    /// <summary>
    ///     生成积木 id 对应的 Blockly 类型名（前端会把「.」替换为「_」）。
    /// </summary>
    /// <param name="blockId">积木 id</param>
    public static string ToBlocklyType(string blockId)
    {
        return blockId.Replace('.', '_');
    }

    /// <summary>
    ///     解析设置项的选项列表。返回 null 表示当前取不到选项（例如尚未创建任何组件配置方案）。
    /// </summary>
    /// <param name="descriptor">设置项</param>
    /// <param name="dataOutput">参照积木的输出类型</param>
    private static List<(string Name, string Value)>? ResolveOptions(AppSettingDescriptor descriptor,
                                                                     out string dataOutput)
    {
        switch (descriptor.OptionSource)
        {
            case AppSettingOptionSource.EnumIndex:
                dataOutput = "Number";

                return descriptor.EnumNames?
                                 .Select((name, index) => (name, index.ToString(CultureInfo.InvariantCulture)))
                                 .ToList();

            case AppSettingOptionSource.ComponentConfig:
                dataOutput = "String";

                return GetComponentConfigs()?.Select(config => (config, config)).ToList();

            default:
                dataOutput = "String";
                return null;
        }
    }

    /// <summary>
    ///     创建选项参照积木。选项为空时返回 null。
    /// </summary>
    private static BlockMetadata? CreateOptionBlock(AppSettingDescriptor descriptor)
    {
        var options = ResolveOptions(descriptor, out var dataOutput);

        if (options is not { Count: > 0 })
        {
            return null;
        }

        var valueHint = descriptor.OptionSource == AppSettingOptionSource.ComponentConfig
                            ? "输出所选方案的名称"
                            : "输出对应的选项下标（从 0 开始）";

        return new BlockMetadata(OptionIdPrefix + descriptor.PropertyName)
        {
            Kind = BlockKind.Data,
            Name = $"{descriptor.DisplayName} 选项",
            Icon = (descriptor.DisplayName, descriptor.Glyph),
            DataOutput = dataOutput,
            Tooltip =
                $"「{descriptor.DisplayName}」的可选值。{valueHint}，可用于和「获取 {descriptor.DisplayName}」比较，也可以直接接到「设置 {descriptor.DisplayName}」上。",
            Fields = { ["Value"] = BasicFields.Dropdown(string.Empty, options, dataOutput == "Number") }
        };
    }

    /// <summary>
    ///     创建设置积木。
    /// </summary>
    private static BlockMetadata CreateSetBlock(AppSettingDescriptor descriptor)
    {
        return new BlockMetadata(SetIdPrefix + descriptor.PropertyName)
        {
            Kind = BlockKind.Action,
            Name = $"设置 {descriptor.DisplayName}",
            Icon = (descriptor.DisplayName, descriptor.Glyph),
            Tooltip = BuildTooltip(descriptor, true),
            Fields = { ["Value"] = CreateValueField(descriptor) }
        };
    }

    /// <summary>
    ///     创建获取积木。
    /// </summary>
    private static BlockMetadata CreateGetBlock(AppSettingDescriptor descriptor)
    {
        return new BlockMetadata(GetIdPrefix + descriptor.PropertyName)
        {
            Kind = BlockKind.Data,
            Name = $"获取 {descriptor.DisplayName}",
            Icon = (descriptor.DisplayName, descriptor.Glyph),
            DataOutput = GetDataOutput(descriptor),
            Tooltip = BuildTooltip(descriptor, false)
        };
    }

    /// <summary>
    ///     按设置项类型创建「修改为」字段。
    /// </summary>
    private static Field CreateValueField(AppSettingDescriptor descriptor)
    {
        switch (descriptor.Kind)
        {
            case AppSettingValueKind.Boolean:
                return BasicFields.Boolean("修改为", ReadCurrent(descriptor) is true);

            case AppSettingValueKind.Number:
                return BasicFields.Number("修改为", ReadCurrentDouble(descriptor));

            case AppSettingValueKind.Text:
                // 组件配置方案：与枚举一样做成「输入槽 + 影子 = 选项参照积木」，
                // 既可下拉选择，也可以把「获取」积木接进来。取不到方案列表时退回普通文本输入。
                if (descriptor.OptionSource == AppSettingOptionSource.ComponentConfig)
                {
                    var current = ReadCurrentString(descriptor);
                    var configs = GetComponentConfigs();

                    if (configs is { Count: > 0 } && configs.Contains(current))
                    {
                        return CreateOptionInputField(descriptor, "String", current);
                    }
                }

                return BasicFields.Text("修改为", ReadCurrentString(descriptor));

            case AppSettingValueKind.Color:
                return BasicFields.Color("修改为", ReadCurrentString(descriptor, "#000000"));

            case AppSettingValueKind.EnumIndex:
                // 输入槽 + 影子积木 = 该设置项的「选项参照」积木：
                // 默认渲染为只有下拉框的积木，同时可以把「获取」积木直接接进来。
                return CreateOptionInputField(
                    descriptor, "Number", ReadCurrentInt(descriptor).ToString(CultureInfo.InvariantCulture));

            case AppSettingValueKind.Json:
                return BasicFields.Text("修改为(JSON)", ReadCurrentString(descriptor, "null"));

            default:
                return BasicFields.Text("修改为", string.Empty);
        }
    }

    /// <summary>
    ///     创建「选项参照」型的输入槽：默认渲染为该设置项的选项参照积木，也可接入其它积木。
    /// </summary>
    /// <param name="descriptor">设置项</param>
    /// <param name="check">输入类型</param>
    /// <param name="currentValue">当前值，作为影子积木的默认选项</param>
    private static Field CreateOptionInputField(AppSettingDescriptor descriptor, string check, string currentValue)
    {
        return BasicFields.CreateInputField("修改为", field =>
        {
            field.Check = check;
            field.ShadowBlockType = ToBlocklyType(OptionIdPrefix + descriptor.PropertyName);
            field.Options["Value"] = currentValue;
        });
    }

    /// <summary>
    ///     获取积木的输出类型。
    /// </summary>
    private static string GetDataOutput(AppSettingDescriptor descriptor)
    {
        return descriptor.Kind switch
        {
            AppSettingValueKind.Boolean                                 => "Boolean",
            AppSettingValueKind.Number or AppSettingValueKind.EnumIndex => "Number",
            AppSettingValueKind.Color                                   => "SAI_Color",
            _                                                           => "String"
        };
    }

    private static string BuildTooltip(AppSettingDescriptor descriptor, bool isSet)
    {
        var lines = new List<string>
        {
            isSet
                ? $"把 ClassIsland 应用设置「{descriptor.DisplayName}」修改为指定值。修改会立即生效并写入 ClassIsland 配置。"
                : $"读取 ClassIsland 应用设置「{descriptor.DisplayName}」的当前值。"
        };

        if (descriptor.Kind == AppSettingValueKind.Json)
        {
            lines.Add("该设置项以 JSON 文本形式读写。");
        }

        if (descriptor.IsObsolete)
        {
            lines.Add("该设置项已被 ClassIsland 标记为废弃。");
        }

        return string.Join("\n", lines);
    }

    private static List<string>? GetComponentConfigs()
    {
        try
        {
            return IAppHost.TryGetService<IComponentsService>()?.ComponentConfigs.ToList();
        }
        catch
        {
            return null;
        }
    }

    private static object? ReadCurrent(AppSettingDescriptor descriptor)
    {
        return ClassIslandSettingsAccessor.GetValue(descriptor);
    }

    private static double ReadCurrentDouble(AppSettingDescriptor descriptor)
    {
        return ReadCurrent(descriptor) switch
        {
            double d => d,
            int i    => i,
            long l   => l,
            _        => 0d
        };
    }

    private static int ReadCurrentInt(AppSettingDescriptor descriptor)
    {
        return (int)Math.Round(ReadCurrentDouble(descriptor));
    }

    private static string ReadCurrentString(AppSettingDescriptor descriptor, string fallback = "")
    {
        return ReadCurrent(descriptor)?.ToString() ?? fallback;
    }
}