using System;
using System.Globalization;
using System.IO;
using Avalonia.Data.Converters;

namespace BatchProcess3.Tools.Converters;

public class FileNameConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is string path && !string.IsNullOrWhiteSpace(path)
            ? Path.GetFileNameWithoutExtension(path) 
            : value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}