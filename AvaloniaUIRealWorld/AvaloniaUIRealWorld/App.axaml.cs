using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Metadata;
using AvaloniaUIRealWorld.Data;
using AvaloniaUIRealWorld.Factories;
using AvaloniaUIRealWorld.ViewModels;
using AvaloniaUIRealWorld.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using AvaloniaUIRealWorld.Data.EnumValues;

// 自定义 XML Namespace 参考链接：https://docs.avaloniaui.net/docs/guides/custom-controls/how-to-create-a-custom-controls-library#xml-namespace-definitions
// 参考视频：https://www.youtube.com/watch?v=M3CFj0x-tts&list=PLrW43fNmjaQWwIdZxjZrx5FSXcNzaucOO&index=7
[assembly: XmlnsDefinition("https://github.com/avaloniaui", "AvaloniaUIRealWorld.Controls")]

namespace AvaloniaUIRealWorld
{
    public partial class App : Application
    {
        // Some code here

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var collection = new ServiceCollection();
            // 依赖注入
            collection.AddSingleton<MainViewModel>();
            collection.AddTransient<HomePageViewModel>();
            collection.AddTransient<ProcessPageViewModel>();
            collection.AddTransient<ActionsPageViewModel>();
            collection.AddTransient<MacrosPageViewModel>();
            collection.AddTransient<ReporterPageViewModel>();
            collection.AddTransient<HistoryPageViewModel>();
            collection.AddTransient<SettingsPageViewModel>();
            collection.AddTransient<ErrorPageViewModel>();

            collection.AddSingleton<Func<ApplicationPageNames, PageViewModel>>(x => name => name switch
            {
                ApplicationPageNames.Home => x.GetRequiredService<HomePageViewModel>(),
                ApplicationPageNames.Process => x.GetRequiredService<ProcessPageViewModel>(),
                ApplicationPageNames.Macros => x.GetRequiredService<MacrosPageViewModel>(),
                ApplicationPageNames.Actions => x.GetRequiredService<ActionsPageViewModel>(),
                ApplicationPageNames.Reporter => x.GetRequiredService<ReporterPageViewModel>(),
                ApplicationPageNames.History => x.GetRequiredService<HistoryPageViewModel>(),
                ApplicationPageNames.Settings => x.GetRequiredService<SettingsPageViewModel>(),
                ApplicationPageNames.Error => x.GetRequiredService<ErrorPageViewModel>(),
                _ => throw new InvalidOperationException(),
            });

            collection.AddSingleton<PageFactory>();

            var services = collection.BuildServiceProvider();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainView()
                {
                    // DataContext = services.GetService<MainViewModel>()          // 参数 T 可为空，为空时不会报错
                    DataContext = services.GetRequiredService<MainViewModel>()  // 参数 T 为空时报错
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}