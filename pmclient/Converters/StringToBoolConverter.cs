namespace pmclient.Converters;

using System;
using System.Globalization;
using Avalonia.Data.Converters;

public class StringToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is string str && !string.IsNullOrEmpty(str);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException("ConvertBack not supported");
    }
}