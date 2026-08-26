using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using ClassIsland.Core;
using ClassIsland.Core.Icons;
using FluentAvalonia.UI.Controls;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Models.Data;
using SuperAutoIsland.Shared;

namespace SuperAutoIsland.Services.Automations.Blocks;

public class TextDialogBlock : DataBlockBase
{
    // typo. cannot fix
    public override string Id => "sai.rules.dialogs.text";
    public override string Name => "文本输入对话框";
    public override (string, string) Icon => ("对话框", FluentIcons.WindowHeaderHorizontalRegular);
    public override Type SettingsType => typeof(TextDialogDataModel);

    public override void GetFields(FieldsRegister it)
    {
        if (GlobalConstants.Configs.MainConfig!.Data.EnableEasterEggs)
        {
            it
                .AddDummy()
                .AddField("Header", BasicFields.Text("标题", "运行世界式..."))
                .AddField("Message", BasicFields.Text("消息", "请输入要运行的世界式。"))
                .AddField("DefaultText", BasicFields.Text("默认文本", "世界式·反转「归约」"));
        }
        else
        {
            it
                .AddDummy()
                .AddField("Header", BasicFields.Text("标题", "输入文本..."))
                .AddField("Message", BasicFields.Text("消息", "请输入文本。"))
                .AddField("DefaultText", BasicFields.Text("默认文本"));
        }
        
        it
            .AddField("OkText", BasicFields.Text("「Ok」按钮文本", "确定"))
            .AddField("CancelText", BasicFields.Text("「Cancel」按钮文本", "取消"))
            .AddField("Topmost", BasicFields.Boolean("置顶？", false))
            .AddField("CountdownEnabled", BasicFields.Boolean("启用倒计时？", true))
            .AddField("CountdownTime", BasicFields.Number("倒计时时长(s)", 5));
    }

    public override async Task<object> Handler(object? data)
    {
        if (data is not TextDialogDataModel model)
            return "???";
        return await Dispatcher.UIThread.InvokeAsync(async Task<string> () => await ShowDialogAsync(model));
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
}