using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using ClassIsland.Core;
using ClassIsland.Core.Abstractions.Automation;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Controls;
using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using FluentAvalonia.UI.Controls;
using SuperAutoIsland.Enums;
using SuperAutoIsland.Models.Actions;

namespace SuperAutoIsland.Services.Automations.Actions;

[ActionInfo("sai.actions.dialogs.confirmExecute", "工作流执行确认", FluentIcons.AirplaneLandingRegular, false)]
public class ConfirmExecuteAction : ActionBase<ConfirmExecuteActionSettings>
{
    private IActionService? _actionService;
    
    protected override async Task OnInvoke()
    {
        await base.OnInvoke();
        _actionService ??= IAppHost.GetService<IActionService>();

        var result = await ShowDialogAsync(Settings, ActionSet.Name);
        if (result == ResultType.No)
        {
            _ = _actionService.InterruptActionSetAsync(ActionSet);
        }
        else if (result == ResultType.Delay)
        {
            var delayTime = await ShowDelayDialogAsync(Settings, ActionSet.Name);
            if (delayTime == null)
            {
                _ = _actionService.InterruptActionSetAsync(ActionSet);
                return;
            }

            await Task.Delay(TimeSpan.FromSeconds(delayTime.Value));
        }
    }
    
    public static async Task<ResultType> ShowDialogAsync(ConfirmExecuteActionSettings settings, string actionSetName)
    {
        var delayButton = new FATaskDialogButton(settings.DelayText.Replace("{name}", actionSetName), "delay");
        
        var noButton = new FATaskDialogButton(settings.NoText.Replace("{name}", actionSetName), "no")
        {
            IsDefault = !settings.PreferYes,
        };
        
        var yesButton = new FATaskDialogButton(settings.YesText.Replace("{name}", actionSetName), "yes")
        {
            IsDefault = settings.PreferYes
        };
        
        var dialog = new FATaskDialog
        {
            Title = settings.Header.Replace("{name}", actionSetName),
            Header = settings.Header.Replace("{name}", actionSetName),
            Content = settings.Message.Replace("{name}", actionSetName),
            Buttons = settings.CanDelay ?
                (settings.PreferYes ? [delayButton, noButton, yesButton] : [delayButton, yesButton, noButton]) :
                (settings.PreferYes ? [noButton, yesButton] : [yesButton, noButton]),
            XamlRoot = AppBase.Current.GetRootWindow()
        };

        if (settings.CountdownEnabled)
        {
            var stopwatch = Stopwatch.StartNew();
            
            var countdownButton = settings.CountdownMode switch
            {
                CountdownMode.Enable => settings.PreferYes ? yesButton : noButton,
                CountdownMode.Resolve => yesButton,
                _ => noButton
            };
            var countdownOriginText = settings.CountdownMode switch
            {
                CountdownMode.Enable =>
                    (settings.PreferYes ? settings.YesText : settings.NoText)
                    .Replace("{name}", actionSetName),
                CountdownMode.Resolve => settings.YesText.Replace("{name}", actionSetName),
                _ => settings.NoText.Replace("{name}", actionSetName)
            };
            
            var completed = false;
            
            if (settings.CountdownMode == CountdownMode.Enable)
            {
                dialog.Closing += (sender, args) =>
                {
                    args.Cancel = !completed;
                };

                delayButton.IsEnabled = false;
                noButton.IsEnabled = false;
                yesButton.IsEnabled = false;
            }

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
                        countdownButton.Text = $"{countdownOriginText} ({remainingTime:0}s)";
                    });

                    var checkInterval = Math.Min(remainingMs, 1000);
                    await Task.Delay(checkInterval);
                }
                
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    completed = true;

                    countdownButton.Text = countdownOriginText;
                    
                    switch (settings.CountdownMode)
                    {
                        case CountdownMode.Enable:
                            delayButton.IsEnabled = true;
                            noButton.IsEnabled = true;
                            yesButton.IsEnabled = true;
                            break;
                        case CountdownMode.Resolve:
                            dialog.Hide(true);
                            break;
                        case CountdownMode.Reject:
                            dialog.Hide(false);
                            break;
                        default:
                            dialog.Hide();
                            break;
                    }
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
        var text = result as string ?? "???";
        
        return text switch
        {
            "yes" => ResultType.Yes,
            "no" => ResultType.No,
            "delay" => ResultType.Delay,
            _ => ResultType.No
        };
    }

    public static async Task<double?> ShowDelayDialogAsync(ConfirmExecuteActionSettings settings, string actionSetName)
    {
        var cancelButton = new FATaskDialogButton("取消执行", false);
        
        var okButton = new FATaskDialogButton("确定", true)
        {
            IsDefault = true
        };

        var numberBox = new FANumberBox
        {
            Value = settings.DefaultDelayTime,
            Minimum = 0,
            Maximum = 60,
            SmallChange = 1,
            LargeChange = 5,
            SpinButtonPlacementMode = FANumberBoxSpinButtonPlacementMode.Inline,
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
                        Text = settings.DelayDescription.Replace("{name}", actionSetName),
                        TextWrapping = TextWrapping.Wrap
                    },
                    new Field
                    {
                        Content = numberBox,
                        Suffix = "s"
                    }
                }
            },
            Buttons = [cancelButton, okButton],
            XamlRoot = AppBase.Current.GetRootWindow()
        };
        
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
        
        return Equals(result, true) ? numberBox.Value : null;
    }

    public enum ResultType
    {
        Yes,
        No,
        Delay
    }
}