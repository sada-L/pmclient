using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace pmclient.Converters;

public class DotMaskConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var str = value as string;
        return string.IsNullOrEmpty(str) ? "\uf10c" : "\uf111";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}