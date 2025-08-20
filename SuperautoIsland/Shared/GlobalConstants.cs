using SuperAutoIsland.ConfigHandlers;

namespace SuperAutoIsland.Shared;

public static class GlobalConstants
{
    public static string? PluginFolder { get; set; }
    public static string? PluginConfigFolder { get; set; }

    public static class Configs
    {
        public static MainConfigHandler? MainConfig { get; set; }
    }
    
    public static class Assets
    {
        public static readonly string AsciiLogo = "稍后再说";
    }
}