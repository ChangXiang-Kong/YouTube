using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace AvaloniaApplication1.Controls.Converters;

/// <summary>
/// str.Substring(0, length) 的转换器用法，参数 parameter 为 length，未指定时默认为 3
/// </summary>
public class SubstringConverter : IValueConverter
{
    private const int DefaultLength = 3;
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string str && !string.IsNullOrEmpty(str))
        {
            int length = parameter != null && int.TryParse(parameter.ToString(), out int len) ? len : DefaultLength;
            return str.Length > length ? str.Substring(0, length) : str;
        }
        return string.Empty;
    }
    
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException("反向转换未实现");
    }
}