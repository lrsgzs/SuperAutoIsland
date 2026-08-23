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
/// 动态下拉框 getter
/// </summary>
public delegate Task<List<(string, string)>> DynamicDropdownGetter();

/// <summary>
/// 数据 getter
/// </summary>
public delegate Task<string> DataHandler(object? parameters);

/// <summary>
/// 专有行动处理器，在 Blockly行动 中会覆盖应用原行动的处理器
/// </summary>
public delegate Task ActionHandler(object? parameters);

/// <summary>
/// 专有规则处理器，在 Blockly行动 中会覆盖应用原规则的处理器
/// </summary>
public delegate bool RuleHandler(object? parameters);

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
    /// 注册动态下拉框 getter
    /// </summary>
    /// <param name="id">动态下拉框 id</param>
    /// <param name="getter">获取函数</param>
    public void RegisterDynamicDropdown(string id, DynamicDropdownGetter getter);
    
    /// <summary>
    /// 专有行动处理器，在 Blockly行动 中会覆盖应用原行动的处理器。
    /// 无特殊需求（如 blockly 独占/处理逻辑需要特调）无需设置
    /// </summary>
    /// <param name="id">行动 id</param>
    /// <param name="handler">处理器</param>
    public void RegisterActionHandler(string id, ActionHandler handler);
    
    /// <summary>
    /// 专有规则处理器，在 Blockly行动 中会覆盖应用原规则的处理器。
    /// 无特殊需求（如 blockly 独占/处理逻辑需要特调）无需设置
    /// </summary>
    /// <param name="id">规则 id</param>
    /// <param name="handler">处理器</param>
    public void RegisterRuleHandler(string id, RuleHandler handler);
    
    /// <summary>
    /// 注册数据处理器
    /// </summary>
    /// <param name="id">数据积木 id</param>
    /// <param name="handler">获取函数</param>
    public void RegisterDataHandler<T>(string id, DataHandler handler);

    /// <summary>
    /// 结束服务器（好像不能用）
    /// </summary>
    public void Shutdown();
}