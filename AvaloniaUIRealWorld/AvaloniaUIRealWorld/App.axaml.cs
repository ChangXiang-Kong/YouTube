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
using System.Collections.Generic;
using AvaloniaUIRealWorld.Data.EnumValues;
using AvaloniaUIRealWorld.Extensions;
using AvaloniaUIRealWorld.ViewModels.Actions;
using AvaloniaUIRealWorld.Views.Actions;

// 自定义 XML Namespace 参考链接：https://docs.avaloniaui.net/docs/guides/custom-controls/how-to-create-a-custom-controls-library#xml-namespace-definitions
// 参考视频：https://www.youtube.com/watch?v=M3CFj0x-tts&list=PLrW43fNmjaQWwIdZxjZrx5FSXcNzaucOO&index=7
[assembly: XmlnsDefinition("https://github.com/avaloniaui", "AvaloniaUIRealWorld.Controls")]

namespace AvaloniaUIRealWorld
{
    public partial class App : Application
    {
        // Some code here
        public new static App? Current => Application.Current as App;

        /// <summary>
        /// View与ViewModel映射，[typeof(ViewModel), typeof(View)]
        /// </summary>
        public Dictionary<Type, Type> ViewModelMappings { get; } = new();

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var services = new ServiceCollection();
            // =====  依赖注入 =====
            RegisterViewModels(services);
            RegisterServices(services);

            var serviceProvider = services.BuildServiceProvider();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow()
                {
                    // DataContext = serviceProvider.GetService<MainViewModel>()          // 参数 T 可为空，为空时不会报错
                    DataContext = serviceProvider.GetRequiredService<MainViewModel>() // 参数 T 为空时报错
                };
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
            {
                singleViewPlatform.MainView = new MainView()
                {
                    DataContext = new MainViewModel()
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void RegisterViewModels(IServiceCollection services)
        {
            // Menu 相关
            services.AddSingletonViewModel<MainView, MainViewModel>();
            services.AddTransientViewModel<HomePageView, HomePageViewModel>();
            services.AddTransientViewModel<ProcessPageView, ProcessPageViewModel>();
            services.AddTransientViewModel<ActionsPageView, ActionsPageViewModel>();
            services.AddTransientViewModel<MacrosPageView, MacrosPageViewModel>();
            services.AddTransientViewModel<ReporterPageView, ReporterPageViewModel>();
            services.AddTransientViewModel<HistoryPageView, HistoryPageViewModel>();
            services.AddTransientViewModel<SettingsPageView, SettingsPageViewModel>();
        }

        private void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<PageFactory>();

            services.AddSingleton<Func<ApplicationPageNames, PageViewModel>>(x => name => name switch
            {
                // Menu 相关
                ApplicationPageNames.Home => x.GetRequiredService<HomePageViewModel>(),
                ApplicationPageNames.Process => x.GetRequiredService<ProcessPageViewModel>(),
                ApplicationPageNames.Macros => x.GetRequiredService<MacrosPageViewModel>(),
                ApplicationPageNames.Actions => x.GetRequiredService<ActionsPageViewModel>(),
                ApplicationPageNames.Reporter => x.GetRequiredService<ReporterPageViewModel>(),
                ApplicationPageNames.History => x.GetRequiredService<HistoryPageViewModel>(),
                ApplicationPageNames.Settings => x.GetRequiredService<SettingsPageViewModel>(),
                _ => throw new InvalidOperationException(),
            });
        }
    }
}