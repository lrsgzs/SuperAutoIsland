using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using SuperAutoIsland.Interface.Shared;
using SuperAutoIsland.Shared;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Server;

public class SaiServer
{
    public readonly Dictionary<string, RegisterData> ExtraBlocks = new();
    private bool _isRunning;
    
    public readonly string Url;
    private readonly string _wwwRoot;
    private readonly HttpListener _listener;
    private readonly Logger _logger = new("lrs2187.sai -> SaiServer");
    
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
            catch
            {
                _logger.FormatException();
            }
        }
    }

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
    
    // 处理WebSocket连接
    private async Task HandleWebSocketAsync(HttpListenerContext context)
    {
        WebSocketContext wsContext = await context.AcceptWebSocketAsync(null);
        WebSocket websocket = wsContext.WebSocket;
        _logger.Info($"WebSocket连接已建立: {context.Request.RemoteEndPoint}");

        try
        {
            var buffer = new byte[1024];
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

                    try
                    {
                        var messageJson = JsonDocument.Parse(message);
                        messageJson.RootElement.TryGetProperty("type", out var messageJsonType);
                        var messageType = messageJsonType.GetString()!;
                        
                        _logger.Debug($"Type: {messageType}");
                    }
                    catch
                    {
                        _logger.FormatException();
                        // 回显。
                    }
                    
                    
                    // 回显消息
                    var response = $"服务器回复: {DateTime.Now:HH:mm:ss} - {message}";
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    await websocket.SendAsync(
                        new ArraySegment<byte>(responseBytes),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None);
                }
            }
        }
        catch
        {
            _logger.FormatException();
        }
    }

    // 处理静态文件请求
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

    // 获取MIME类型
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