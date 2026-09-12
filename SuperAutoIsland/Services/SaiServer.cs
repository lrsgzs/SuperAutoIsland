using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using ClassIsland.Core;
using ClassIsland.Platforms.Abstraction;
using ClassIsland.Shared;
using SuperAutoIsland.Enums;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Shared;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Services;

/// <summary>
/// SuperAutoIsland 服务器
/// </summary>
public class SaiServer
{
    public readonly string Url;
    private readonly SaiBlockRunner _runner = IAppHost.GetService<SaiBlockRunner>();
    private bool _isRunning;
    private readonly string _wwwRoot;
    private readonly HttpListener _listener;
    private readonly Logger<SaiServer> _logger = new();

    private static readonly JsonSerializerOptions ExtraBlocksOptions = new()
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new StringTupleConverter(),
            new JsonStringEnumConverter<BlockKind>(JsonNamingPolicy.CamelCase)
        }
    };
    
    public SaiServer(string port)
    {
        Url = $"http://localhost:{port}/";
        _wwwRoot = Path.Combine(GlobalConstants.PluginFolder!, "Assets", "wwwroot");
        _isRunning = true;
        
        _listener = new HttpListener();
        _listener.Prefixes.Add(Url);
        _listener.Start();
        
        _logger.Info("已启动 SaiServer");
    }

    /// <summary>
    /// 服务器 启动启动启动
    /// </summary>
    public async Task Serve()
    {
        while (_isRunning)
        {
            try
            {
                var context = await _listener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    // WebSocket请求
                    _ = HandleWebSocketAsync(context);
                }
                else
                {
                    // 静态文件请求
                    _ = ServeStaticFileAsync(context);
                }
            }
            catch (HttpListenerException ex) when (ex.ErrorCode == 995)
            {
                _logger.Info("哦齁齁齁齁，服务器已经关了呢喵...");
            }
            catch (Exception e)
            {
                _logger.FormatException(e);
            }
        }
    }

    /// <summary>
    /// 停止服务器，但是不工作
    /// </summary>
    public void Shutdown()
    {
        _logger.Debug("开始关闭服务器...");
        _isRunning = false;
        if (_listener.IsListening) {
            _listener.Stop();
            _listener.Close();
        }
        GC.SuppressFinalize(this);
    }
    
    // Generated base server by DeepSeek（
    // features written by lrs2187（
    
    /// <summary>
    /// 处理WebSocket连接
    /// </summary>
    /// <param name="context">listener 上下文</param>
    private async Task HandleWebSocketAsync(HttpListenerContext context)
    {
        WebSocketContext wsContext = await context.AcceptWebSocketAsync(null);
        var websocket = wsContext.WebSocket;
        _logger.Info($"WebSocket连接已建立: {context.Request.RemoteEndPoint}");

        try
        {
            var chunk = new byte[4096];
            var receiveBuffer = new ArraySegment<byte>(chunk);
            
            while (websocket.State == WebSocketState.Open)
            {
                using var ms = new MemoryStream();
                WebSocketReceiveResult result;
                
                do
                {
                    result = await websocket.ReceiveAsync(receiveBuffer, CancellationToken.None);
                    ms.Write(chunk, 0, result.Count);
                } while (!result.EndOfMessage);

                ms.Position = 0;

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await websocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                    _logger.Info("WebSocket连接关闭");
                }
                else
                {
                    string message;
                    using (var reader = new StreamReader(ms, Encoding.UTF8))
                    {
                        message = await reader.ReadToEndAsync();
                    }
                        
                    _logger.Info($"收到消息: {message}");
                    object jsonReturnData;

                    try
                    {
                        var messageJson = JsonDocument.Parse(message);
                        var messageJsonType = messageJson.RootElement.GetProperty("type");
                        var messageType = messageJsonType.GetString()!;

                        // TODO: 以后有时间了抽离此处逻辑
                        _logger.Debug($"Type: {messageType}");
                        switch (messageType)
                        {
                            // 获取额外积木
                            case "getExtraBlocks":
                                var extraBlocks =
                                    JsonSerializer.Serialize(SaiBlocksRegistry.Categories, ExtraBlocksOptions);
                                jsonReturnData = new
                                {
                                    type = "result",
                                    blocksString = extraBlocks // 直接返回 json 避免问题。前端有 JSON.parse
                                };
                                break;
                            // 运行行动
                            case "runAction":
                                var actionId = messageJson.RootElement.GetProperty("id").GetString()!;
                                var actionSettings = messageJson.RootElement.GetProperty("settings");
                                await _runner.RunAction(actionId, actionSettings);
                                jsonReturnData = new
                                {
                                    type = "result"
                                };
                                break;
                            // 运行规则
                            case "runRule":
                                var ruleId = messageJson.RootElement.GetProperty("id").GetString()!;
                                var ruleSettings = messageJson.RootElement.GetProperty("settings");
                                jsonReturnData = new
                                {
                                    type = "result",
                                    result = await _runner.RunRule(ruleId, ruleSettings)
                                };
                                break;
                            // 运行数据
                            case "runData":
                                var dataId = messageJson.RootElement.GetProperty("id").GetString() ?? "<null>";
                                var dataSettings = messageJson.RootElement.GetProperty("settings");
                                jsonReturnData = new
                                {
                                    type = "result",
                                    data = await _runner.RunData(dataId, dataSettings)
                                };
                                break;
                            // 保存项目
                            case "save":
                                var projectData = messageJson.RootElement.GetProperty("data");
                                switch (projectData.GetProperty("type").GetString()!)
                                {
                                    case "blocklyAction":
                                        var guid1 = projectData.GetProperty("guid").GetGuid();
                                        if (guid1 == Guid.Empty)
                                        {
                                            guid1 = Guid.NewGuid();
                                        }
                                        
                                        var project1 = ProjectsConfigManager.GetOrCreateProject(
                                            ProjectsType.BlocklyAction,
                                            guid1, null);
                                        ProjectsConfigManager.SaveBlocklyProject(
                                            project1,
                                            projectData.GetProperty("workspace").GetString()!,
                                            projectData.GetProperty("code").GetString()!);

                                        jsonReturnData = new
                                        {
                                            type = "result"
                                        };
                                        break;
                                    default:
                                        jsonReturnData = new
                                        {
                                            type = "bad-project-type"
                                        };
                                        break;
                                }

                                break;
                            // 加载项目
                            case "load":
                                var guid2 = messageJson.RootElement.GetProperty("guid").GetGuid();
                                if (guid2 == Guid.Empty)
                                {
                                    guid2 = Guid.NewGuid();
                                }

                                var project2 = ProjectsConfigManager.GetOrCreateProject(
                                    ProjectsType.BlocklyAction,
                                    guid2, null);
                                string workspace;
                                try
                                {
                                    workspace = ProjectsConfigManager.LoadBlocklyProjectWorkspace(project2);
                                }
                                catch (Exception e)
                                {
                                    _logger.FormatException(e);
                                    workspace = "{}";
                                }

                                jsonReturnData = new
                                {
                                    type = "result",
                                    workspace,
                                    guid = project2.Id,
                                };
                                break;
                            // 动态下拉框
                            case "getDynamicDropdownContent":
                                var dynamicDropdownId =
                                    messageJson.RootElement.GetProperty("id").GetString() ?? "<null>";
                                var getter =
                                    SaiBlocksRegistry.DynamicDropdowns.GetValueOrDefault(dynamicDropdownId);
                                List<(string, string)> options;

                                if (getter != null)
                                {
                                    options = await getter();
                                }
                                else
                                {
                                    _logger.Warn($"未找到 DynamicDropdown getter {dynamicDropdownId}");
                                    options = [("???", "???")];
                                }

                                jsonReturnData = new
                                {
                                    type = "result",
                                    options = options.Select(t => (List<string>)[t.Item1, t.Item2]).ToList(),
                                };
                                break;
                            // 选取本地文件
                            case "pickFile":
                                var pickKind = messageJson.RootElement.TryGetProperty("kind", out var kindElement)
                                    ? kindElement.GetString()
                                    : null;
                                var allowMultiple = messageJson.RootElement.TryGetProperty(
                                    "allowMultiple", out var multipleElement) &&
                                    multipleElement.ValueKind == JsonValueKind.True;
                                var pickTitle = messageJson.RootElement.TryGetProperty("title", out var titleElement)
                                    ? titleElement.GetString()
                                    : null;
                                jsonReturnData = await PickFilesAsync(pickKind, allowMultiple, pickTitle);
                                break;
                            // 默认行为
                            default:
                                jsonReturnData = new
                                {
                                    type = "bad-command-type"
                                };
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.FormatException(e);
                        jsonReturnData = new
                        {
                            type = "error",
                        };
                    }

                    var returnJson = JsonSerializer.Serialize(jsonReturnData);
                    _logger.Info($"服务器回复: {returnJson}");
                    var responseBytes = Encoding.UTF8.GetBytes(returnJson);
                    await websocket.SendAsync(
                        new ArraySegment<byte>(responseBytes),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None);
                }
            }
        }
        catch (Exception e)
        {
            _logger.FormatException(e);
        }
    }

    /// <summary>
    /// 打开本地文件选择器并返回可用的本地路径。
    /// </summary>
    /// <param name="kind">文件类型，如 image、json、text</param>
    /// <param name="allowMultiple">是否允许选择多个文件</param>
    /// <param name="title">选择器标题</param>
    private async Task<object> PickFilesAsync(string? kind, bool allowMultiple, string? title)
    {
        return await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            try
            {
                var root = AppBase.Current.GetRootWindow();
                var files = await PlatformServices.FilePickerService.OpenFilesPickerAsync(
                    new FilePickerOpenOptions
                    {
                        Title = title ?? "选择文件",
                        AllowMultiple = allowMultiple,
                        FileTypeFilter = GetFileTypeFilter(kind),
                    }, root);

                var paths = new List<string>();
                var invalid = 0;
                foreach (var path in files)
                {
                    if (PlatformServices.FilePickerService.IsBookmark(path) ||
                        !Path.IsPathFullyQualified(path))
                    {
                        invalid++;
                        continue;
                    }

                    paths.Add(path);
                }

                return (object)new
                {
                    type = "result",
                    paths,
                    message = invalid > 0 ? "无法直接引用所选文件。请先将它保存到本地，再输入文件路径。" : "",
                };
            }
            catch (Exception e)
            {
                _logger.FormatException(e);
                return (object)new
                {
                    type = "result",
                    paths = Array.Empty<string>(),
                    message = "打开文件选择器失败，请直接输入文件路径。",
                };
            }
        });
    }

    /// <summary>
    /// 获取文件类型过滤器
    /// </summary>
    private static FilePickerFileType[]? GetFileTypeFilter(string? kind) => kind?.ToLowerInvariant() switch
    {
        "image" => [FilePickerFileTypes.ImageAll],
        "json" => [FilePickerFileTypes.Json],
        "text" => [FilePickerFileTypes.TextPlain],
        _ => null,
    };

    /// <summary>
    /// 处理静态文件请求
    /// </summary>
    /// <param name="context">listener 上下文</param>
    private async Task ServeStaticFileAsync(HttpListenerContext context)
    {
        try
        {
            var path = context.Request.Url!.LocalPath.TrimStart('/');
            path = string.IsNullOrEmpty(path) ? "index.html" : path;
            var fullPath = Path.Combine(_wwwRoot, path);

            if (File.Exists(fullPath))
            {
                var content = await File.ReadAllBytesAsync(fullPath);
                context.Response.ContentType = GetMimeType(Path.GetExtension(fullPath));
                context.Response.ContentLength64 = content.Length;
                await context.Response.OutputStream.WriteAsync(content, 0, content.Length);
                _logger.Info($"已发送文件: {path}");
            }
            else
            {
                context.Response.StatusCode = 404;
                var notFound = "File Not Found"u8.ToArray();
                await context.Response.OutputStream.WriteAsync(notFound);
                _logger.Warn($"文件未找到: {path}");
            }
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            var error = Encoding.UTF8.GetBytes($"Server Error: {ex.Message}");
            await context.Response.OutputStream.WriteAsync(error, 0, error.Length);
            _logger.Warn($"文件处理错误: {ex.Message}");
        }
        finally
        {
            context.Response.Close();
        }
    }

    /// <summary>
    /// 获取 MIME类型
    /// </summary>
    /// <param name="extension">类型扩展名字符串</param>
    /// <returns>MIME 类型字符串</returns>
    private static string GetMimeType(string extension) => extension.ToLower() switch
    {
        ".html" => "text/html",
        ".js" => "application/javascript",
        ".css" => "text/css",
        ".png" => "image/png",
        ".jpg" or ".jpeg" => "image/jpeg",
        ".gif" => "image/gif",
        ".json" => "application/json",
        ".svg" => "image/svg+xml",
        _ => "application/octet-stream"
    };
}