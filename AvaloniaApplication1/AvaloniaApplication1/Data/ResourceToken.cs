using System.Reflection;
using Avalonia;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;

namespace AvaloniaApplication1.Data;

public static class ResourceToken
{
    public static string AppName => "数字项目管理平台";
    public static string CompanyName => "这是公司名称";
    
    // 从 Avalonia 应用程序自动获取（更贴合框架），这个会读取你 App.axaml 里写的：<Application Name="BatchProcess3">
    public static readonly string AppName1 = Application.Current?.Name ?? "UnknownApp";
    // public static string AppName1 { get; } = Application.Current?.Name ?? "UnknownApp";
    // 反射自动获取当前项目/程序集名称
    public static readonly string AppName21 = Assembly.GetExecutingAssembly().GetName().Name!;
    
    
    // SemiIcon
    public static readonly DynamicResourceExtension DynamicResourceExtensionSemiIconChevronLeft = new("SemiIconChevronLeft");
    public static readonly DynamicResourceExtension DynamicResourceExtensionSemiIconChevronRight = new("SemiIconChevronRight");
    public static readonly DynamicResourceExtension DynamicResourceExtensionSemiIconChevronUp = new("SemiIconChevronUp");
    public static readonly DynamicResourceExtension DynamicResourceExtensionSemiIconChevronDown = new("SemiIconChevronDown");
    public static readonly DynamicResourceExtension DynamicResourceExtensionSemiIconClose = new("SemiIconClose");
    // Button Style
    public static readonly DynamicResourceExtension DynamicResourceExtensionSolidButton = new("SolidButton");
    public static readonly DynamicResourceExtension DynamicResourceExtensionOutlineButton = new("OutlineButton");
    public static readonly DynamicResourceExtension DynamicResourceExtensionBorderlessIconButton = new("BorderlessIconButton");
    // Color
    public static readonly DynamicResourceExtension DynamicResourceExtensionSemiGreen2 = new("SemiGreen2");
    public static readonly DynamicResourceExtension DynamicResourceTextBlockTertiaryForeground = new("TextBlockTertiaryForeground");
    public static readonly DynamicResourceExtension DynamicResourceTextBlockDefaultForeground = new("TextBlockDefaultForeground");
    public static readonly DynamicResourceExtension DynamicResourceTextBlockInfoForeground = new("TextBlockInfoForeground");
    public static readonly DynamicResourceExtension DynamicResourceTextBlockSuccessForeground = new("TextBlockSuccessForeground");
    public static readonly DynamicResourceExtension DynamicResourceTextBlockWarningForeground = new("TextBlockWarningForeground");
    public static readonly DynamicResourceExtension DynamicResourceTextBlockDangerForeground = new("TextBlockDangerForeground");
    // Thickness
    public static readonly Thickness BottomBarToggleTrueMargin = new(20, 0, 0, -10);
    public static readonly Thickness BottomBarToggleFalseMargin = new(20, 0, 0, -41);
    // Geometry
    public static readonly StreamGeometry GeometryAsk = StreamGeometry.Parse("M512 0 30.11843 240.941297l0 542.117406 481.88157 240.941297 481.88157-240.941297L993.88157 240.941297 512 0zM575.776472 768.799969 460.188012 768.799969 460.188012 656.222073l115.588459 0L575.776472 768.799969zM623.335603 509.329685c-52.375829 36.723353-59.600363 55.988096-59.600363 84.885211l0 19.866447L468.616977 614.081343l0-26.489278c0-45.754021 13.846342-80.67124 61.406497-116.791866 46.957428-36.723353 57.79423-62.0082 57.79423-84.282484 0-25.284848-21.67258-54.181962-55.386393-54.181962-42.743457 0-70.436142 26.489278-82.477374 85.486914l-105.956088-21.67258c24.683144-111.976192 82.477374-157.127486 205.289345-157.127486 98.12985 0 157.72919 63.212631 157.72919 131.842639C707.017407 423.240044 688.956071 461.76953 623.335603 509.329685z");

    
    
    
    
    
    
    
}