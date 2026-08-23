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
using SuperAutoIsland.Interface.Metadata;
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
                new BlockMetadata("sai.actions.setDynamicText")
                {
                    Kind = BlockKind.Action,
                    Name = "设置动态文本",
                    Icon = ("文本编辑", FluentIcons.TextEditStyleRegular),
                    Fields = new Dictionary<string, Field>
                    {
                        ["Key"] = BasicFields.Text("将"),
                        ["Value"] = BasicFields.Text("修改为")
                    }
                },
                new BlockMetadata("sai.actions.runBlockly")
                {
                    Kind = BlockKind.Action,
                    Name = "运行 Blockly 项目",
                    Icon = ("Blockly 项目", FluentIcons.AlignSpaceEvenlyVerticalRegular),
                    Fields = new Dictionary<string, Field>
                    {
                        ["ProjectGuid"] = BasicFields.DynamicDropdown("", "sai.actions.runBlockly.options")
                    }
                },
                new BlockMetadata("sai.actions.runActionSet")
                {
                    Kind = BlockKind.Action,
                    Name = "运行可复用的行动组",
                    Icon = ("行动组", FluentIcons.AirplaneTakeOffRegular),
                    Fields = new Dictionary<string, Field>
                    {
                        ["ProjectGuid"] = BasicFields.DynamicDropdown("", "sai.actions.runActionSet.options")
                    }
                }
            ],
            Rules =
            [
                new BlockMetadata("sai.rules.runCiRuleset")
                {
                    Kind = BlockKind.Rule,
                    Name = "运行可复用的规则集",
                    Icon = ("规则集", FluentIcons.TagMultipleRegular),
                    Fields = new Dictionary<string, Field>
                    {
                        ["ProjectGuid"] = BasicFields.DynamicDropdown("", "sai.rules.runCiRuleset.options")
                    }
                },
                new BlockMetadata("sai.rules.dialogs.yesNo")
                {
                    Kind = BlockKind.Rule,
                    Name = "YesNo 对话框",
                    Icon = ("对话框", FluentIcons.WindowHeaderHorizontalRegular),
                    Fields = new Dictionary<string, Field>
                    {
                        ["dummy1"] = BasicFields.Dummy(""),
                        ["Header"] = BasicFields.Text("标题"),
                        ["Message"] = BasicFields.Text("消息"),
                        ["YesText"] = BasicFields.Text("「Yes」按钮文本"),
                        ["NoText"] = BasicFields.Text("「No」按钮文本"),
                        ["PreferYes"] = BasicFields.Boolean("默认按钮为「Yes」？"),
                        ["Topmost"] = BasicFields.Boolean("置顶？"),
                        ["CountdownEnabled"] = BasicFields.Boolean("启用倒计时？"),
                        ["CountdownTime"] = BasicFields.Number("倒计时时长(s)"),
                    }
                }
            ],
            Data =
            [
                new BlockMetadata("sai.data.getDynamicText")
                {
                    Kind = BlockKind.Data,
                    Name = "获取动态文本",
                    Icon = ("文本", FluentIcons.TextboxRegular),
                    Fields = new Dictionary<string, Field>
                    {
                        ["Key"] = BasicFields.Text("ID"),
                    }
                },
                new BlockMetadata("sai.rules.dialogs.text")
                {
                    Kind = BlockKind.Data,
                    Name = "文本输入对话框",
                    Icon = ("对话框", FluentIcons.WindowHeaderHorizontalRegular),
                    Fields = new Dictionary<string, Field>
                    {
                        ["dummy1"] = BasicFields.Dummy(""),
                        ["Header"] = BasicFields.Text("标题"),
                        ["Message"] = BasicFields.Text("消息"),
                        ["DefaultText"] = BasicFields.Text("默认文本"),
                        ["OkText"] = BasicFields.Text("「Ok」按钮文本"),
                        ["CancelText"] = BasicFields.Text("「Cancel」按钮文本"),
                        ["Topmost"] = BasicFields.Boolean("置顶？"),
                        ["CountdownEnabled"] = BasicFields.Boolean("启用倒计时？"),
                        ["CountdownTime"] = BasicFields.Number("倒计时时长(s)"),
                    }
                }
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

        SaiServer.RegisterDataHandler<GetDynamicTextSettings>("sai.data.getDynamicText", data =>
        {
            if (data is not GetDynamicTextSettings settings) return Task.FromResult("???");

            var provider = IAppHost.GetService<DynamicTextProvider>();
            return Task.FromResult(provider.GetText(settings.Key) ?? "[未设置值]");
        });

        SaiServer.RegisterDataHandler<TextDialogDataModel>("sai.rules.dialogs.text", async data =>
        {
            if (data is not TextDialogDataModel model) return "???";
            return await Dispatcher.UIThread.InvokeAsync(async Task<string> () => await ShowDialogAsync(model));
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
            dialog.Closing += (sender, args) => { args.Cancel = !completed; };

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