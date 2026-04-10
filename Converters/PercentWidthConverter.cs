using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace RaceSimulator.Converters;

public class PercentWidthConverter : IValueConverter
{
    public static readonly PercentWidthConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double d)
            return Math.Clamp(d, 0, 100);
        return 0.0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
