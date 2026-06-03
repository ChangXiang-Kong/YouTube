using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml.MarkupExtensions;
using AvaloniaApplication1.Tools.ListBoxLog;

namespace AvaloniaApplication1.Controls.Converters;

public class LogTypeToForegroundConverter : IValueConverter
{
    private readonly DynamicResourceExtension _dynamicResourceTextBlockTertiaryForeground = new("TextBlockTertiaryForeground");
    private readonly DynamicResourceExtension _dynamicResourceTextBlockDefaultForeground = new("TextBlockDefaultForeground");
    private readonly DynamicResourceExtension _dynamicResourceTextBlockInfoForeground = new("TextBlockInfoForeground");
    private readonly DynamicResourceExtension _dynamicResourceTextBlockSuccessForeground = new("TextBlockSuccessForeground");
    private readonly DynamicResourceExtension _dynamicResourceTextBlockWarningForeground = new("TextBlockWarningForeground");
    private readonly DynamicResourceExtension _dynamicResourceTextBlockDangerForeground = new("TextBlockDangerForeground");
    private readonly DynamicResourceExtension _dynamicResourceTextBlockFatalForeground = new("SemiAIPurple5");
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
            return _dynamicResourceTextBlockDefaultForeground;

        return (LogType)value switch
        {
            LogType.Tip => _dynamicResourceTextBlockTertiaryForeground,
            LogType.Default => _dynamicResourceTextBlockDefaultForeground,
            LogType.Info => _dynamicResourceTextBlockInfoForeground,
            LogType.Success => _dynamicResourceTextBlockSuccessForeground,
            LogType.Warning => _dynamicResourceTextBlockWarningForeground,
            LogType.Error => _dynamicResourceTextBlockDangerForeground,
            LogType.Fatal => _dynamicResourceTextBlockFatalForeground,
            _ => _dynamicResourceTextBlockDefaultForeground
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException("反向转换未实现");
    }
}