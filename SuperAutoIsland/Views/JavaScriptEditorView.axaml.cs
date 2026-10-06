using System.Diagnostics;
using System.Xml;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Threading;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Highlighting.Xshd;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Helpers.UI;
using ClassIsland.Core.Models.UI;
using ClassIsland.Shared;
using FluentAvalonia.UI.Controls;
using SuperAutoIsland.Enums;
using SuperAutoIsland.Models;
using SuperAutoIsland.Services;
using SuperAutoIsland.Services.BlocklyRunner;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Views;

/// <summary>
///     「JavaScript 编辑器」视图。
/// </summary>
public partial class JavaScriptEditorView : ViewBase
{
    /// <summary>
    ///     运行输出面板最多保留的行数。
    /// </summary>
    private const int MaxLogLines = 500;

    private const string SaveAndExitResult = "save";
    private const string DiscardAndExitResult = "discard";
    private const string CancelExitResult = "cancel";

    /// <summary>
    ///     随插件打包的深色高亮定义名（见 Assets/Highlighting/JavaScript-Dark.xshd）。
    /// </summary>
    private const string DarkHighlightingName = "JavaScript (Dark)";

    private const string DarkEditorBackground = "#1E1E1E";
    private const string DarkEditorForeground = "#D4D4D4";
    private const string LightEditorBackground = "#FFFFFF";
    private const string LightEditorForeground = "#1F1F1F";

    private readonly Logger<JavaScriptEditorView> _logger = new();
    private readonly BlocklyRunner _blocklyRunner = IAppHost.GetService<BlocklyRunner>();
    private readonly List<string> _logLines = [];

    private Project? _project;
    private bool _isUpdatingEditor;
    private bool _isDirty;
    private bool _isRunning;
    private bool _isExitConfirmed;
    private bool _isExitConfirmationShowing;
    private CancellationTokenSource? _runCancellation;

    public JavaScriptEditorView()
    {
        InitializeComponent();

        ApplyHighlighting();
        ScriptEditor.TextChanged += ScriptEditor_OnTextChanged;
        Closing += OnClosing;

        // 视图被关闭时掐掉还在跑的脚本，避免后台继续跑
        Closed += (_, _) => _runCancellation?.Cancel();
    }

    /// <summary>
    ///     应用 JavaScript 语法高亮与配套的编辑器配色。
    ///     <para>
    ///         优先用随插件打包的深色定义（VS Code Dark+ 配色，配深色底）；
    ///         资源缺失或解析失败时退回 AvaloniaEdit 内置的浅色定义配浅色底。
    ///     </para>
    /// </summary>
    private void ApplyHighlighting()
    {
        if (TryGetDarkHighlighting() is { } dark)
        {
            ScriptEditor.SyntaxHighlighting = dark;
            SetEditorColors(DarkEditorBackground, DarkEditorForeground);
            return;
        }

        ScriptEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("JavaScript");
        SetEditorColors(LightEditorBackground, LightEditorForeground);
    }

    /// <summary>
    ///     取得深色高亮定义，首次调用时从插件资源里加载并注册。
    /// </summary>
    private IHighlightingDefinition? TryGetDarkHighlighting()
    {
        // 高亮定义是全局注册的，已注册过就直接复用
        if (HighlightingManager.Instance.GetDefinition(DarkHighlightingName) is { } registered)
        {
            return registered;
        }

        try
        {
            using var stream = AssetLoader.Open(
                new Uri("avares://SuperAutoIsland/Assets/Highlighting/JavaScript-Dark.xshd"));
            using var reader = XmlReader.Create(stream);
            var definition = HighlightingLoader.Load(reader, HighlightingManager.Instance);
            HighlightingManager.Instance.RegisterHighlighting(DarkHighlightingName, [".js"], definition);
            return definition;
        }
        catch (Exception exception)
        {
            _logger.Error("加载 JavaScript 深色高亮定义失败，将改用内置的浅色定义。");
            _logger.FormatException(exception);
            return null;
        }
    }

    private void SetEditorColors(string background, string foreground)
    {
        ScriptEditor.Background = new SolidColorBrush(Color.Parse(background));
        ScriptEditor.Foreground = new SolidColorBrush(Color.Parse(foreground));
    }

    /// <summary>
    ///     载入要编辑的 JavaScript 行动项目。
    /// </summary>
    /// <param name="project">要编辑的项目</param>
    /// <exception cref="ArgumentException">项目类型不是 JavaScript 行动时抛出</exception>
    public void LoadProject(Project project)
    {
        if (project.Type is not ProjectsType.JavaScriptAction)
        {
            throw new ArgumentException("只能编辑 JavaScript 行动的脚本。", nameof(project));
        }

        // 同一视图会被复用：切换到另一个项目前先把当前未保存的改动落盘，避免直接丢弃
        if (_isDirty && _project is not null && !ReferenceEquals(_project, project))
        {
            _logger.Info($"切换到项目 {project.Name}，先保存上一个项目的改动。");
            SaveScript();
        }

        _project = project;
        _isExitConfirmed = false;
        Header = $"JavaScript 编辑器 - {project.Name}";

        var path = ProjectsConfigManager.GetJavaScriptJsPath(project);
        ScriptPathText.Text = path;
        ToolTip.SetTip(ScriptPathText, path);

        _isUpdatingEditor = true;
        try
        {
            ScriptEditor.Text = ProjectsConfigManager.LoadJavaScriptProjectJs(project);
            SetDirty(false);
        }
        finally
        {
            _isUpdatingEditor = false;
        }

        StatusText.Text = "就绪";
    }

    /// <summary>
    ///     保存脚本到文件。
    /// </summary>
    /// <returns>是否保存成功</returns>
    private bool SaveScript()
    {
        if (_project is null)
        {
            return false;
        }

        try
        {
            ProjectsConfigManager.SaveJavaScriptProject(_project, ScriptEditor.Text ?? string.Empty);
            SetDirty(false);
            StatusText.Text = "已保存";
            return true;
        }
        catch (Exception exception)
        {
            _logger.Error("保存 JavaScript 脚本失败。");
            _logger.FormatException(exception);
            StatusText.Text = "保存失败";
            this.ShowErrorToast("保存脚本失败。", exception);
            return false;
        }
    }

    /// <summary>
    ///     标记（或清除）未保存状态。
    /// </summary>
    private void SetDirty(bool isDirty)
    {
        _isDirty = isDirty;
        DirtyIndicator.IsVisible = isDirty;
    }

    /// <summary>
    ///     运行当前脚本，运行日志会回流到「运行输出」面板。
    /// </summary>
    private async Task RunScriptAsync()
    {
        if (_project is null || _isRunning)
        {
            return;
        }

        _isRunning = true;
        SaveAndRunButton.IsEnabled = false;
        StopButton.IsEnabled = true;
        LogExpander.IsExpanded = true;

        // 每次运行都从干净的输出开始
        ClearLog();
        AppendLog("INFO", $"开始运行 {_project.Name}");

        var cancellation = new CancellationTokenSource();
        _runCancellation = cancellation;
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _blocklyRunner.RunJavaScriptProject(_project, cancellation.Token,
                (level, message) => Dispatcher.UIThread.Post(() => AppendLog(level, message)));

            StatusText.Text = $"运行完成（{stopwatch.ElapsedMilliseconds} ms）";
            AppendLog("INFO", $"运行完成，用时 {stopwatch.ElapsedMilliseconds} ms");
        }
        catch (OperationCanceledException)
        {
            StatusText.Text = "运行已停止";
            AppendLog("WARN", "运行已停止");
        }
        catch (Exception exception)
        {
            _logger.Error("运行 JavaScript 脚本失败。");
            _logger.FormatException(exception);
            StatusText.Text = "运行失败";
            AppendLog("ERROR", exception.Message);
        }
        finally
        {
            if (ReferenceEquals(_runCancellation, cancellation))
            {
                _runCancellation = null;
            }

            cancellation.Dispose();
            _isRunning = false;
            SaveAndRunButton.IsEnabled = true;
            StopButton.IsEnabled = false;
        }
    }

    /// <summary>
    ///     向运行输出面板追加一行日志。
    /// </summary>
    private void AppendLog(string level, string message)
    {
        _logLines.Add($"[{level}] {message}");
        if (_logLines.Count > MaxLogLines)
        {
            _logLines.RemoveRange(0, _logLines.Count - MaxLogLines);
        }

        LogTextBox.Text = string.Join('\n', _logLines);
        LogTextBox.CaretIndex = LogTextBox.Text.Length;
    }

    /// <summary>
    ///     清空运行输出。
    /// </summary>
    private void ClearLog()
    {
        _logLines.Clear();
        LogTextBox.Text = string.Empty;
    }

    /// <summary>
    ///     窗口关闭拦截：有未保存的改动时询问是否保存。
    ///     <para>
    ///         事件是同步调用的，因此先取消本次关闭，再把询问过程 Post 到 UI 线程稍后执行
    ///         （与 ClassIsland 的 <c>TutorialEditorWindow</c> 一致，避免在关闭流程里直接开新窗口）。
    ///     </para>
    /// </summary>
    private void OnClosing(object? sender, ViewClosingEventArgs e)
    {
        // 已确认退出，或本次关闭不可取消（例如应用退出、系统关机）时直接放行
        if (_isExitConfirmed || !e.IsCancelable)
        {
            return;
        }

        // 没有未保存的改动就不需要询问
        if (!_isDirty)
        {
            return;
        }

        e.Cancel = true;
        Dispatcher.UIThread.Post(async () => await ConfirmExitAsync());
    }

    /// <summary>
    ///     弹出询问是否保存的 TaskDialog，并据此决定保存、丢弃或留在编辑器里。
    ///     <para>
    ///         用 TaskDialog 而不是 ContentDialog：ContentDialog 挂在当前窗口的 OverlayLayer 上，
    ///         在有 WebView 的窗口里会被盖住；TaskDialog 在桌面端以独立窗口弹出。
    ///         拿不到可视根或弹窗失败时保持窗口打开（此时按「保存」按钮手动保存后即可正常退出）。
    ///     </para>
    /// </summary>
    private async Task ConfirmExitAsync()
    {
        if (_isExitConfirmationShowing)
        {
            return;
        }

        var root = TopLevel;
        if (root == null)
        {
            _logger.Warn("无法获取可视根，跳过保存确认。");
            _isExitConfirmed = true;
            return;
        }

        _isExitConfirmationShowing = true;
        try
        {
            var result = await new FATaskDialog
            {
                XamlRoot = root,
                Title = "脚本尚未保存",
                Header = "脚本尚未保存",
                Content = "脚本还有未保存的改动，退出前要保存吗？",
                Buttons =
                [
                    new FATaskDialogButton("保存", SaveAndExitResult)
                    {
                        IsDefault = true
                    },
                    new FATaskDialogButton("不保存", DiscardAndExitResult),
                    new FATaskDialogButton("取消", CancelExitResult)
                ]
            }.ShowAsync();

            // 取消，或直接关掉了对话框窗口：留在编辑器
            if (Equals(result, CancelExitResult) || result is FATaskDialogStandardResult.None)
            {
                return;
            }

            // 保存失败时不退出，避免丢掉改动
            if (Equals(result, SaveAndExitResult) && !SaveScript())
            {
                return;
            }

            _isExitConfirmed = true;
            Close();
        }
        catch (Exception exception)
        {
            _logger.Error("询问是否保存脚本时出现异常。");
            _logger.FormatException(exception);
            this.ShowErrorToast("无法弹出保存确认对话框，请先手动保存再退出。", exception);
        }
        finally
        {
            _isExitConfirmationShowing = false;
        }
    }

    private void ScriptEditor_OnTextChanged(object? sender, EventArgs e)
    {
        if (_isUpdatingEditor)
        {
            return;
        }

        SetDirty(true);
    }

    private void SaveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        SaveScript();
    }

    private async void SaveAndRunButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (!SaveScript())
        {
            return;
        }

        await RunScriptAsync();
    }

    private void StopButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_runCancellation is null)
        {
            return;
        }

        StatusText.Text = "正在停止…";
        _runCancellation.Cancel();
    }

    private void ClearLogButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ClearLog();
    }
}
