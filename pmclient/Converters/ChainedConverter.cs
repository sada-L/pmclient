using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace pmclient.Converters;

public class ChainedConverter : IValueConverter
{
    public IValueConverter? First { get; set; }
    public IValueConverter? Second { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var intermediate = First?.Convert(value, targetType, parameter, culture);
        return Second?.Convert(intermediate, targetType, parameter, culture);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var intermediate = Second?.ConvertBack(value, targetType, parameter, culture);
        return First?.ConvertBack(intermediate, targetType, parameter, culture);
    }
}