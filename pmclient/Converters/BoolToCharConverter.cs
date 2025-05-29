using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace pmclient.Converters;

public class BoolToCharConverter : IValueConverter
{
    public char TrueChar { get; set; }
    public char FalseChar { get; set; }
    public char NullChar { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            true => TrueChar,
            false => FalseChar,
            _ => NullChar
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is char ch)
        {
            return ch.Equals(TrueChar);
        }

        return false;
    }
}