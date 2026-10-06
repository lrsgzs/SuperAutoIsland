using Avalonia.Threading;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Models.UI;
using FluentAvalonia.UI.Controls;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Views;

/// <summary>
///     「Blockly 编辑器」视图。
/// </summary>
public partial class BlocklyEditorView : ViewBase
{
    private const string ExitConfirmedResult = "exit";
    private const string ExitCancelledResult = "cancel";

    private readonly Logger<BlocklyEditorView> _logger = new();

    private bool _isExitConfirmed;
    private bool _isExitConfirmationShowing;

    public BlocklyEditorView()
    {
        InitializeComponent();
        Closing += OnClosing;
    }

    public void LoadEditor(Uri uri, string? title = null)
    {
        Header = string.IsNullOrWhiteSpace(title) ? "Blockly 编辑器" : $"Blockly 编辑器 - {title}";
        EditorWebView.Source = uri;
    }

    private void OnClosing(object? sender, ViewClosingEventArgs e)
    {
        if (OperatingSystem.IsAndroid() || _isExitConfirmed || !e.IsCancelable)
        {
            return;
        }

        e.Cancel = true;
        Dispatcher.UIThread.Post(() => _ = ConfirmExitAsync());
    }

    private async Task ConfirmExitAsync()
    {
        if (_isExitConfirmationShowing)
        {
            return;
        }

        var editorWindow = TopLevel;
        if (editorWindow == null)
        {
            _isExitConfirmed = true;
            return;
        }

        _isExitConfirmationShowing = true;
        try
        {
            var result = await new FATaskDialog
            {
                XamlRoot = editorWindow,
                Title = "退出 Blockly 编辑器",
                Content = "确定要退出 Blockly 编辑器吗？未保存的改动将会丢失。",
                Buttons =
                [
                    new FATaskDialogButton("退出", ExitConfirmedResult),
                    new FATaskDialogButton("取消", ExitCancelledResult)
                    {
                        IsDefault = true
                    }
                ]
            }.ShowAsync();

            if (!Equals(result, ExitConfirmedResult))
            {
                return;
            }

            _isExitConfirmed = true;
            Close();
        }
        catch (Exception exception)
        {
            _logger.Error("询问是否退出 Blockly 编辑器时出现异常。");
            _logger.FormatException(exception);
        }
        finally
        {
            _isExitConfirmationShowing = false;
        }
    }
}
