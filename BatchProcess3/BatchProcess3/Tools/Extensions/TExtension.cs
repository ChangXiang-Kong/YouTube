using System;
using System.ComponentModel;
using System.Reflection;
using Avalonia.Media;
using BatchProcess3.CustomAttributes;

namespace BatchProcess3.Tools.Extensions;

public static class TExtension
{
    /// <summary>
    /// 主要用于 GeometryIcon 枚举类型<br/>
    /// 获取 T 的特性 GeometryIconAttribute，并返回该特性的字符串数组[Message, ColorStr, GeometryPath]<br/>
    /// 当 T 没有该特性时，返回默认字符串数组
    /// </summary>
    /// <code>
    /// 示例：
    /// [TextBlock Text="{Binding IconMessage}" />
    /// [Viewbox Width="{Binding IconWidth}" Height="{Binding IconHeight}"  Margin="10,0,0,0" VerticalAlignment="Center">
    ///     [PathIcon Data="{Binding IconGeometry}" Foreground="{Binding IconForeground}" />
    /// [/Viewbox>
    ///  
    /// [ObservableProperty] private double _iconWidth = 40;
    /// [ObservableProperty] private double _iconHeight = 40;
    /// [ObservableProperty] private string _iconMessage = "";
    /// [ObservableProperty] private string _iconForeground = "";
    /// [ObservableProperty] private StreamGeometry _iconGeometry;
    ///  
    /// string[] strArray = GeometryIcon.Success.ParseGeometryIconAttribute()
    /// IconMessage = strArray[0]; 
    /// IconForeground = strArray[1]; 
    /// IconGeometry = StreamGeometry.Parse(strArray[2]);
    /// </code>
    /// <param name="obj"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static string[] ParseGeometryIconAttribute<T>(this T? obj)
    {
        string[] defaultResult =
        [
            "Param obj is null or No GeometryIconAttribute found", 
            "#c738ff",
            "M855.6 427.2H168.5c-12.7 0-24.4 6.9-30.6 18L4.4 684.7C1.5 689.9 0 695.8 0 701.8v287.1c0 19.4 15.7 35.1 35.1 35.1H989c19.4 0 35.1-15.7 35.1-35.1V701.8c0-6-1.5-11.8-4.4-17.1L886.2 445.2c-6.2-11.1-17.9-18-30.6-18zM673.4 695.6c-16.5 0-30.8 11.5-34.3 27.7-12.7 58.5-64.8 102.3-127.2 102.3s-114.5-43.8-127.2-102.3c-3.5-16.1-17.8-27.7-34.3-27.7H119c-26.4 0-43.3-28-31.1-51.4l81.7-155.8c6.1-11.6 18-18.8 31.1-18.8h622.4c13 0 25 7.2 31.1 18.8l81.7 155.8c12.2 23.4-4.7 51.4-31.1 51.4H673.4zM819.9 209.5c-1-1.8-2.1-3.7-3.2-5.5-9.8-16.6-31.1-22.2-47.8-12.6L648.5 261c-17 9.8-22.7 31.6-12.6 48.4 0.9 1.4 1.7 2.9 2.5 4.4 9.5 17 31.2 22.8 48 13L807 257.3c16.7-9.7 22.4-31 12.9-47.8zM375.4 261.1L255 191.6c-16.7-9.6-38-4-47.8 12.6-1.1 1.8-2.1 3.6-3.2 5.5-9.5 16.8-3.8 38.1 12.9 47.8L337.3 327c16.9 9.7 38.6 4 48-13.1 0.8-1.5 1.7-2.9 2.5-4.4 10.2-16.8 4.5-38.6-12.4-48.4zM512 239.3h2.5c19.5 0.3 35.5-15.5 35.5-35.1v-139c0-19.3-15.6-34.9-34.8-35.1h-6.4C489.6 30.3 474 46 474 65.2v139c0 19.5 15.9 35.4 35.5 35.1h2.5z"
        ];
        
        if (obj == null)
            return defaultResult;
        
        // 获取枚举类型
        Type type = obj.GetType();
        
        // 获取当前枚举值对应的 字段信息
        FieldInfo? field = type.GetField(obj.ToString()!);
        
        if (field == null)
            return defaultResult;
        
        // 从字段获取特性
        GeometryIconAttribute? attr = field.GetCustomAttribute<GeometryIconAttribute>(inherit: false);
        
        if (attr == null)
            return defaultResult;

        return [attr.Message, attr.ColorStr, attr.GeometryPath];
    }
}