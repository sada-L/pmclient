using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace pmclient.Converters;

public class BoolToBrushConverter : IValueConverter
{
    public IBrush TrueBrush { get; set; } = Brushes.Green;
    public IBrush FalseBrush { get; set; } = Brushes.Red;
    public IBrush NullBrush { get; set; } = Brushes.Gray;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            true => TrueBrush,
            false => FalseBrush,
            _ => NullBrush
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is IBrush brush)
        {
            return brush.Equals(TrueBrush);
        }

        return false;
    }
}