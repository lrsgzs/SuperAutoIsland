using Avalonia.Data.Converters;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Helpers.UI;
using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using SuperAutoIsland.Shared.Logger;
using SuperAutoIsland.ViewModel;

namespace SuperAutoIsland.Views;

/// <summary>
/// 日志窗口视图
/// </summary>
public partial class SaiLogsView : ViewBase
{
    /// <summary>
    /// 等级-图标 转换器
    /// </summary>
    public static readonly FuncValueConverter<string, string> LogLevelToIconGlyphConverter = new(x => x switch
    {
        "ERROR" => FluentIcons.ErrorCircleFilled,
        "WARN" => FluentIcons.WarningFilled,
        "INFO" => FluentIcons.InfoRegular,
        "DEBUG" => FluentIcons.BugFilled,
        _ => FluentIcons.PresenceDndFilled
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
    private Logger<SaiLogsView> _logger = new();
    
    public SaiLogsView()
    {
        InitializeComponent();
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
            TopLevel?.Clipboard?.SetTextAsync(string.Join('\n', logs));
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
