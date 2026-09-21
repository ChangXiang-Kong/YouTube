using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;

namespace BatchProcess3.Tools.Converters;

/// <summary>
/// 多个 bool 值中的任意一个为 true，则返回 true。<br/>
/// 用于实现 IsVisible="{Binding $parent[ListBoxItem].IsPointerOver || $parent[ListBoxItem].IsSelected}" 效果
/// </summary>
public class BoolOrMultiConverter: IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        // 只要有一个 true，返回 true
        return values.OfType<bool>().Any(b => b);
    }

    public object[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}