using SuperAutoIsland.Services.Config;

namespace SuperAutoIsland.Shared;

/// <summary>
/// 公开常量
/// </summary>
public static class GlobalConstants
{
    /// <summary>
    /// 插件路径
    /// </summary>
    public static string? PluginFolder { get; set; }
    
    /// <summary>
    /// 插件配置路径
    /// </summary>
    public static string? PluginConfigFolder { get; set; }

    /// <summary>
    /// 配置集
    /// </summary>
    public static class Configs
    {
        public static MainConfigHandler? MainConfig { get; set; }
        public static ProjectConfigHandler? ProjectConfig { get; set; }
    }
    
    /// <summary>
    /// 资源
    /// </summary>
    public static class Assets
    {
        public static readonly string AsciiLogo = "稍后再说";
        public static readonly Guid ProjectNullGuid = Guid.Parse("8ca34af7-03cb-47ab-a630-0083dc942135");
    }
}