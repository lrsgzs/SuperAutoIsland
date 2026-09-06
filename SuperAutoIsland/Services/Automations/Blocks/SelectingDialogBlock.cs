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

public class SelectingDialogBlock : DataBlockBase
{
    public override string Id => "sai.data.dialogs.selecting";
    public override string Name => "项目选择对话框";
    public override (string, string) Icon => ("列表", FluentIcons.ListRegular);
    public override Type SettingsType => typeof(SelectingDialogSettings);

    public override void GetFields(FieldsRegister it)
    {
        if (GlobalConstants.Configs.MainConfig!.Data.EnableEasterEggs)
        {
            it
                .AddDummy()
                .AddField("Header", BasicFields.Text("标题", "ChooseOne..."))
                .AddField("Message", BasicFields.Text("消息", "请选择你的心向之物。"))
                .AddField("Items", BasicFields.Dictionary("项目字典"))
                .AddDummy("↑ 键为返回值")
                .AddDummy("↑ 值为显示文本")
                .AddField("Default", BasicFields.Text("默认键(可空)", "sandrone"));
        }
        else
        {
            it
                .AddDummy()
                .AddField("Header", BasicFields.Text("标题", "选择一项..."))
                .AddField("Message", BasicFields.Text("消息", "请选择一个项目。"))
                .AddField("Items", BasicFields.Dictionary("项目字典"))
                .AddDummy("↑ 键为返回值")
                .AddDummy("↑ 值为显示文本")
                .AddField("Default", BasicFields.Text("默认键(可空)", string.Empty));
        }
        
        it
            .AddField("Topmost", BasicFields.Boolean("置顶？", false))
            .AddField("CountdownEnabled", BasicFields.Boolean("启用倒计时？", true))
            .AddField("CountdownTime", BasicFields.Number("倒计时时长(s)", 5));
    }

    public override async Task<object> Handler(object? data)
    {
        if (data is not SelectingDialogSettings model)
            return "???";
        return await Dispatcher.UIThread.InvokeAsync(async Task<string> () => await ShowDialogAsync(model));
    }

    public static async Task<string> ShowDialogAsync(SelectingDialogSettings settings)
    {
        if (settings.Items.Count == 0)
            return "???";
        
        var buttons = settings.Items
            .Select(kvp =>
            {
                var button = new FATaskDialogButton(kvp.Value, kvp.Key);
                if (settings.Default == kvp.Key)
                    button.IsDefault = true;
                return button;
            })
            .ToList();

        var defaultButton = buttons
            .Where(x => x.IsDefault)
            .FirstOrDefault(buttons.Last())!;
        var defaultButtonText = defaultButton.Text;
        
        var dialog = new FATaskDialog
        {
            Title = settings.Header,
            Header = settings.Header,
            Content = settings.Message,
            Buttons = buttons,
            XamlRoot = AppBase.Current.GetRootWindow()
        };

        if (settings.CountdownEnabled)
        {
            var stopwatch = Stopwatch.StartNew();
            var completed = false;
            dialog.Closing += (sender, args) => { args.Cancel = !completed; };
            
            buttons.ForEach(x => x.IsEnabled = false);

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
                        defaultButton.Text = $"{defaultButtonText} ({remainingTime:0}s)";
                    });

                    var checkInterval = Math.Min(remainingMs, 1000);
                    await Task.Delay(checkInterval);
                }

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    completed = true;
                    buttons.ForEach(x => x.IsEnabled = true);
                    defaultButton.Text = defaultButtonText;
                });
            });
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
        
        return result as string ?? "???";
    }
}