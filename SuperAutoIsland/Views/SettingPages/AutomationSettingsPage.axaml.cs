using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Helpers.UI;
using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using CommunityToolkit.Mvvm.Input;
using SuperAutoIsland.Enums;
using SuperAutoIsland.Models;
using SuperAutoIsland.Services;
using SuperAutoIsland.Services.BlocklyRunner;
using SuperAutoIsland.Shared;
using SuperAutoIsland.Shared.Logger;
using SuperAutoIsland.ViewModel.SettingPages;
using SuperAutoIsland.Views;

namespace SuperAutoIsland.Views.SettingPages;

/// <summary>
///     项目类型节点
/// </summary>
public class ProjectTypeNode
{
    /// <summary>
    ///     类型
    /// </summary>
    public ProjectsType Type { get; set; } = ProjectsType.BlocklyAction;

    /// <summary>
    ///     名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     图标
    /// </summary>
    public string IconGlyph { get; set; } = string.Empty;

    /// <summary>
    ///     工具提示
    /// </summary>
    public string ToolTip { get; set; } = string.Empty;
}

/// <summary>
///     「SuperAutoIsland 自动化」视图
/// </summary>
[HidePageTitle]
[FullWidthPage]
[Group("sai.settings")]
[SettingsPageInfo("sai.settings.automation", "自动化", FluentIcons.PlayCircleSparkleRegular,
                  FluentIcons.PlayCircleSparkleFilled)]
public partial class AutomationSettingsPage : SettingsPageBase
{
    /// <summary>
    ///     类型-字符串转换器
    /// </summary>
    public static readonly FuncValueConverter<ProjectsType, string> ProjectsTypeNameConverter = new(x => x switch
    {
        ProjectsType.BlocklyAction    => "Blockly 行动",
        ProjectsType.JavaScriptAction => "JavaScript 行动",
        ProjectsType.CiRuleset        => "可复用的规则集",
        ProjectsType.CiActionSet      => "可复用的行动组",
        _                             => "未知"
    });

    private readonly BlocklyRunner _blocklyRunner = IAppHost.GetService<BlocklyRunner>();
    private readonly CiRunner _ciRunner = IAppHost.GetService<CiRunner>();
    private readonly Logger<AutomationSettingsPage> _logger = new();

    public AutomationSettingsPage()
    {
        if (GlobalConstants.Configs.MainConfig!.Data.EnableEasterEggs)
        {
            ProjectTypeNodes[0].ToolTip = "析构万理的 Blockly 先生";
            ProjectTypeNodes[1].ToolTip = "闪耀千星的 JS 女士";
        }

        DataContext = this;
        InitializeComponent();
    }

    public AutomationViewModel ViewModel { get; } = IAppHost.GetService<AutomationViewModel>();

    /// <summary>
    ///     是否为桌面端。安卓端没有文件关联与外部编辑器，相关入口（用默认程序打开、打开所在文件夹）会隐藏。
    /// </summary>
    public bool IsDesktop { get; } = !OperatingSystem.IsAndroid();

    public ProjectTypeNode[] ProjectTypeNodes { get; } =
    [
        new()
        {
            Type = ProjectsType.BlocklyAction,
            Name = "Blockly 行动",
            IconGlyph = FluentIcons.AlignSpaceEvenlyVerticalRegular,
            ToolTip = "更自由的自动化行动"
        },
        new()
        {
            Type = ProjectsType.JavaScriptAction,
            Name = "JavaScript 行动",
            IconGlyph = FluentIcons.JavascriptRegular,
            ToolTip = "代码化的自动化行动"
        },
        new()
        {
            Type = ProjectsType.CiRuleset,
            Name = "可复用的规则集",
            IconGlyph = FluentIcons.TagMultipleRegular,
            ToolTip = "快速复用同套规则集"
        },
        new()
        {
            Type = ProjectsType.CiActionSet,
            Name = "可复用的行动组",
            IconGlyph = FluentIcons.AirplaneTakeOffRegular,
            ToolTip = "快速复用同套行动组"
        }
    ];

    private void ProjectsListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        ViewModel.IsPanelOpened = true;
    }

    /// <summary>
    ///     创建项目命令
    /// </summary>
    [RelayCommand]
    private void CreateProject(ProjectsType type)
    {
        switch (type)
        {
            case ProjectsType.BlocklyAction:
                ViewModel.SelectedProject =
                    ProjectsConfigManager.CreateProject(ProjectsType.BlocklyAction, "新 Blockly 行动");
                break;
            case ProjectsType.JavaScriptAction:
                ViewModel.SelectedProject =
                    ProjectsConfigManager.CreateProject(ProjectsType.JavaScriptAction, "新 JavaScript 行动");
                break;
            case ProjectsType.CiRuleset:
                ViewModel.SelectedProject = ProjectsConfigManager.CreateProject(ProjectsType.CiRuleset, "新可复用的规则集");
                break;
            case ProjectsType.CiActionSet:
                ViewModel.SelectedProject = ProjectsConfigManager.CreateProject(ProjectsType.CiActionSet, "新可复用的行动组");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    /// <summary>
    ///     打开项目编辑器点击事件。
    ///     <para>
    ///         启用「应用内编辑器」时，以非模态的 <see cref="BlocklyEditorView" /> 展示编辑器前端界面；
    ///         否则沿用系统浏览器打开。
    ///     </para>
    /// </summary>
    private void OpenProjectEditorButton_Click(object? sender, RoutedEventArgs e)
    {
        var project = ViewModel.SelectedProject;
        if (project == null)
        {
            return;
        }

        var uri = new Uri($"http://localhost:{GlobalConstants.Configs.MainConfig!.Data.ServerPort}/" +
                          $"?id={project.Id}");

        if (GlobalConstants.Configs.MainConfig!.Data.EnableInAppBlocklyEditor && TryOpenInAppEditor(uri, project.Name))
        {
            return;
        }

        IAppHost.TryGetService<IUriNavigationService>()?.NavigateWrapped(uri);
    }

    /// <summary>
    ///     通过「应用内编辑器」视图（非模态）打开项目编辑器。
    /// </summary>
    /// <param name="uri">编辑器前端地址</param>
    /// <param name="projectName">项目名称，用于窗口标题</param>
    /// <returns>是否成功打开。失败时返回 <c>false</c>，由调用方回退到浏览器打开。</returns>
    private bool TryOpenInAppEditor(Uri uri, string projectName)
    {
        try
        {
            var editorView = IAppHost.GetService<BlocklyEditorView>();
            editorView.LoadEditor(uri, projectName);
            editorView.Open();
            return true;
        }
        catch (Exception exception)
        {
            // 例如平台缺少 WebView 后端（Windows 上未安装 WebView2 运行时）：
            // 回退到浏览器，至少保证编辑器仍然可用。
            _logger.Error("无法在应用内打开编辑器，将改用浏览器打开。");
            _logger.FormatException(exception);
            this.ShowErrorToast("无法在应用内打开编辑器，已改用浏览器打开。", exception);
            return false;
        }
    }

    /// <summary>
    ///     打开 JavaScript 行动的编辑器点击事件。
    ///     <para>
    ///         启用「应用内 JS 编辑器」（安卓端强制启用）时用应用内代码编辑器打开；
    ///         否则用系统默认程序打开脚本文件。
    ///     </para>
    /// </summary>
    private void OpenJavaScriptEditorButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var project = ViewModel.SelectedProject;
        if (project == null)
        {
            return;
        }

        if (GlobalConstants.Configs.MainConfig!.Data.IsInAppJsEditorEnabled)
        {
            try
            {
                var editorView = IAppHost.GetService<JavaScriptEditorView>();
                editorView.LoadProject(project);
                editorView.Open();
                return;
            }
            catch (Exception exception)
            {
                _logger.Error("无法在应用内打开 JavaScript 编辑器，将改用系统默认程序打开。");
                _logger.FormatException(exception);
                this.ShowErrorToast("无法在应用内打开 JavaScript 编辑器，已改用系统默认程序打开。", exception);
            }
        }

        OpenJavaScriptFileWithDefaultProgram(project);
    }

    /// <summary>
    ///     用系统默认程序打开脚本文件（第三方编辑器兜底）。
    /// </summary>
    private void OpenJavaScriptFileWithDefaultProgram(Project project)
    {
        try
        {
            var path = ProjectsConfigManager.GetJavaScriptJsPath(project);
            Process.Start(new ProcessStartInfo(path)
            {
                UseShellExecute = true
            });
        }
        catch (Exception exception)
        {
            _logger.Error("无法用系统默认程序打开脚本文件。");
            _logger.FormatException(exception);
            this.ShowErrorToast("无法打开脚本文件，请手动打开插件配置目录下的 Scripts 文件夹。", exception);
        }
    }

    /// <summary>
    ///     打开脚本所在文件夹点击事件。
    /// </summary>
    private void OpenJavaScriptFolderButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var project = ViewModel.SelectedProject;
        if (project == null)
        {
            return;
        }

        try
        {
            var path = ProjectsConfigManager.GetJavaScriptJsPath(project);
            var folder = Path.GetDirectoryName(path);
            if (folder == null)
            {
                return;
            }

            Process.Start(new ProcessStartInfo(folder)
            {
                UseShellExecute = true
            });
        }
        catch (Exception exception)
        {
            _logger.Error("无法打开脚本所在文件夹。");
            _logger.FormatException(exception);
            this.ShowErrorToast("无法打开脚本所在文件夹。", exception);
        }
    }

    /// <summary>
    ///     运行项目点击事件
    /// </summary>
    private async void RunProjectButton_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            switch (ViewModel.SelectedProject!.Type)
            {
                case ProjectsType.BlocklyAction:
                    await _blocklyRunner.RunBlocklyProject(ViewModel.SelectedProject!);
                    break;
                case ProjectsType.JavaScriptAction:
                    await _blocklyRunner.RunJavaScriptProject(ViewModel.SelectedProject!);
                    break;
                case ProjectsType.CiRuleset:
                    _ciRunner.RunRulesetProject(ViewModel.SelectedProject);
                    break;
                case ProjectsType.CiActionSet:
                    await _ciRunner.RunActionSetProject(ViewModel.SelectedProject);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        catch (Exception exception)
        {
            _logger.FormatException(exception);
        }
    }

    /// <summary>
    ///     删除项目点击事件
    /// </summary>
    private void DeleteProjectButton_Click(object? sender, RoutedEventArgs e)
    {
        ProjectsConfigManager.DeleteProject(ViewModel.SelectedProject!);
        ViewModel.SelectedProject = null;
        ViewModel.IsPanelOpened = false;
    }
}