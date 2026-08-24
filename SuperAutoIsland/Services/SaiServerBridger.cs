using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Shared;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Services;

/// <summary>
/// 服务器桥接器
/// </summary>
public class SaiServerBridger : ISaiServer
{
    private readonly SaiServer _instance;
    private readonly Logger<SaiServerBridger> _logger = new();
    
    /// <summary>
    /// 构造函数
    /// <see cref="SaiServerBridger"/>
    /// </summary>
    public SaiServerBridger()
    {
        _instance = new SaiServer(GlobalConstants.Configs.MainConfig!.Data.ServerPort);
        _logger.Info($"服务器地址：{_instance.Url}");
        _ = _instance.Serve();
        
        _logger.Info("已初始化 SaiServer！");
    }

    /// <inheritdoc />
    public void RegisterBlocks(string categoryName, RegisterHandler handler)
    {
        var register = new BlocksRegister();
        handler(register);
        SaiBlocksRegistry.Categories[categoryName] = register.Items;
        foreach (var (id, block) in register.Blocks)
        {
            SaiBlocksRegistry.Blocks[id] = block;
        }
        
        _logger.Info($"{categoryName} 已注册 blocks");
    }

    /// <inheritdoc />
    public void RegisterDynamicDropdown(string id, DynamicDropdownHandler handler)
    {
        SaiBlocksRegistry.DynamicDropdowns[id] = handler;
        _logger.Info($"已注册 id 为 {id} 的 DynamicDropdownGetter");
    }

    /// <inheritdoc />
    public void Shutdown()
    {
        _instance.Shutdown();
    }
}