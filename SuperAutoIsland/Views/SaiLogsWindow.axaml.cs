using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using ClassIsland.Core.Controls;
using ClassIsland.Core.Helpers.UI;
using ClassIsland.Shared;
using SuperAutoIsland.Shared.Logger;
using SuperAutoIsland.ViewModel;

namespace SuperAutoIsland.Views;

/// <summary>
/// 日志窗口视图
/// </summary>
public partial class SaiLogsWindow : MyWindow
{
    /// <summary>
    /// 等级-图标 转换器
    /// </summary>
    public static readonly FuncValueConverter<string, string> LogLevelToIconGlyphConverter = new(x => x switch
    {
        "ERROR" => "\uE808",
        "WARN" => "\uF430",
        "INFO" => "\uE9E4",
        "DEBUG" => "\uE2C7",
        _ => "\uEDF6"
    });
    
    /// <summary>
    /// 等级-可读文字 转换器
    /// </summary>
    public static readonly FuncValueConverter<string, string> LogLevelToNameConverter = new(x => x switch
    {
        "ERROR" => "错误",
        "WARN" => "警告",
        "INFO" => "信息",
        "DEBUG" => "调试",
        _ => $"其他[{x}]"
    });
    
    public SaiLogsViewModel ViewModel { get; } = IAppHost.GetService<SaiLogsViewModel>();
    private Logger<SaiLogsWindow> _logger = new();
    private bool _isOpened = false;
    
    public SaiLogsWindow()
    {
        InitializeComponent();
    }
    
    /// <summary>
    /// 打开窗口
    /// </summary>
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
    
    /// <summary>
    /// 清理日志点击事件
    /// </summary>
    private void ButtonClearLogs_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.RootLogger.ClearLogs();
    }

    /// <summary>
    /// 复制选中的日志点击事件
    /// </summary>
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
