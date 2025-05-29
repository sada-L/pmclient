using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace pmclient.Converters;

public class AddValueConverter : IValueConverter
{
    public double Offset { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double d)
            return d + Offset;
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double d)
            return d - Offset;
        return value;
    }
}