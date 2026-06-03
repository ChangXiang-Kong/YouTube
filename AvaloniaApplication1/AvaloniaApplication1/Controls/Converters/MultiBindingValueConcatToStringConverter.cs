using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Data.Converters;

namespace AvaloniaApplication1.Controls.Converters;

/// <summary>
/// 多值拼接字符串转换器，默认分隔符：" -- "<br/>
/// 将多个简单值（string, int, ...）拼接为字符串（"aa -- bb -- cc"），若合并失败则返回异常信息字符串
/// </summary>
public class MultiBindingValueConcatToStringConverter : IMultiValueConverter
{
    private const string DefaultSeparator = " -- ";
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="values"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter">分隔符</param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        try
        {
            if (!(values.Count > 0))    // if (values is not { Count: > 0 })
                return null;
            
            // 取分隔符：优先使用ConverterParameter
            var separator = parameter as string ?? DefaultSeparator;
            
            // 过滤：跳过UnsetValue、null，取有效ToString
            var validItems = values
                .Where(v => !IsUnsetValue(v))
                .Select(v => v.ToString())
                .Where(s => s is not null);

            if (!validItems.Any())
                return null;

            return string.Join(separator, validItems);
            
            
            /* 其他参考示例
            if (!(values.Count > 0))    // if (values is not { Count: > 0 })
                return null;
    
            // TODO: 可能有很多值都是 UnsetValueType
            if (values[0] is UnsetValueType)
                return null;
            if (values[1] is UnsetValueType)
                return null;
            
            if (values.Count < 2)
                return values[0]?.ToString();
            
            // 取分隔符：优先使用ConverterParameter
            var separator = parameter as string ?? DefaultSeparator;
    
            // 方式一：Join（最简洁，推荐）
            string str = string.Join(separator, values.Select(v => v?.ToString() ?? "Null"));
            // string strr = string.Join(separator, values.Where(v => v != null).Select(v => v!.ToString()));  // 过滤掉null值
    
            // 方式二：使用Aggregate但删除最后的字符
            // string str = values.Aggregate("", (current, value) => current + $"{value?.ToString()} -- ");
            // if (str.EndsWith(separator))
            //     str = str.Substring(0, str.Length - 3);
             
            // 原始方式：
            // string str = "";
            // foreach (var value in values)
            // {
            //     str += $"{value?.ToString()}  ";
            // }
            // if (str.EndsWith(separator))
            //     str = str.Substring(0, str.Length - 3);
            
            return str;
            */
        }
        catch (Exception e)
        {
            return $"{nameof(MultiBindingValueConcatToStringConverter)}.Convert() throw an exception{Environment.NewLine + e.Message}";
        }
    }
    
    /// <summary>
    /// 判断是否是Avalonia未赋值标记UnsetValue
    /// </summary>
    private static bool IsUnsetValue(object? value)
    {
        if (value is null) return false;
        return value == AvaloniaProperty.UnsetValue || value.GetType().Name == nameof(UnsetValueType);
    }
    
    public object? ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException("反向转换未实现");
    }
}