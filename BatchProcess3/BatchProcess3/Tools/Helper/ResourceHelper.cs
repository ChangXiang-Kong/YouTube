using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace BatchProcess3.Tools.Helper;

public static class ResourceHelper
{
    /// <summary>
    /// 获取默认的 TopLevel（MainWindow 或 MainView）
    /// </summary>
    /// <returns></returns>
    public static TopLevel? GetDefaultTopLevel()
    {
        var app = Application.Current;
        if (app is null)
            return null;
        
        return app.ApplicationLifetime switch
        {
            IClassicDesktopStyleApplicationLifetime desktopLifetime => TopLevel.GetTopLevel(desktopLifetime.MainWindow),
            ISingleViewApplicationLifetime singleView => TopLevel.GetTopLevel(singleView.MainView),
            _ => null
        };
    }
    
    /// <summary>
    /// 先从 MainWindow 或 MainView 搜索资源，若未找到则从 Application 全局资源搜索，若都未找到则返回 default
    /// </summary>
    public static T? FindResource<T>(string resourceKey, bool searchFromApplication = false)
    {
        if (!searchFromApplication)
        {
            var topLevel = GetDefaultTopLevel();
            if (topLevel is null)
                return default;

            // 从当前visual向上找TopLevel，优先使用控件上下文查找
            // 从主窗口 / 主 View（TopLevel）开始向上搜索资源，能读到 窗口级资源，受当前窗口局部主题覆盖；
            var resFromTopLevel = topLevel.FindResource(resourceKey);   // TopLevel.FindResource 默认使用 ActualThemeVariant（当前窗口实际生效主题）
            if (resFromTopLevel is T typedRes)  // 等于 if (resFromTopLevel is T typedRes && typedRes != null)
                return typedRes;
        }
        
        var app =  Application.Current;
        if (app is null)
            return default;
        
        return app.FindResource(app.ActualThemeVariant, resourceKey) is T res ? res : default;
    }

    /// <summary>
    /// 先从指定Visual上下文查找资源（和XAML原生查找逻辑完全一致）， 使用该Visual的 ActualThemeVariant，向上遍历逻辑树资源字典，
    /// 当Visual为null时，降级到Application全局资源
    /// </summary>
    /// <typeparam name="T">资源目标类型</typeparam>
    /// <param name="visual">UI控件/Visual实例</param>
    /// <param name="resourceKey">资源Key</param>
    /// <param name="searchFromApplication">false 从当前 Visual搜索，true 直接从  Application 根节点搜索</param>
    /// <returns>找到返回实例，找不到返回default，不会抛出异常</returns>
    public static T? FindResource<T>(Visual? visual, string resourceKey, bool searchFromApplication = false)
    {
        // Avalonia 资源查找规则：调用节点向上遍历逻辑树，找到第一个匹配 key 的资源就返回；越靠近调用节点优先级越高Avalonia

        if (!searchFromApplication)
        {
            if (visual is null)
                return default;

            // 从当前visual向上找TopLevel，优先使用控件上下文查找
            // 从主窗口 / 主 View（TopLevel）开始向上搜索资源，能读到 窗口级资源，受当前窗口局部主题覆盖；
            var topLevel = TopLevel.GetTopLevel(visual);
            if (topLevel is not null)
            {
                var resFromTopLevel = topLevel.FindResource(resourceKey);   // TopLevel.FindResource 默认使用 ActualThemeVariant（当前窗口实际生效主题）
                if (resFromTopLevel is T typedRes)  // 等于 if (resFromTopLevel is T typedRes && typedRes != null)
                    return typedRes;
            }
        }
        
        // 兜底：App全局资源
        // 直接从 Application 根节点搜索，看不到 Window 级别资源，只能读取 App 全局资源，使用 应用全局ActualThemeVariant
        var app =  Application.Current;
        if (app is null)
            return default;
        
        return app.FindResource(app.ActualThemeVariant, resourceKey) is T res ? res : default;
    }

    /// <summary>
    /// 输出所有可用资源键（排查资源是否加载）
    /// </summary>
    public static void ShowAllResourceKeys()
    {
        var topLevel = GetDefaultTopLevel();
        var resources = topLevel?.Resources;
        var resourceKeys = topLevel?.Resources.Keys;
        var mergedDictionaries = topLevel?.Resources.MergedDictionaries;
        if (resourceKeys != null)
        {
            foreach (var resourcesKeys in resourceKeys)
            {
                System.Diagnostics.Debug.WriteLine($"全局资源键：{resourcesKeys}");
            }
        }
        if (mergedDictionaries != null)
        {
            foreach (var resourcesKeys in mergedDictionaries)
            {
                System.Diagnostics.Debug.WriteLine($"合并资源键：{resourcesKeys}");
            }
        }
    }
}