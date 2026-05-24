using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;

namespace BatchProcess3.Tools.Helper;

public static class ResourceHelper
{
    public static TopLevel? ResolveDefaultTopLevel()
    {
        return Application.Current?.ApplicationLifetime switch
        {
            IClassicDesktopStyleApplicationLifetime desktopLifetime => desktopLifetime.MainWindow,
            ISingleViewApplicationLifetime singleView => TopLevel.GetTopLevel(singleView.MainView),
            _ => null
        };
    }

    /// <summary>
    /// 查找资源（强泛型版本）
    /// </summary>
    public static T? FindResource<T>(string resourceKey)
    {
        var topLevel = ResolveDefaultTopLevel();
        return topLevel?.FindResource(resourceKey) is T resource ? resource : default;
    }

    /// <summary>
    /// 尝试查找资源（强泛型版本）
    /// </summary>
    public static bool TryFindResource<T>(string resourceKey)
    {
        var topLevel = ResolveDefaultTopLevel();
        return topLevel?.TryFindResource(resourceKey, out var value) == true && value is T resource;
    }

    /// <summary>
    /// 输出所有可用资源键（排查资源是否加载）
    /// </summary>
    public static void ShowAllResourceKeys()
    {
        var topLevel = ResolveDefaultTopLevel();
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