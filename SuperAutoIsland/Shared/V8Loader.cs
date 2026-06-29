using System.Reactive.Concurrency;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using ClassIsland.Core;
using ClassIsland.Core.Controls;
using FluentAvalonia.UI.Controls;

namespace SuperAutoIsland.Shared;

public static class V8Loader
{
    public static bool IsV8Ready { get; private set; } = false;
    public static bool IsV8Denied { get; private set; } = false;
    
    private static readonly AsyncLock Lock = new();
    private static Task<bool>? _initializationTask;

    /// <summary>
    /// 获取一个 Task，表示 V8 是否已成功加载。
    /// 所有依赖 V8 的代码都应 await 此 Task。
    /// </summary>
    public static Task<bool> InitializationTask => 
        _initializationTask ??= EnsureV8Loaded();
    
    /// <summary>
    /// 确保 V8 原生运行时已下载并配置好搜索路径。
    /// </summary>
    public static async Task<bool> EnsureV8Loaded()
    {
        if (IsV8Ready) return true;
        if (IsV8Denied) return false;
        
        var logger = new Logger.Logger("EnsureV8Loaded");
        
        string rid;
        var arch = RuntimeInformation.ProcessArchitecture;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            rid = arch switch
            {
                Architecture.X64 => "win-x64",
                Architecture.X86 => "win-x86",
                Architecture.Arm64 => "win-arm64",
                _ => throw new PlatformNotSupportedException($"Unsupported Windows architecture: {arch}")
            };
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            rid = arch switch
            {
                Architecture.X64 => "linux-x64",
                Architecture.Arm64 => "linux-arm64",
                _ => throw new PlatformNotSupportedException($"Unsupported Linux architecture: {arch}")
            };
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            rid = arch switch
            {
                Architecture.X64 => "osx-x64",
                Architecture.Arm64 => "osx-arm64",
                _ => throw new PlatformNotSupportedException($"Unsupported macOS architecture: {arch}")
            };
        }
        else
        {
            throw new PlatformNotSupportedException($"Unsupported OS platform.");
        }

        if (!ClearScriptV8Libs.Libs.TryGetValue(rid, out var items))
        {
            throw new PlatformNotSupportedException($"RID '{rid}' is not supported by SuperAutoIsland.");
        }

        var cacheRoot = Path.Combine(CommonDirectories.AppCacheFolderPath, "lrs2187.sai", "V8Runtimes");
        var cacheDir = Path.Combine(cacheRoot, rid);
        Directory.CreateDirectory(cacheDir);

        var missingItems = items.Where(item => !File.Exists(Path.Combine(cacheDir, item.FileName))).ToList();
        if (missingItems.Count > 0)
        {
            logger.Warn("检测到 V8 资源缺失");
            
            var dialog = new TaskDialog
            {
                Title = "SuperAutoIsland | 资源缺失",
                Content = new TextBlock { Text = "检测到 V8 资源缺失，是否下载？" },
                Buttons =
                [
                    new TaskDialogButton("是", true) { IsDefault = true },
                    new TaskDialogButton("否", false)
                ],
                XamlRoot = AppBase.Current.GetRootWindow()
            };
            var result = await dialog.ShowAsync();
            if (Equals(result, false))
            {
                IsV8Denied = true;
                return false;
            }
            
            using var httpClient = new HttpClient();
            foreach (var item in missingItems)
            {
                var destPath = Path.Combine(cacheDir, item.FileName);

                try
                {
                    logger.Log($"Downloading {item.FileName}");
                    await DownloadFileAsync(httpClient, item.Url, destPath);
                    
                    if (OperatingSystem.IsLinux())
                    {
                        File.SetUnixFileMode(destPath, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to download {item.FileName} from {item.Url}", ex);
                }
            }
            
            logger.Log("V8 资源补齐完毕");
            await CommonTaskDialogs.ShowDialog("SuperAutoIsland | 资源缺失", "V8 资源补齐完毕");
        }

        var pluginBase = Plugin.Current?.Info.PluginFolderPath ?? AppContext.BaseDirectory;
        var targetDir = Path.Combine(pluginBase, "runtimes", rid, "native");
        Directory.CreateDirectory(targetDir);

        foreach (var item in items)
        {
            var srcPath = Path.Combine(cacheDir, item.FileName);
            var destPath = Path.Combine(targetDir, item.FileName);
            
            if (!File.Exists(destPath))
            {
                File.Copy(srcPath, destPath, overwrite: true);
            }
        }

        IsV8Ready = true;
        return true;
    }

    private static async Task DownloadFileAsync(HttpClient httpClient, string url, string destPath)
    {
        using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        await using var fileStream = new FileStream(destPath, FileMode.Create, FileAccess.Write, FileShare.None);
        await response.Content.CopyToAsync(fileStream);
    }
}