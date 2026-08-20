using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.VisualTree;
using ClassIsland.Core.Abstractions.Controls;
using FluentAvalonia.UI.Controls;
using SuperAutoIsland.Models.Rules;
using SuperAutoIsland.Services;
using SuperAutoIsland.Services.Automations;

namespace SuperAutoIsland.Controls.RuleSettingsControls;

public partial class YesNoDialogRuleSettingsControl : RuleSettingsControlBase<YesNoDialogRuleSettings>
{
    public YesNoDialogRuleSettingsControl()
    {
        InitializeComponent();
    }

    private void PreviewButton_OnClick(object? sender, RoutedEventArgs e)
    {
        _ = RuleHandlerService.ShowDialogAsync(Settings);
    }
    
    private void ShowSettingsButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.FindResource("SettingsDrawer") is not ContentControl cc) return;
        cc.DataContext = this;
        _ = ShowDrawer(cc);
    }

    private async Task ShowDrawer(Control control, bool isOpenInDialog = false)
    {
        if (!isOpenInDialog &&
            this.FindAncestorOfType<ViewBase>()?.GetType().FullName == "ClassIsland.Views.SettingsWindowNew")
        {
            control.Classes.Remove("in-dialog");
            control.Classes.Add("in-drawer");
            
            if (control is ContentControl cc)
            {
                cc.Padding = new Thickness(16);
            }
            else
            {
                control.Margin = new Thickness(16);
            }
            
            SettingsPageBase.OpenDrawerCommand.Execute(control);
        }
        else
        {
            control.Classes.Remove("in-drawer");
            control.Classes.Add("in-dialog");

            if (control.Parent is FAContentDialog contentDialog)
            {
                contentDialog.Content = null;
            }

            var dialog = new FAContentDialog
            {
                Content = control,
                TitleTemplate = new DataTemplate(),
                PrimaryButtonText = "确定",
                DefaultButton = FAContentDialogButton.Primary,
                DataContext = this
            };

            await dialog.ShowAsync(TopLevel.GetTopLevel(this));
        }
    }
}