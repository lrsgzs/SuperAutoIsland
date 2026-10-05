using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Channels;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using ClassIsland.Core;
using ClassIsland.Platforms.Abstraction;
using SuperAutoIsland.Enums;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Shared;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Services;

/// <summary>
///     一条 WebSocket 连接：收发、命令分发、「后端运行」会话
/// </summary>
/// <remarks>
///     从 <see cref="SaiServer" /> 里抽出来：SaiServer 只管监听和静态文件，
///     这里不依赖 HttpListener，可以直接拿一个假的 <see cref="WebSocket" /> 驱动跑协议测试。
/// </remarks>
/// <param name="websocket">连接</param>
/// <param name="runner">行动/规则/数据运行器</param>
/// <param name="blocklyRunner">Blockly 运行器</param>
/// <param name="logger">日志</param>
public class SaiConnection(WebSocket websocket, SaiBlockRunner runner,
                           BlocklyRunner.BlocklyRunner blocklyRunner, Logger logger)
{
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

    /// <summary>
    ///     处理这条连接，直到对端关闭
    /// </summary>
    public async Task HandleAsync()
    {
        // 所有出站消息（回包 + 运行日志/运行结果推送）都走这一条队列，由唯一的发送任务发出：
        // 这样 socket 上不会有并发 SendAsync，日志和回包的先后顺序也是稳定的。
        var sendQueue = Channel.CreateUnbounded<string>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });
        var sendTask = Task.Run(async () =>
        {
            try
            {
                await foreach (var json in sendQueue.Reader.ReadAllAsync())
                {
                    var bytes = Encoding.UTF8.GetBytes(json);
                    await websocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true,
                                              CancellationToken.None);
                }
            }
            catch (Exception e)
            {
                logger.Debug($"发送链结束：{e.Message}");
            }
        });

        // 连接级取消：连接断了要掐掉正在跑的脚本
        var connectionCancellation = new CancellationTokenSource();

        // 当前正在跑的脚本（编辑器点「后端运行」时用）
        CancellationTokenSource? runCancellation = null;

        void Send(object payload)
        {
            sendQueue.Writer.TryWrite(JsonSerializer.Serialize(payload));
        }

        // 跑一段前端送来的代码。特意不 await：运行期间这条连接还要能继续收「停止」「保存」这些命令
        async Task StartBlocklyRunAsync(string code)
        {
            // 上一次还没跑完就先掐掉（编辑器上连点「运行」只保留最后一次）
            runCancellation?.Cancel();

            var runCts = CancellationTokenSource.CreateLinkedTokenSource(connectionCancellation.Token);
            runCancellation = runCts;
            var stopwatch = Stopwatch.StartNew();

            try
            {
                await blocklyRunner.RunJavaScript(code, runCts.Token,
                                                 (level, logMessage) => Send(new
                                                 {
                                                     type = "log",
                                                     level,
                                                     message = logMessage
                                                 }));
                Send(new
                {
                    type = "runFinished",
                    ok = true,
                    elapsedMs = stopwatch.ElapsedMilliseconds
                });
            }
            catch (Exception e)
            {
                // 取消不一定抛 OperationCanceledException：Jint 中断脚本抛的是 ExecutionCanceledException，
                // 所以这里按「token 是否已取消」判断，别把「停止」报成运行失败。
                if (runCts.IsCancellationRequested)
                {
                    Send(new
                    {
                        type = "runFinished",
                        ok = false,
                        cancelled = true,
                        elapsedMs = stopwatch.ElapsedMilliseconds
                    });
                }
                else
                {
                    logger.FormatException(e);
                    Send(new
                    {
                        type = "runFinished",
                        ok = false,
                        error = e.Message,
                        elapsedMs = stopwatch.ElapsedMilliseconds
                    });
                }
            }
            finally
            {
                if (ReferenceEquals(runCancellation, runCts))
                {
                    runCancellation = null;
                }

                runCts.Dispose();
            }
        }

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
                    logger.Info("WebSocket连接关闭");
                }
                else
                {
                    string message;
                    using (var reader = new StreamReader(ms, Encoding.UTF8))
                    {
                        message = await reader.ReadToEndAsync();
                    }

                    logger.Info($"收到消息: {message}");
                    object jsonReturnData;
                    string? messageId = null;

                    try
                    {
                        var messageJson = JsonDocument.Parse(message);
                        var messageJsonType = messageJson.RootElement.GetProperty("type");
                        var messageType = messageJsonType.GetString()!;

                        if (messageJson.RootElement.TryGetProperty("msgId", out var msgIdElement) &&
                            msgIdElement.ValueKind == JsonValueKind.String)
                        {
                            messageId = msgIdElement.GetString();
                        }

                        logger.Debug($"Type: {messageType}");
                        switch (messageType)
                        {
                            case "getCategories":
                                var categories =
                                    JsonSerializer.Serialize(SaiBlocksRegistry.Categories.Values, ExtraBlocksOptions);
                                jsonReturnData = new
                                {
                                    type = "result",
                                    blocksString = categories // 直接返回 json 避免问题。前端有 JSON.parse
                                };
                                break;
                            // 运行行动
                            case "runAction":
                                var actionId = messageJson.RootElement.GetProperty("id").GetString()!;
                                var actionSettings = messageJson.RootElement.GetProperty("settings");
                                await runner.RunAction(actionId, actionSettings);
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
                                    result = await runner.RunRule(ruleId, ruleSettings)
                                };
                                break;
                            // 运行数据
                            case "runData":
                                var dataId = messageJson.RootElement.GetProperty("id").GetString() ?? "<null>";
                                var dataSettings = messageJson.RootElement.GetProperty("settings");
                                jsonReturnData = new
                                {
                                    type = "result",
                                    data = await runner.RunData(dataId, dataSettings)
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
                                    logger.FormatException(e);
                                    workspace = "{}";
                                }

                                jsonReturnData = new
                                {
                                    type = "result",
                                    workspace,
                                    guid = project2.Id
                                };
                                break;
                            // 用后端 Jint 跑一段前端送来的代码（编辑器「后端运行」，不落盘）
                            case "run":
                                var runCode = messageJson.RootElement.GetProperty("code").GetString() ?? "";
                                _ = StartBlocklyRunAsync(runCode);
                                jsonReturnData = new
                                {
                                    type = "result"
                                };
                                break;
                            // 停止当前正在跑的后端脚本
                            case "stopRun":
                                runCancellation?.Cancel();
                                jsonReturnData = new
                                {
                                    type = "result"
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
                                    logger.Warn($"未找到 DynamicDropdown getter {dynamicDropdownId}");
                                    options = [("???", "???")];
                                }

                                jsonReturnData = new
                                {
                                    type = "result",
                                    options = options.Select(t => (List<string>)[t.Item1, t.Item2]).ToList()
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
                        logger.FormatException(e);
                        jsonReturnData = new
                        {
                            type = "error",
                            message = e.Message
                        };
                    }

                    var returnNode = JsonSerializer.SerializeToNode(jsonReturnData);
                    if (returnNode is JsonObject returnObject && !string.IsNullOrEmpty(messageId))
                    {
                        returnObject["msgId"] = messageId;
                    }

                    var returnJson = returnNode?.ToJsonString() ?? "{}";
                    logger.Log("TRACE", $"服务器回复: {returnJson}");
                    sendQueue.Writer.TryWrite(returnJson);
                }
            }
        }
        catch (Exception e)
        {
            logger.FormatException(e);
        }
        finally
        {
            // 连接结束：掐掉正在跑的脚本，收掉发送链
            connectionCancellation.Cancel();
            runCancellation?.Cancel();
            sendQueue.Writer.TryComplete();
            try
            {
                await sendTask;
            }
            catch (Exception e)
            {
                logger.Debug($"发送链收尾失败：{e.Message}");
            }

            connectionCancellation.Dispose();
        }
    }

    /// <summary>
    ///     打开本地文件选择器并返回可用的本地路径。
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
                                    FileTypeFilter = GetFileTypeFilter(kind)
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

                return new
                {
                    type = "result",
                    paths,
                    message = invalid > 0 ? "无法直接引用所选文件。请先将它保存到本地，再输入文件路径。" : ""
                };
            }
            catch (Exception e)
            {
                logger.FormatException(e);
                return (object)new
                {
                    type = "result",
                    paths = Array.Empty<string>(),
                    message = "打开文件选择器失败，请直接输入文件路径。"
                };
            }
        });
    }

    /// <summary>
    ///     获取文件类型过滤器
    /// </summary>
    private static FilePickerFileType[]? GetFileTypeFilter(string? kind)
    {
        return kind?.ToLowerInvariant() switch
        {
            "image" => [FilePickerFileTypes.ImageAll],
            "json"  => [FilePickerFileTypes.Json],
            "text"  => [FilePickerFileTypes.TextPlain],
            _       => null
        };
    }
}
