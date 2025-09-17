using System.Globalization;
using Avalonia.Data.Converters;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Models;

namespace SuperAutoIsland.Shared.Converters;

public class ActionsToActionSetConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return new ActionSet
        {
            Name = $"SAI 行动组 - {((Project?)value)?.Name ?? "？？？"}",
            ActionItems = ((Project?)value)?.Actions ?? []
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return ((ActionSet?)value)?.ActionItems ?? [];
    }
}