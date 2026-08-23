using ClassIsland.Core.Models.Ruleset;
using ClassIsland.Shared.Models.Automation;

namespace SuperAutoIsland.Interface.Services;

/// <summary>
/// 积木注册委托
/// </summary>
public delegate void RegisterHandler(BlocksRegister register);

/// <summary>
/// 行动 wrapper
/// </summary>
[Obsolete("已过时。请使用 v2 注册方法。")]
public delegate ActionItem ActionWrapper(ActionItem action);

/// <summary>
/// 规则 wrapper
/// </summary>
[Obsolete("已过时。请使用 v2 注册方法。")]
public delegate Rule RuleWrapper(Rule rule);

/// <summary>
/// 动态下拉框 getter
/// </summary>
public delegate Task<List<(string, string)>> DynamicDropdownHandler();

/// <summary>
/// 专有行动处理器，在 Blockly行动 中会覆盖应用原行动的处理器
/// </summary>
[Obsolete("已过时。请使用 v2 注册方法。")]
public delegate Task ActionHandler(object? parameters);

/// <summary>
/// 专有规则处理器，在 Blockly行动 中会覆盖应用原规则的处理器
/// </summary>
[Obsolete("已过时。请使用 v2 注册方法。")]
public delegate bool RuleHandler(object? parameters);

/// <summary>
/// 数据 getter
/// </summary>
[Obsolete("已过时。请使用 v2 注册方法。")]
public delegate Task<string> DataHandler(object? parameters);

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
    [Obsolete("已过时。请使用 v2 注册方法。")]
    public void RegisterBlocks(string pluginName, RegisterData data);

    /// <summary>
    /// v2 注册积木
    /// </summary>
    /// <param name="categoryName">分类名称</param>
    /// <param name="handler">注册委托 (立即执行)</param>
    public void RegisterBlocks(string categoryName, RegisterHandler handler);
    
    /// <summary>
    /// 注册行动 wrapper
    /// </summary>
    /// <param name="id">行动 id</param>
    /// <param name="wrapper">wrapper 函数</param>
    [Obsolete("已过时。请使用 v2 注册方法。")]
    public void RegisterWrapper(string id, ActionWrapper wrapper);
    
    /// <summary>
    /// 注册规则 wrapper
    /// </summary>
    /// <param name="id">规则 id</param>
    /// <param name="wrapper">wrapper 函数</param>
    [Obsolete("已过时。请使用 v2 注册方法。")]
    public void RegisterWrapper(string id, RuleWrapper wrapper);
    
    /// <summary>
    /// 注册动态下拉框 getter
    /// </summary>
    /// <param name="id">动态下拉框 id</param>
    /// <param name="handler">获取函数</param>
    public void RegisterDynamicDropdown(string id, DynamicDropdownHandler handler);
    
    /// <summary>
    /// 专有行动处理器，在 Blockly行动 中会覆盖应用原行动的处理器。
    /// 无特殊需求（如 blockly 独占/处理逻辑需要特调）无需设置
    /// </summary>
    /// <param name="id">行动 id</param>
    /// <param name="handler">处理器</param>
    [Obsolete("已过时。请使用 v2 注册方法。")]
    public void RegisterActionHandler(string id, ActionHandler handler);
    
    /// <summary>
    /// 专有规则处理器，在 Blockly行动 中会覆盖应用原规则的处理器。
    /// 无特殊需求（如 blockly 独占/处理逻辑需要特调）无需设置
    /// </summary>
    /// <param name="id">规则 id</param>
    /// <param name="handler">处理器</param>
    [Obsolete("已过时。请使用 v2 注册方法。")]
    public void RegisterRuleHandler(string id, RuleHandler handler);
    
    /// <summary>
    /// 注册数据处理器
    /// </summary>
    /// <param name="id">数据积木 id</param>
    /// <param name="handler">获取函数</param>
    [Obsolete("已过时。请使用 v2 注册方法。")]
    public void RegisterDataHandler<T>(string id, DataHandler handler);

    /// <summary>
    /// 结束服务器（好像不能用）
    /// </summary>
    public void Shutdown();
}