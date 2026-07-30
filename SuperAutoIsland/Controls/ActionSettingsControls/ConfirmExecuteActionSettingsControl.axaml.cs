using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using ClassIsland.Core.Abstractions.Controls;
using SuperAutoIsland.Models.Actions;
using SuperAutoIsland.Services.Automations.Actions;

namespace SuperAutoIsland.Controls.ActionSettingsControls;

public partial class ConfirmExecuteActionSettingsControl : ActionSettingsControlBase<ConfirmExecuteActionSettings>
{
    public ConfirmExecuteActionSettings ActionSettings => Settings;
    
    public ConfirmExecuteActionSettingsControl()
    {
        InitializeComponent();

        Loaded += (sender, args) =>
        {
            HeaderTextBlock.Bind(TextBlock.TextProperty,
                CompiledBinding.Create<ConfirmExecuteActionSettings, string>(
                    x => x.Header, Settings));
            MessageTextBlock.Bind(TextBlock.TextProperty,
                CompiledBinding.Create<ConfirmExecuteActionSettings, string>(
                    x => x.Message, Settings));
        };
    }
    
    private async void PreviewButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var result = await ConfirmExecuteAction.ShowDialogAsync(Settings, "占位符");
        if (result == ConfirmExecuteAction.ResultType.Delay)
        {
            await ConfirmExecuteAction.ShowDelayDialogAsync(Settings, "占位符");
        }
    }
    
    private void ShowSettingsButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.FindResource("SettingsDrawer") is not ContentControl cc) return;
        cc.DataContext = this;
        _ = ShowDrawer(cc);
    }
}