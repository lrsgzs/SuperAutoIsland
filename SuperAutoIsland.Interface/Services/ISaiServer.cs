using ClassIsland.Core.Models.Ruleset;
using ClassIsland.Shared.Models.Automation;

namespace SuperAutoIsland.Interface.Services;

/// <summary>
/// 行动 wrapper
/// </summary>
public delegate ActionItem ActionWrapper(ActionItem action);

/// <summary>
/// 规则 wrapper
/// </summary>
public delegate Rule RuleWrapper(Rule rule);

/// <summary>
/// 服务器接口
/// </summary>
public interface ISaiServer
{
    /// <summary>
    /// 注册积木
    /// </summary>
    /// <param name="pluginName">插件名称</param>
    /// <param name="data">积木数据</param>
    public void RegisterBlocks(string pluginName, RegisterData data);
    
    /// <summary>
    /// 注册行动 wrapper
    /// </summary>
    /// <param name="id">行动 id</param>
    /// <param name="wrapper">wrapper 函数</param>
    public void RegisterWrapper(string id, ActionWrapper wrapper);
    
    /// <summary>
    /// 注册规则 wrapper
    /// </summary>
    /// <param name="id">规则 id</param>
    /// <param name="wrapper">wrapper 函数</param>
    public void RegisterWrapper(string id, RuleWrapper wrapper);

    /// <summary>
    /// 结束服务器（好像不能用）
    /// </summary>
    public void Shutdown();
}