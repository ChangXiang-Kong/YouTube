using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace BatchProcess3.Tools.Converters;

public class BoolToBoldFontConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // return value is bool boolValue && boolValue ? FontWeight.Bold : FontWeight.Normal;
        return value is bool and true ? FontWeight.Bold : FontWeight.Normal;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}