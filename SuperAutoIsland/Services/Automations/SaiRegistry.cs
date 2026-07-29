using System.Text.Json;
using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Enums;
using SuperAutoIsland.Interface;
using SuperAutoIsland.Interface.MetaData;
using SuperAutoIsland.Interface.MetaData.ArgsType;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Models.Actions;
using SuperAutoIsland.Shared;

namespace SuperAutoIsland.Services.Automations;

public static class SaiRegistry
{
    private static ISaiServer SaiServer { get; } = IAppHost.GetService<ISaiServer>();

    public static void Register()
    {
        SaiServer.RegisterBlocks("SuperAutoIsland", new RegisterData
            {
                Actions =
                [
                    new BlockMetadata
                    {
                        Id = "sai.actions.runBlockly",
                        Name = "运行 Blockly 项目",
                        Icon = ("Blockly 项目", FluentIcons.AlignSpaceEvenlyVerticalRegular),
                        Args = new Dictionary<string, MetaArgsBase>
                        {
                            ["ProjectGuid"] = new DynamicDropdownMetaArgs
                            {
                                Name = "",
                                Type = MetaType.dynamic_dropdown,
                                Id = "sai.actions.runBlockly.options"
                            }
                        },
                        DropdownUseNumbers = false,
                        InlineField = false,
                        InlineBlock = false
                    },
                    new BlockMetadata
                    {
                        Id = "sai.actions.runActionSet",
                        Name = "运行可复用的行动组",
                        Icon = ("行动组", FluentIcons.AirplaneTakeOffRegular),
                        Args = new Dictionary<string, MetaArgsBase>
                        {
                            ["ProjectGuid"] = new DynamicDropdownMetaArgs
                            {
                                Name = "",
                                Type = MetaType.dynamic_dropdown,
                                Id = "sai.actions.runActionSet.options"
                            }
                        },
                        DropdownUseNumbers = false,
                        InlineField = false,
                        InlineBlock = false
                    }
                ],
                Rules =
                [
                    new BlockMetadata
                    {
                        Id = "sai.rules.runCiRuleset",
                        Name = "运行可复用的规则集",
                        Icon = ("规则集", FluentIcons.TagMultipleRegular),
                        Args = new Dictionary<string, MetaArgsBase>
                        {
                            ["ProjectGuid"] = new DynamicDropdownMetaArgs
                            {
                                Name = "",
                                Type = MetaType.dynamic_dropdown,
                                Id = "sai.rules.runCiRuleset.options"
                            }
                        },
                        DropdownUseNumbers = false,
                        InlineField = false,
                        InlineBlock = false
                    },
                    new BlockMetadata
                    {
                        Id = "sai.rules.dialogs.yesNo",
                        Name = "YesNo 对话框",
                        Icon = ("对话框", FluentIcons.WindowHeaderHorizontalRegular),
                        Args = new Dictionary<string, MetaArgsBase>
                        {
                            ["dummy1"] = new CommonMetaArgs
                            {
                                Name = "",
                                Type = MetaType.dummy
                            },
                            ["Header"] = new CommonMetaArgs
                            {
                                Name = "标题",
                                Type = MetaType.text
                            },
                            ["Message"] = new CommonMetaArgs
                            {
                                Name = "消息",
                                Type = MetaType.text
                            },
                            ["YesText"] = new CommonMetaArgs
                            {
                                Name = "「Yes」按钮文本",
                                Type = MetaType.text
                            },
                            ["NoText"] = new CommonMetaArgs
                            {
                                Name = "「No」按钮文本",
                                Type = MetaType.text
                            },
                            ["PreferYes"] = new CommonMetaArgs
                            {
                                Name = "默认按钮为「Yes」？",
                                Type = MetaType.boolean
                            },
                            ["Topmost"] = new CommonMetaArgs
                            {
                                Name = "置顶？",
                                Type = MetaType.boolean
                            },
                            ["CountdownEnabled"] = new CommonMetaArgs
                            {
                                Name = "启用倒计时？",
                                Type = MetaType.boolean
                            },
                            ["CountdownTime"] = new CommonMetaArgs
                            {
                                Name = "倒计时时长",
                                Type = MetaType.number
                            },
                        },
                        DropdownUseNumbers = false,
                        InlineField = false,
                        InlineBlock = false
                    }
                ],
                Data = 
                [
                    // new BlockMetadata
                    // {
                    //     Id = "sai.data.test",
                    //     Name = "测试数据",
                    //     Icon = ("测试", FluentIcons.AirplaneTakeOffRegular),
                    //     Args = new Dictionary<string, MetaArgsBase>
                    //     {
                    //         ["_dummy1"] = new CommonMetaArgs
                    //         {
                    //             Name = "repeat 2x and echo",
                    //             Type = MetaType.dummy
                    //         },
                    //         ["Text"] = new CommonMetaArgs
                    //         {
                    //             Name = "",
                    //             Type = MetaType.text
                    //         },
                    //     }
                    // }
                ]
            });

        SaiServer.RegisterWrapper("classisland.os.run.program", RunActionProgramWrapper);

        SaiServer.RegisterDynamicDropdown("sai.actions.runBlockly.options", async () =>
            EnsureListHasItemOrDefaultListItem(
                GlobalConstants.Configs.ProjectConfig!.Data.Projects
                    .Where(e => e.Type is ProjectsType.BlocklyAction)
                    .Select(e => (e.Name, e.Id.ToString()))
                    .ToList(),
                new ValueTuple<string, string>("???",
                    GlobalConstants.Assets.ProjectNullGuid.ToString())));

        SaiServer.RegisterDynamicDropdown("sai.actions.runActionSet.options", async () =>
            EnsureListHasItemOrDefaultListItem(
                GlobalConstants.Configs.ProjectConfig!.Data.Projects
                    .Where(e => e.Type is ProjectsType.CiActionSet)
                    .Select(e => (e.Name, e.Id.ToString()))
                    .ToList(),
                new ValueTuple<string, string>("???",
                    GlobalConstants.Assets.ProjectNullGuid.ToString())));

        SaiServer.RegisterDynamicDropdown("sai.rules.runCiRuleset.options", async () =>
            EnsureListHasItemOrDefaultListItem(
                GlobalConstants.Configs.ProjectConfig!.Data.Projects
                    .Where(e => e.Type is ProjectsType.CiRuleset)
                    .Select(e => (e.Name, e.Id.ToString()))
                    .ToList(),
                new ValueTuple<string, string>("???",
                    GlobalConstants.Assets.ProjectNullGuid.ToString())));
        
        // SaiServer.RegisterDataGetter<TestData>("sai.data.test", async (data) =>
        // {
        //     if (data is not TestData testData) return "???";
        //     return testData.Text + testData.Text;
        // });
    }
    
    /// <summary>
    /// 「运行应用程序」包装器
    /// </summary>
    /// <param name="actionItem">行动项目</param>
    /// <returns>修改后的行动项目</returns>
    private static ActionItem RunActionProgramWrapper(ActionItem actionItem)
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

    private static List<T> EnsureListHasItemOrDefaultListItem<T>(List<T> data, T defaultItem)
    {
        return data.Count > 0 ? data : [defaultItem];
    }
}