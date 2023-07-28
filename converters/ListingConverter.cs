using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Project1.converters;

public class ListingConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        switch (value as bool?)
        {
            case false:
                return "Copy all content";
            case true:
                return "Whitelist";
            case null:
                return "Blacklist";
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value;
    }
}