using System.Diagnostics;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using ClassIsland.Core;
using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using ClassIsland.Shared.Models.Automation;
using FluentAvalonia.UI.Controls;
using SuperAutoIsland.Enums;
using SuperAutoIsland.Interface;
using SuperAutoIsland.Interface.MetaData;
using SuperAutoIsland.Interface.MetaData.ArgsType;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Models.Actions;
using SuperAutoIsland.Models.Data;
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
                                Name = "倒计时时长(s)",
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
                    // },
                    new BlockMetadata
                    {
                        Id = "sai.rules.dialogs.text",
                        Name = "文本输入对话框",
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
                            ["DefaultText"] = new CommonMetaArgs
                            {
                                Name = "默认文本",
                                Type = MetaType.text
                            },
                            ["OkText"] = new CommonMetaArgs
                            {
                                Name = "「Ok」按钮文本",
                                Type = MetaType.text
                            },
                            ["CancelText"] = new CommonMetaArgs
                            {
                                Name = "「Cancel」按钮文本",
                                Type = MetaType.text
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
                                Name = "倒计时时长(s)",
                                Type = MetaType.number
                            },
                        },
                        DropdownUseNumbers = false,
                        InlineField = false,
                        InlineBlock = false
                    },
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
        
        // SaiServer.RegisterDataGetter<TestData>("sai.data.test", (data) =>
        // {
        //     try
        //     {
        //         if (data is not TestData testData) return Task.FromResult("???");
        //         
        //         return Task.FromResult(testData.Text + testData.Text);
        //     }
        //     catch (Exception exception)
        //     {
        //         return Task.FromException<string>(exception);
        //     }
        // });
        
        SaiServer.RegisterDataGetter<TextDialogDataModel>("sai.rules.dialogs.text", async data =>
        {
            if (data is not TextDialogDataModel model) return "???";
            return await ShowDialogAsync(model);
        });
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

    public static async Task<string> ShowDialogAsync(TextDialogDataModel settings)
    {
        var cancelButton = new FATaskDialogButton(settings.CancelText, false);
        
        var okButton = new FATaskDialogButton(settings.OkText, true)
        {
            IsDefault = true
        };

        var textBox = new TextBox
        {
            Text = settings.DefaultText
        };
        
        var dialog = new FATaskDialog
        {
            Title = settings.Header,
            Header = settings.Header,
            Content = new StackPanel
            {
                Spacing = 8,
                Children =
                {
                    new TextBlock
                    {
                        Text = settings.Message,
                        TextWrapping = TextWrapping.Wrap
                    },
                    textBox
                }
            },
            Buttons = [cancelButton, okButton],
            XamlRoot = AppBase.Current.GetRootWindow()
        };

        okButton.IsEnabled = !settings.CountdownEnabled && !string.IsNullOrEmpty(textBox.Text.Trim());
        
        if (settings.CountdownEnabled)
        {
            var stopwatch = Stopwatch.StartNew();
            var completed = false;
            dialog.Closing += (sender, args) =>
            {
                args.Cancel = !completed;
            };
            
            cancelButton.IsEnabled = false;
            okButton.IsEnabled = false;
            
            textBox.TextChanged += (sender, args) =>
            {
                okButton.IsEnabled = completed && !string.IsNullOrEmpty(textBox.Text.Trim());
            };
            
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    var targetMs = settings.CountdownTime * 1000;
                    var elapsedMs = stopwatch.ElapsedMilliseconds;

                    if (elapsedMs >= targetMs)
                    {
                        break;
                    }

                    var remainingMs = (int)(targetMs - elapsedMs);
                    var remainingTime = Math.Ceiling((double)remainingMs / 1000);

                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        okButton.Text = $"{settings.OkText} ({remainingTime:0}s)";
                    });

                    var checkInterval = Math.Min(remainingMs, 1000);
                    await Task.Delay(checkInterval);
                }
                
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    completed = true;
                    cancelButton.IsEnabled = true;
                    okButton.IsEnabled = !string.IsNullOrEmpty(textBox.Text.Trim());
                    okButton.Text = settings.OkText;
                });
            });
        }
        else
        {
            textBox.TextChanged += (sender, args) =>
            {
                okButton.IsEnabled = !string.IsNullOrEmpty(textBox.Text.Trim());
            };
        }
        
        var task = dialog.ShowAsync();
        if (AppBase.Current.DesktopLifetime != null && settings.Topmost)
        {
            await Task.Delay(100);
            var topLevel = TopLevel.GetTopLevel(dialog);
            if (topLevel is Window window)
            {
                window.Topmost = true;
            }
        }
        var result = await task;

        return Equals(result, true) ? textBox.Text : "canceled";
    }

    private static List<T> EnsureListHasItemOrDefaultListItem<T>(List<T> data, T defaultItem)
    {
        return data.Count > 0 ? data : [defaultItem];
    }
}