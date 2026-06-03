using Avalonia.Input.Platform;
using AvaloniaApplication1.Tools.Helper;

namespace AvaloniaApplication1.Tools.Extensions;

public static class StringExtension
{
    /// <summary>
    /// 将输入字符串写入剪贴板
    /// </summary>
    /// <param name="text">输入字符串</param>
    public static void ClipboardSetTextAsync(this string text)
    {
        // 获取剪贴板
        var clipboard = ResourceHelper.ResolveDefaultTopLevel()?.Clipboard;
        // 写入剪贴板
        clipboard?.SetTextAsync(text);
    }
    
    
}