using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace BatchProcess3.Controls.Converters;

public class BoolToBoldFontConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // return value is bool boolValue && boolValue ? FontWeight.Bold : FontWeight.Normal;
        return value is bool and true ? FontWeight.Bold : FontWeight.Normal;
        // return value is bool and true ? Application.Current?.FindResource("NotoSansJP-Bold") : Application.Current?.FindResource("NotoSansJP-Regular");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}