using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using SuperAutoIsland.Interface.Shared;
using SuperAutoIsland.Servers;

namespace SuperAutoIsland.Shared;

public class SaiServerInstance
{
    private readonly Dictionary<string, RegisterData> _extraBlocks = new();
    private SaiServer? _server;
    
    public void RegisterBlocks(string pluginName, RegisterData data)
    {
        _extraBlocks[pluginName] = data;
    }

    public void LoadServer(string port)
    {
        Console.WriteLine("假装服务器已经启动");
        _server = new SaiServer(port);
    }
    
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
        
        return JsonSerializer.Serialize(_extraBlocks, options);
    }
}