using Avalonia;
using System;
using System.Threading;
using System.Threading.Tasks;
using BatchProcess3.Core;
using BatchProcess3.Core.SolidWorksHost;
using BatchProcess3.Host.SolidWorksHost;
using BatchProcess3.Host.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
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
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<BatchProcessHost>();
        
        
        builder.WebHost.UseUrls($"http://localhost:{BatchProcessHostUrls.DefaultPort}");

        WebApp = builder.Build();
        
        WebApp.MapGet("/", ([FromServices] MainViewModel viewModel) => $"Hello! {viewModel.Greeting}");
        
        
        

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