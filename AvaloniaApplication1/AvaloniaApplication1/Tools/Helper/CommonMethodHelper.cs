using System.Threading.Tasks;
using Avalonia.Input.Platform;

namespace AvaloniaApplication1.Tools.Helper;

public static class CommonMethodHelper
{
    public static void ClipboardSetTextAsync(string text)
    {
        // 获取剪贴板
        var clipboard = ResourceHelper.ResolveDefaultTopLevel()?.Clipboard;
        // 写入剪贴板
        clipboard?.SetTextAsync(text);
    }
}