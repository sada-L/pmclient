using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;

namespace pmclient.Converters;

public class DoubleToGridLengthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double d)
        {
            return new GridLength(d, GridUnitType.Pixel);
        }

        return GridLength.Auto;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is GridLength gridLength && gridLength.IsAbsolute)
        {
            return gridLength.Value;
        }

        return 0.0;
    }
}