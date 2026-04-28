using System;
using System.IO;
using Avalonia;
using Avalonia.Dialogs;
using Avalonia.Media;
using BatchProcess3.Data;

namespace BatchProcess3.Desktop;

internal class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            BuildAvaloniaApp()
                .With(new FontManagerOptions
                {
                    FontFallbacks =
                    [
                        new FontFallback
                        {
                            FontFamily = new FontFamily("Microsoft YaHei")
                        }
                    ]
                })
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            LogException(ex);
            // TODO: 弹出窗口提示异常信息
            throw;
        }
    }

    private static void LogException(Exception ex)
    {
        var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var logDirectory = Path.Combine(homeDirectory, Path.Combine(ResourceToken.AppName, "AppCrashLogs"));
        Directory.CreateDirectory(logDirectory);

        var logFileName = $"CrashLog_{DateTime.Now:yyyyMMdd_HHmmss}.log";
        var logFilePath = Path.Combine(logDirectory, logFileName);

        File.WriteAllText(logFilePath,
            $"CrashTime: {DateTime.Now}{Environment.NewLine}" +
            $"Exception Type: {ex.GetType().Name}{Environment.NewLine}" +
            $"Exception Message: {ex.Message}{Environment.NewLine}" +
            $"Stack Info: {Environment.NewLine}{ex.StackTrace}");
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UseManagedSystemDialogs()
            .UsePlatformDetect()
            .With(new Win32PlatformOptions())
            .WithInterFont()
            .LogToTrace();
}