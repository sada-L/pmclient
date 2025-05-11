using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace pmclient.Converters;

public class BoolToStringConverter : IValueConverter
{
    public string TrueString { get; set; }

    public string FalseString { get; set; }

    public string NullString { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            true => TrueString,
            false => FalseString,
            _ => NullString
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string ch)
        {
            return ch.Equals(TrueString);
        }

        return false;
    }
}