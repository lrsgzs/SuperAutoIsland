using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using SuperAutoIsland.Interface;
using SuperAutoIsland.Interface.Enums;
using SuperAutoIsland.Services;
using SuperAutoIsland.Shared;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Server;

/// <summary>
/// SuperAutoIsland 服务器
/// </summary>
public class SaiServer
{
    /// <summary>
    /// 额外积木批发
    /// </summary>
    public readonly Dictionary<string, RegisterData> ExtraBlocks = new();
    private readonly ActionAndRuleRunner _runner = new();
    private bool _isRunning;
    
    public readonly string Url;
    private readonly string _wwwRoot;
    private readonly HttpListener _listener;
    private readonly Logger<SaiServer> _logger = new();
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="port"></param>
    public SaiServer(string port)
    {
        Url = $"http://localhost:{port}/";
        _wwwRoot = Path.Combine(GlobalConstants.PluginFolder!, "assets", "wwwroot");
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
                _logger.Info("呃呃呃啊哦呃，服务器已经关了呢喵...");
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
            var buffer = new byte[81920];
            while (websocket.State == WebSocketState.Open)
            {
                var result = await websocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await websocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                    _logger.Info("WebSocket连接关闭");
                }
                else
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    _logger.Info($"收到消息: {message}");
                    object jsonReturnData;
                    
                    try
                    {
                        var messageJson = JsonDocument.Parse(message);
                        var messageJsonType = messageJson.RootElement.GetProperty("type");
                        var messageType = messageJsonType.GetString()!;
                        
                        // 以后有时间了抽离此处逻辑
                        _logger.Debug($"Type: {messageType}");
                        switch (messageType)
                        {
                            // 获取额外积木
                            case "getExtraBlocks":
                                var extraBlocks = GenerateExtraBlocksJson();
                                jsonReturnData = new
                                {
                                    type = "result",
                                    blocksString = extraBlocks  // 直接返回 json 避免问题。前端有 JSON.parse
                                };
                                break;
                            // 运行行动
                            case "runAction":
                                var actionId = messageJson.RootElement.GetProperty("id").GetString()!;
                                await _runner.RunAction(actionId, messageJson.RootElement.GetProperty("settings"));
                                jsonReturnData = new
                                {
                                    type = "result"
                                };
                                break;
                            // 运行规则
                            case "runRule":
                                var ruleId = messageJson.RootElement.GetProperty("id").GetString()!;
                                var resultBoolean = _runner.RunRule(ruleId, messageJson.RootElement.GetProperty("settings"));
                                jsonReturnData = new
                                {
                                    type = "result",
                                    result = resultBoolean
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
                                            type = "not-recognized-project-type"
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
                            // 数据 - 获取学科
                            case "getSubjects":
                                var profileService = IAppHost.GetService<IProfileService>();
                                var subjects = profileService.Profile.Subjects
                                    .Select(kvp => new { Id = kvp.Key, kvp.Value.Name })
                                    .ToList();
                                
                                jsonReturnData = new 
                                {
                                    type = "result",
                                    subjects,
                                };
                                break;
                            // 数据 - 获取组件配置
                            case "getComponentConfigs":
                                var componentService = IAppHost.GetService<IComponentsService>();
                                
                                jsonReturnData = new 
                                {
                                    type = "result",
                                    configs = componentService.ComponentConfigs,
                                };
                                break;
                            // 默认行为
                            default:
                                jsonReturnData = new 
                                {
                                    type = "not-recognized-command-type"
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
                await context.Response.OutputStream.WriteAsync(notFound, 0, notFound.Length);
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
        _ => "application/octet-stream"
    };
    
    /// <summary>
    /// 生成额外积木 json
    /// </summary>
    /// <returns>额外积木 json</returns>
    private string GenerateExtraBlocksJson()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = 
            {
                new MetaArgsConverter(),  // 处理多态参数
                new TupleConverter(),     // 处理元组
                new JsonStringEnumConverter() // 处理枚举
            }
        };
        
        return JsonSerializer.Serialize(ExtraBlocks, options);
    }
}