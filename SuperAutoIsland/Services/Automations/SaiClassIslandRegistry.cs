using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Services.Automations.Blocks.ClassIsland;

namespace SuperAutoIsland.Services.Automations;

public static class SaiClassIslandRegistry
{
    private static ISaiServer SaiServer { get; } = IAppHost.GetService<ISaiServer>();

    public static void Register()
    {
        SaiServer.RegisterBlocks("ClassIsland", it => it
            .AddLabel("规则")
            .AddBlock(new BlockMetadata("classisland.windows.className")
            {
                Kind = BlockKind.Rule,
                Name = "前台窗口类名",
                Icon = ("窗口指纹", "\uF4A2"),
                InlineField = true,
                Fields = new Dictionary<string, Field>
                {
                    ["UseRegex"] = BasicFields.CheckBox("正则:"),
                    ["Text"] = BasicFields.Text("")
                }
            })
            .AddBlock(new BlockMetadata("classisland.windows.text")
            {
                Kind = BlockKind.Rule,
                Name = "前台窗口标题",
                Icon = ("文本字段", "\uF26B"),
                InlineField = true,
                Fields = new Dictionary<string, Field>
                {
                    ["UseRegex"] = BasicFields.CheckBox("正则:"),
                    ["Text"] = BasicFields.Text("")
                }
            })
            .AddBlock(new BlockMetadata("classisland.windows.status")
            {
                Kind = BlockKind.Rule,
                Name = "前台窗口状态是",
                Icon = ("面板独立窗口", "\uEC83"),
                InlineBlock = true,
                InlineField = true,
                Fields = new Dictionary<string, Field>
                {
                    ["State"] = BasicFields.Dropdown("", [
                        ("正常", "0"), ("最大化", "1"), ("最小化", "2"), ("全屏", "3")
                    ], true)
                }
            })
            .AddBlock(new BlockMetadata("classisland.windows.processName")
            {
                Kind = BlockKind.Rule,
                Name = "前台窗口进程",
                Icon = ("窗口AD人", "\uF488"),
                InlineField = true,
                Fields = new Dictionary<string, Field>
                {
                    ["UseRegex"] = BasicFields.CheckBox("正则:"),
                    ["Text"] = BasicFields.Text("")
                }
            })
            .AddBlock(new BlockMetadata("classisland.lessons.currentSubject")
            {
                Kind = BlockKind.Rule,
                Name = "科目是",
                Icon = ("书", "\uE215"),
                InlineBlock = true,
                InlineField = true,
                Fields = new Dictionary<string, Field>
                {
                    ["SubjectId"] = BasicFields.DynamicDropdown("", "classisland.lessons.subjects")
                }
            })
            .AddBlock(new BlockMetadata("classisland.lessons.nextSubject")
            {
                Kind = BlockKind.Rule,
                Name = "下节课科目是",
                Icon = ("创建书", "\uE217"),
                InlineBlock = true,
                InlineField = true,
                Fields = new Dictionary<string, Field>
                {
                    ["SubjectId"] = BasicFields.DynamicDropdown("", "classisland.lessons.subjects")
                }
            })
            .AddBlock(new BlockMetadata("classisland.lessons.previousSubject")
            {
                Kind = BlockKind.Rule,
                Name = "上节课科目是",
                Icon = ("删除书", "\uE226"),
                InlineBlock = true,
                InlineField = true,
                Fields = new Dictionary<string, Field>
                {
                    ["SubjectId"] = BasicFields.DynamicDropdown("", "classisland.lessons.subjects")
                }
            })
            .AddBlock(new BlockMetadata("classisland.lessons.timeState")
            {
                Kind = BlockKind.Rule,
                Name = "当前时间状态是",
                Icon = ("钟表", "\uE4C4"),
                InlineBlock = true,
                InlineField = true,
                Fields = new Dictionary<string, Field>
                {
                    ["State"] = BasicFields.Dropdown("", [
                        ("无", "0"), ("上课", "1"), ("准备上课（这个没用）", "2"),
                        ("课间休息", "3"), ("放学后", "4")
                    ], true)
                }
            })
            .AddBlock(new BlockMetadata("classisland.weather.currentWeather")
            {
                Kind = BlockKind.Rule,
                Name = "当前天气是",
                Icon = ("多云", "\uE4DC"),
                InlineBlock = true,
                InlineField = true,
                Fields = new Dictionary<string, Field>
                {
                    ["WeatherId"] = BasicFields.Dropdown("", WeatherOptions, true)
                }
            })
            .AddBlock(new BlockMetadata("classisland.weather.hasWeatherAlert")
            {
                Kind = BlockKind.Rule,
                Name = "存在气象预警",
                Icon = ("警告", "\uF431"),
                InlineField = true,
                Fields = new Dictionary<string, Field>
                {
                    ["UseRegex"] = BasicFields.CheckBox("正则:"),
                    ["Text"] = BasicFields.Text("")
                }
            })
            .AddBlock(new BlockMetadata("classisland.weather.rainTime")
            {
                Kind = BlockKind.Rule,
                Name = "距离降水开始/结束还剩",
                Icon = ("雨", "\uF43F"),
                Fields = new Dictionary<string, Field>
                {
                    ["label1"] = BasicFields.Dummy("\n"),
                    ["IsRemainingTime"] = BasicFields.CheckBox("是否为距离结束:"),
                    ["RainTimeMinutes"] = BasicFields.Number("距离开始/结束剩余时间（分钟）")
                }
            })
            .AddLabel("行动")
            .AddBlock(new BlockMetadata("classisland.broadcastSignal")
            {
                Kind = BlockKind.Action,
                Name = "广播信号",
                Icon = ("广播", "\uE561"),
                Fields = new Dictionary<string, Field>
                {
                    ["SignalName"] = BasicFields.Text("")
                }
            })
            .AddBlock(new BlockMetadata("classisland.settings.currentComponentConfig")
            {
                Kind = BlockKind.Action,
                Name = "组件配置方案",
                Icon = ("Apps", "\uE06F"),
                InlineBlock = true,
                InlineField = true,
                Fields = new Dictionary<string, Field>
                {
                    ["Value"] = BasicFields.DynamicDropdown("修改为", "classisland.settings.componentConfigs")
                }
            })
            .AddBlock(new BlockMetadata("classisland.settings.theme")
            {
                Kind = BlockKind.Action,
                Name = "应用主题",
                Icon = ("主题", "\uE5CB"),
                InlineBlock = true,
                InlineField = true,
                Fields = new Dictionary<string, Field>
                {
                    ["Value"] = BasicFields.Dropdown("修改为", [("跟随系统", "0"), ("明亮", "1"), ("黑暗", "2")], true)
                }
            })
            .AddBlock(new BlockMetadata("classisland.settings.windowDockingLocation")
            {
                Kind = BlockKind.Action,
                Name = "窗口停靠位置",
                Icon = ("TV", "\uF397"),
                InlineBlock = true,
                InlineField = true,
                Fields = new Dictionary<string, Field>
                {
                    ["Value"] = BasicFields.Dropdown("移动到", [
                        ("左上角", "0"), ("中上侧", "1"), ("右上角", "2"),
                        ("左下角", "3"), ("中下侧", "4"), ("右下角", "5")
                    ], true)
                }
            })
            .AddBlock(new BlockMetadata("classisland.settings.windowLayer")
            {
                Kind = BlockKind.Action,
                Name = "窗口层级",
                Icon = ("层级", "\uEA2F"),
                InlineBlock = true,
                InlineField = true,
                Fields = new Dictionary<string, Field>
                {
                    ["Value"] = BasicFields.Dropdown("", [("置底", "0"), ("置顶", "1")], true)
                }
            })
            .AddBlock(new BlockMetadata("classisland.settings.windowDockingOffsetX")
            {
                Kind = BlockKind.Action,
                Name = "窗口向右偏移",
                Icon = ("左右箭头", "\uE099"),
                Fields = new Dictionary<string, Field>
                {
                    ["Value"] = BasicFields.Number("修改为")
                }
            })
            .AddBlock(new BlockMetadata("classisland.settings.windowDockingOffsetY")
            {
                Kind = BlockKind.Action,
                Name = "窗口向下偏移",
                Icon = ("下箭头", "\uE094"),
                Fields = new Dictionary<string, Field>
                {
                    ["Value"] = BasicFields.Number("修改为")
                }
            })
            .AddBlock(new BlockMetadata("classisland.os.run")
            {
                Kind = BlockKind.Action,
                Name = "运行",
                Icon = ("打开", "\uEC2E"),
                InlineField = true,
                Fields = new Dictionary<string, Field>
                {
                    ["RunType"] = BasicFields.Dropdown("",
                        [("终端命令", "Command"), ("文件", "File"), ("文件夹", "Folder"), ("Url 链接", "Url")]),
                    ["Value"] = BasicFields.Text("")
                }
            })
            .AddBlock<ClassIslandRunProgramBlock>()
            .AddBlock<ClassIslandSleepBlock>()
            .AddBlock(new BlockMetadata("classisland.showNotification")
            {
                Kind = BlockKind.Action,
                Name = "显示提醒",
                Icon = ("提醒", "\uE02B"),
                Fields = new Dictionary<string, Field>
                {
                    ["label1"] = BasicFields.Dummy("内容设置:"),
                    ["Mask"] = BasicFields.Text("标题内容"),
                    ["MaskDurationSeconds"] = BasicFields.Number("标题持续时间(秒)", 5),
                    ["Content"] = BasicFields.Text("正文内容"),
                    ["ContentDurationSeconds"] = BasicFields.Number("正文持续时长(秒)", 10),
                    ["IsMaskSpeechEnabled"] = BasicFields.Boolean("启用标题语音", true),
                    ["IsContentSpeechEnabled"] = BasicFields.Boolean("启用正文语音", true),
                    ["IsWaitForCompleteEnabled"] = BasicFields.Boolean("等待提醒结束", false),
                    ["label2"] = BasicFields.Dummy("高级设置:"),
                    ["IsAdvancedSettingsEnabled"] = BasicFields.CheckBox("启用?"),
                    ["IsTopmostEnabled"] = BasicFields.Boolean("置顶提醒", true),
                    ["IsEffectEnabled"] = BasicFields.Boolean("启用提醒特效", true),
                    ["IsSoundEffectEnabled"] = BasicFields.Boolean("启用提醒音效", true),
                    ["CustomSoundEffectPath"] = BasicFields.Text("自定义提醒音效(留空默认)")
                }
            })
            .AddBlock(new BlockMetadata("classisland.notification.weather")
            {
                Kind = BlockKind.Action,
                Name = "显示天气提醒",
                Icon = ("多云", "\uF44F"),
                Fields = new Dictionary<string, Field>
                {
                    ["NotificationKind"] =
                        BasicFields.Dropdown("", [("三天天气预报", "0"), ("气象预警", "1"), ("逐小时天气预报", "2")], true)
                }
            })
            .AddBlock(new BlockMetadata("classisland.app.quit")
            {
                Kind = BlockKind.Action,
                Name = "退出 ClassIsland",
                Icon = ("退出", "\uE0DE")
            })
            .AddBlock(new BlockMetadata("classisland.app.restart")
            {
                Kind = BlockKind.Action,
                Name = "重启 ClassIsland",
                Icon = ("转圈箭头", "\uE0BD"),
                Fields = new Dictionary<string, Field>
                {
                    ["Value"] = BasicFields.Boolean("静默重启:")
                }
            }));

        SaiServer.RegisterDynamicDropdown("classisland.lessons.subjects", async () =>
            IAppHost.GetService<IProfileService>().Profile.Subjects
                .Select(x => (x.Value.Name, x.Key.ToString()))
                .ToList());

        SaiServer.RegisterDynamicDropdown("classisland.settings.componentConfigs", async () =>
            IAppHost.GetService<IComponentsService>().ComponentConfigs
                .Select(x => (x, x))
                .ToList());
    }

    private static readonly List<(string, string)> WeatherOptions =
    [
        ("晴", "0"), ("多云", "1"), ("阴", "2"), ("阵雨", "3"), ("雷阵雨", "4"),
        ("雷阵雨并伴有冰雹", "5"), ("雨夹雪", "6"), ("小雨", "7"), ("中雨", "8"),
        ("大雨", "9"), ("暴雨", "10"), ("大暴雨", "11"), ("特大暴雨", "12"),
        ("阵雪", "13"), ("小雪", "14"), ("中雪", "15"), ("大雪", "16"), ("暴雪", "17"),
        ("雾", "18"), ("冻雨", "19"), ("沙尘暴", "20"), ("小雨-中雨", "21"),
        ("中雨-大雨", "22"), ("大雨-暴雨", "23"), ("暴雨-大暴雨", "24"),
        ("大暴雨-特大暴雨", "25"), ("小雪-中雪", "26"), ("中雪-大雪", "27"),
        ("大雪-暴雪", "28"), ("浮尘", "29"), ("扬沙", "30"), ("强沙尘暴", "31"),
        ("飑", "32"), ("龙卷风", "33"), ("弱高吹雪", "34"), ("轻雾", "35"),
        ("霾", "53"), ("雨", "301"), ("雪", "302"), ("未知", "99")
    ];
}