using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Threading;
using ClassIsland.Core;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using FluentAvalonia.UI.Controls;
using SuperAutoIsland.Enums;
using SuperAutoIsland.Models.Rules;
using SuperAutoIsland.Shared;

namespace SuperAutoIsland.Services.Automations;

public class RuleHandlerService
{
    private readonly IRulesetService _rulesetService = IAppHost.GetService<IRulesetService>();
    private readonly ILessonsService _lessonsService = IAppHost.GetService<ILessonsService>();

    public RuleHandlerService()
    {
        _rulesetService.RegisterRuleHandler("sai.rules.runCiRuleset", settings =>
        {
            if (settings is not RunCiRulesetSettings s) return false;

            var ciRunner = IAppHost.GetService<CiRunner>();
            
            if (s.ProjectGuid == GlobalConstants.Assets.ProjectNullGuid)
                return false;
            
            var project = ProjectsConfigManager.GetProject(s.ProjectGuid);
            if (project.RulesetState != null)
            {
                return project.RulesetState.Value;
            }
            
            var state = ciRunner.RunRulesetProject(project);
            project.RulesetState = state;
            _rulesetService.StatusUpdated += ClearState;
            
            return state;
            
            void ClearState(object? sender, EventArgs e)
            {
                project.RulesetState = null;
                _rulesetService.StatusUpdated -= ClearState;
            }
        });
        
        _rulesetService.RegisterRuleHandler("sai.rules.dialogs.yesNo", obj =>
        {
            if (obj is not YesNoDialogRuleSettings settings) return false;

            if (settings is { ShowOnce: true, Showed: true })
            {
                return settings.LastResult;
            }

            var tcs = new TaskCompletionSource<bool>();
            Dispatcher.UIThread.Post(async void () =>
            {
                try
                {
                    var result = await ShowDialogAsync(settings);
                    tcs.SetResult(Equals(result, true));
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });

            var frame = new DispatcherFrame();
            tcs.Task.ContinueWith(_ => frame.Continue = false, TaskScheduler.Default);
            Dispatcher.UIThread.PushFrame(frame);

            var result = tcs.Task.Result;
            settings.LastResult = result;
            settings.Showed = true;
            return result;
        });
    }

    public static async Task<bool> ShowDialogAsync(YesNoDialogRuleSettings settings)
    {
        var noButton = new FATaskDialogButton(settings.NoText, false)
        {
            IsDefault = !settings.PreferYes,
        };
        
        var yesButton = new FATaskDialogButton(settings.YesText, true)
        {
            IsDefault = settings.PreferYes
        };
        
        var dialog = new FATaskDialog
        {
            Title = settings.Header,
            Header = settings.Header,
            Content = settings.Message,
            Buttons = settings.PreferYes ? [noButton, yesButton] : [yesButton, noButton],
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
                CountdownMode.Enable => settings.PreferYes ? settings.YesText : settings.NoText,
                CountdownMode.Resolve => settings.YesText,
                _ => settings.NoText
            };
            
            var completed = false;
            
            if (settings.CountdownMode == CountdownMode.Enable)
            {
                dialog.Closing += (sender, args) =>
                {
                    args.Cancel = !completed;
                };
                
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
        return Equals(result, true);
    }
}