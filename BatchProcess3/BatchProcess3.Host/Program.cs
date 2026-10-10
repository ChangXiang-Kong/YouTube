using Avalonia;
using System;
using System.Threading;
using System.Threading.Tasks;
using BatchProcess3.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace BatchProcess3.Host;

sealed class Program
{
    private static CancellationTokenSource _cts = new CancellationTokenSource();

    public static WebApplication? WebApp { get; private set; }
    
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Inject services
        
        
        
        builder.WebHost.UseUrls($"http://0.0.0.0:{BatchProcessHostUrls.DefaultPort}");

        WebApp = builder.Build();
        
        WebApp.MapGet("/", () => "Hello World!");
        
        
        

        // Start kestrel on background thread
        Task.Run(() => WebApp.RunAsync(_cts.Token));

        try
        {
            // Run Avalonia
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        finally
        {
            // Close kestrel
            _cts.Cancel();
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
// #if DEBUG
//             .WithDeveloperTools()
// #endif
            .WithInterFont()
            .LogToTrace();
}