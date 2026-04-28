using System.Reflection;
using Avalonia;

namespace BatchProcess3.Data;

public static class ResourceToken
{
    // 从 Avalonia 应用程序自动获取（更贴合框架），这个会读取你 App.axaml 里写的：<Application Name="BatchProcess3">
    public static string AppName { get; } = Application.Current?.Name ?? "UnknownApp";
    // 反射自动获取当前项目/程序集名称
    public static string AppName1 { get; } = Assembly.GetExecutingAssembly().GetName().Name!;
    
    
    
}