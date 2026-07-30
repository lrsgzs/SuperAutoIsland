using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using ClassIsland.Core.Abstractions.Automation;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace SuperAutoIsland.Shared;

public static class ActionSerializer
{
    private static JsonSerializerOptions DefaultJsonOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
    
    public static List<string> GetActionsId() => IActionService.ActionInfos.Keys.ToList();
    
    public static string GetActionInfo(string actionId, JsonSerializerOptions? jsonSerializerOptions = null)
    {
        Dictionary<string, object> GetEnumValues(Type target)
        {
            var dict = new Dictionary<string, object>();

            foreach (var enumValue in target.GetEnumValues())
            {
                var name = target.GetEnumName(enumValue) ?? "???";
                dict[name] = enumValue;
            }
            
            return dict;
        }
        
        Dictionary<string, object> GetTypeInfo(Type target)
        {
            var dict = new Dictionary<string, object>();
            var defaultInstance = Activator.CreateInstance(target);
            
            foreach (var property in target.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var type = property.PropertyType;
                if (type.FullName?.StartsWith("System.") ?? true)
                {
                    dict[property.Name] = new
                    {
                        Type = type.ToString(),
                        Default = property.GetValue(defaultInstance)
                    };
                }
                else if (type.IsEnum)
                {
                    dict[property.Name] = new
                    {
                        Type = type.ToString(),
                        Default = property.GetValue(defaultInstance),
                        IsEnum = true,
                        Values = GetEnumValues(type)
                    };
                }
                else
                {
                    dict[property.Name] = new
                    {
                        Type = type.ToString(),
                        Properties = GetTypeInfo(type)
                    };
                }
            }

            return dict;
        }

        var action = IAppHost.Host?.Services.GetKeyedService<ActionBase>(actionId);
        
        var settingsInfo = new object();
        var settingsType = action?.GetType().BaseType?.GetGenericArguments().FirstOrDefault();
        if (settingsType != null)
        {
            settingsInfo = GetTypeInfo(settingsType);
        }
        
        var metadata = new
        {
            ActionInfo = new
            {
                IActionService.ActionInfos[actionId].Id,
                IActionService.ActionInfos[actionId].Name,
                IActionService.ActionInfos[actionId].IsRevertable,
            },
            Settings = settingsInfo
        };
        return JsonSerializer.Serialize(metadata, jsonSerializerOptions ?? DefaultJsonOptions);
    }
}