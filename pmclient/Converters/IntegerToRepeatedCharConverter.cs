using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace pmclient.Converters;

public class IntegerToRepeatedCharConverter : IValueConverter
{
    public char CharToRepeat { get; set; } = '*';

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int count && count >= 0)
        {
            return new string(CharToRepeat, count);
        }

        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string str)
        {
            return str.Length;
        }

        return 0;
    }
}