using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using ClassIsland.Core.Controls;
using ClassIsland.Core.Helpers.UI;
using ClassIsland.Shared;
using Microsoft.Extensions.Logging;
using SuperAutoIsland.Shared.Logger;
using SuperAutoIsland.ViewModel;

namespace SuperAutoIsland.Controls.Windows;

public partial class SaiLogsWindow : MyWindow
{
    public static readonly FuncValueConverter<string, string> LogLevelToIconGlyphConverter = new(x => x switch
    {
        "ERROR" => "\uE808",
        "WARN" => "\uF430",
        "INFO" => "\uE9E4",
        "DEBUG" => "\uE2C7",
        _ => "\uEDF6;"
    });
    
    public static readonly FuncValueConverter<string, string> LogLevelToNameConverter = new(x => x switch
    {
        "ERROR" => "错误",
        "WARN" => "警告",
        "INFO" => "信息",
        "DEBUG" => "调试",
        _ => $"其他[{x}]"
    });
    
    public SaiLogsViewModel ViewModel { get; } = IAppHost.GetService<SaiLogsViewModel>();
    private Logger _logger = new Logger("SaiLogsWindow");
    private bool _isOpened = false;
    
    public SaiLogsWindow()
    {
        InitializeComponent();
    }

    public void Open()
    {
        if (!_isOpened)
        {
            _isOpened = true;
            Show();
        }
        else
        {
            if (WindowState == WindowState.Minimized)
            {
                WindowState = WindowState.Normal;
            }

            Activate();
        }
    }
    
    private void ButtonClearLogs_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.RootLogger.ClearLogs();
    }

    private void ButtonCopySelectedLogs_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var logs = DataGridMain.SelectedItems.Cast<object?>().Select(x => x?.ToString() ?? "").ToList();
            Clipboard?.SetTextAsync(string.Join('\n', logs));
            this.ShowSuccessToast($"已将 {logs.Count} 条日志复制到剪贴板。");
        }
        catch (Exception ex)
        {
            _logger.Error("无法复制日志到剪切板。");
            _logger.FormatException(ex);
            this.ShowErrorToast("无法复制日志到剪切板。", ex);
        }
    }
}
