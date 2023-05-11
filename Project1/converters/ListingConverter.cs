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
                return "BlackList";
            case true:
                return "WhiteList";
            case null:
                return "Copy all Content";
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value;
    }
}