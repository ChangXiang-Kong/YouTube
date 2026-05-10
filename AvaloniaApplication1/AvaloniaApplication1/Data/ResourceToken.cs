using System.Reflection;
using Avalonia;

namespace AvaloniaApplication1.Data;

public static class ResourceToken
{
    public static string AppName => "数字项目管理平台";
    public static string CompanyName => "这是公司名称";
    
    // 从 Avalonia 应用程序自动获取（更贴合框架），这个会读取你 App.axaml 里写的：<Application Name="BatchProcess3">
    public static string AppName1 { get; } = Application.Current?.Name ?? "UnknownApp";
    // 反射自动获取当前项目/程序集名称
    public static string AppName2 { get; } = Assembly.GetExecutingAssembly().GetName().Name!;
    
    
    
    
    
    
    
    
    
}