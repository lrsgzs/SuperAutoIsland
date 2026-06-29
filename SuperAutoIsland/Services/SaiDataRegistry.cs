using SuperAutoIsland.Models;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Services;

public static class SaiDataRegistry
{
    public static readonly Dictionary<string, DataGetterItem> DataGetters = new();
    
    private static readonly Logger Logger = new("SaiDataRegistry");

    public static Type GetGetterType(string dataId)
    {
        var dataGetter = DataGetters.GetValueOrDefault(dataId);
        
        return dataGetter != null ? dataGetter.Type : typeof(object);
    }
    
    public static async Task<string> RunData(string dataId, object? parameters)
    {
        var dataGetter = DataGetters.GetValueOrDefault(dataId);
        string dataString;
 
        if (dataGetter != null)
        {
            dataString = await dataGetter.Getter(parameters);
        }
        else
        {
            Logger.Warn($"未找到 DataGetter getter {dataId}");
            dataString = "???";
        }
        
        return dataString;
    }
}