using System.Net;
using ClassIsland.Shared;
using SuperAutoIsland.Shared;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Services;

/// <summary>
///     SuperAutoIsland 服务器
/// </summary>
public class SaiServer
{
    public readonly string Url;
    private readonly HttpListener _listener;
    private readonly Logger<SaiServer> _logger = new();
    private readonly SaiBlockRunner _runner;
    private readonly BlocklyRunner.BlocklyRunner _blocklyRunner;
    private readonly string _wwwRoot;
    private bool _isRunning;

    /// <summary>
    ///     构造函数
    /// </summary>
    /// <param name="port">监听端口</param>
    /// <param name="runner">行动/规则/数据运行器，为 null 时从宿主取</param>
    /// <param name="blocklyRunner">Blockly 运行器，为 null 时从宿主取</param>
    public SaiServer(string port, SaiBlockRunner? runner = null,
                     BlocklyRunner.BlocklyRunner? blocklyRunner = null)
    {
        _runner = runner ?? IAppHost.GetService<SaiBlockRunner>();
        _blocklyRunner = blocklyRunner ?? IAppHost.GetService<BlocklyRunner.BlocklyRunner>();
        Url = $"http://localhost:{port}/";
        _wwwRoot = Path.Combine(GlobalConstants.PluginFolder!, "Assets", "wwwroot");
        _isRunning = true;

        _listener = new HttpListener();
        _listener.Prefixes.Add(Url);
        _listener.Start();

        _logger.Info("已启动 SaiServer");
    }

    /// <summary>
    ///     服务器 启动启动启动
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
    ///     停止服务器，但是不工作
    /// </summary>
    public void Shutdown()
    {
        _logger.Debug("开始关闭服务器...");
        _isRunning = false;
        if (_listener.IsListening)
        {
            _listener.Stop();
            _listener.Close();
        }

        GC.SuppressFinalize(this);
    }

    // Generated base server by DeepSeek（
    // features written by lrs2187（

    /// <summary>
    ///     处理WebSocket连接
    /// </summary>
    /// <param name="context">listener 上下文</param>
    private async Task HandleWebSocketAsync(HttpListenerContext context)
    {
        var wsContext = await context.AcceptWebSocketAsync(null);
        _logger.Info($"WebSocket连接已建立: {context.Request.RemoteEndPoint}");

        // 收发和命令分发都在 SaiConnection 里（那边不依赖 HttpListener，可以单独测）
        var connection = new SaiConnection(wsContext.WebSocket, _runner, _blocklyRunner, _logger);
        await connection.HandleAsync();
    }

    /// <summary>
    ///     处理静态文件请求
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
                _logger.Log("TRACE", $"已发送文件: {path}");
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
            var error = System.Text.Encoding.UTF8.GetBytes($"Server Error: {ex.Message}");
            await context.Response.OutputStream.WriteAsync(error, 0, error.Length);
            _logger.Warn($"文件处理错误: {ex.Message}");
        }
        finally
        {
            context.Response.Close();
        }
    }

    /// <summary>
    ///     获取 MIME类型
    /// </summary>
    /// <param name="extension">类型扩展名字符串</param>
    /// <returns>MIME 类型字符串</returns>
    private static string GetMimeType(string extension)
    {
        return extension.ToLower() switch
        {
            ".html"           => "text/html",
            ".js"             => "application/javascript",
            ".css"            => "text/css",
            ".png"            => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif"            => "image/gif",
            ".json"           => "application/json",
            ".svg"            => "image/svg+xml",
            _                 => "application/octet-stream"
        };
    }
}
