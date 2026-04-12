using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Metadata;
using BatchProcess3.Data;
using BatchProcess3.Factories;
using BatchProcess3.ViewModels;
using BatchProcess3.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using BatchProcess3.Data;
using BatchProcess3.Extensions;
using BatchProcess3.ViewModels.Actions;
using BatchProcess3.Views.Actions;

// 自定义 XML Namespace 参考链接：https://docs.avaloniaui.net/docs/guides/custom-controls/how-to-create-a-custom-controls-library#xml-namespace-definitions
// 参考视频：https://www.youtube.com/watch?v=M3CFj0x-tts&list=PLrW43fNmjaQWwIdZxjZrx5FSXcNzaucOO&index=7
[assembly: XmlnsDefinition("https://github.com/avaloniaui", "BatchProcess3.Controls")]

namespace BatchProcess3
{
    public partial class App : Application
    {
        // Some code here
        public new static App? Current => Application.Current as App;
        /* 详解以上代码中 new 关键字的作用
        这段代码里的 new 是 C# 中的【隐藏（Hide）】关键字（也叫 “遮蔽”），核心作用是【显式覆盖基类 / 父类中同名的 Current 成员】，告诉编译器 “我故意用子类的这个静态属性覆盖基类的同名成员，不是无意冲突”。
        一、先理解代码背景
        要搞懂 new 的作用，首先要明确这段代码的上下文：
            Application 是 Avalonia/WPF 等框架中的基类，它本身有一个静态属性 Application.Current（返回 Application 类型）；
            你自定义的 App 类继承自 Application，现在想定义一个 App.Current 属性，返回强类型的 App 实例（而非基类的 Application），避免每次使用都手动转换。
        二、new 关键字的核心作用
        1. 语法层面：显式隐藏基类同名成员
            C# 中如果子类定义了和基类同名的静态成员（属性 / 方法），编译器会默认提示 “你可能无意中隐藏了基类成员”，而 new 关键字的作用就是：
            告诉编译器：“我明确要隐藏基类的 Current 属性，这是故意的，不是编码错误”；
            消除编译器的警告（如果不加 new，编译器会报 CS0108 警告：“App.Current 隐藏了继承的成员 Application.Current，若要隐藏请使用 new 关键字”）。
        2. 逻辑层面：重定义成员的行为
            基类 Application.Current 返回的是 Application? 类型，而你定义的 App.Current 通过 new 隐藏后：
            返回类型变为 App?（强类型），无需每次使用时手动写 (App)Application.Current；
            调用 App.Current 时，会优先执行子类的这个属性逻辑，而非基类的 Application.Current。
        三、对比 “有无 new” 的区别
        1. 不加 new 的情况
            // 编译器会报 CS0108 警告：隐藏了基类成员
            public static App? Current => Application.Current as App;
            · 功能上能运行，但编译器会提示警告，暗示你 “可能不小心写了同名成员”；
            · 代码可读性差，其他开发者看到时，无法判断你是故意覆盖还是无意冲突。
        2. 加 new 的情况
            // 无警告，显式声明“隐藏基类成员”
            public new static App? Current => Application.Current as App;
            · 消除编译器警告，明确表达 “我就是要替换基类的 Current 属性”；
            · 语义清晰，团队协作时能快速理解这是 “强类型封装基类 Current” 的设计意图。
        四、关键补充：new 隐藏 vs override 重写
            很多人会混淆 new（隐藏）和 override（重写），这里特别说明：
            特性	    new（隐藏）	                        override（重写）
            适用成员	静态成员 / 实例成员（无 virtual）	   仅实例成员（基类需标记 virtual/abstract）
            绑定方式	编译时绑定（调用哪个版本看变量声明类型）  运行时绑定（调用哪个版本看对象实际类型）
            基类要求	无需基类做任何标记	                   基类成员必须标记 virtual/abstract
            ⚠️ 注意：你这段代码中 Current 是静态属性，静态成员不支持 override（因为静态成员属于类，而非实例），所以只能用 new 隐藏。
        五、实际使用场景示例
        假设你自定义的 App 类有一个专属属性 MyCustomConfig，通过这个 new 修饰的 Current 可以直接访问：
            // 自定义 App 类
            public class App : Application
            {
                public string MyCustomConfig { get; set; } = "自定义配置";
                
                // 用 new 隐藏基类的 Current
                public new static App? Current => Application.Current as App;
            }

            // 调用时（无需手动转换类型）
            var config = App.Current?.MyCustomConfig; // 直接访问 App 类的专属属性
            // 如果没有这个 new 属性，需要写：
            var config = (Application.Current as App)?.MyCustomConfig;
        总结
        关键点回顾
            核心作用：new 是 “显式隐藏” 关键字，用于覆盖基类（Application）中同名的 Current 静态属性，消除编译器警告；
            语义价值：明确表达 “故意替换基类成员” 的设计意图，提升代码可读性；
            使用限制：静态成员只能用 new 隐藏，无法用 override 重写（override 仅适用于实例的虚方法 / 属性）。
            简单来说，这个 new 就是告诉编译器和其他开发者：“我知道基类有个 Current，我就是要重新定义一个更贴合 App 类的 Current，别提示我警告”。
         */

        /// <summary>
        /// View与ViewModel映射，[typeof(ViewModel), typeof(View)]<br/>
        /// 因为 ViewLocator.Build(object? data) 参数 data 为 ViewModel，所以这里的映射关系顺序为 [typeof(ViewModel), typeof(View)]
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
            
            // 使用 PageFactory0 或 PageFactory1 时的依赖注入写法
            // services.AddSingleton<Func<ApplicationPageName, PageViewModel>>(x => name => name switch
            // {
            //     // Menu 相关
            //     ApplicationPageName.Home => x.GetRequiredService<HomePageViewModel>(),
            //     ApplicationPageName.Process => x.GetRequiredService<ProcessPageViewModel>(),
            //     ApplicationPageName.Macros => x.GetRequiredService<MacrosPageViewModel>(),
            //     ApplicationPageName.Actions => x.GetRequiredService<ActionsPageViewModel>(),
            //     ApplicationPageName.Reporter => x.GetRequiredService<ReporterPageViewModel>(),
            //     ApplicationPageName.History => x.GetRequiredService<HistoryPageViewModel>(),
            //     ApplicationPageName.Settings => x.GetRequiredService<SettingsPageViewModel>(),
            //     _ => throw new InvalidOperationException(),
            // });
            // 使用 PageFactory 时的依赖注入写法
            services.AddSingleton<Func<Type, PageViewModel>>(x => type => type switch
            {
                // Menu 相关
                _ when type == typeof(HomePageViewModel) => x.GetRequiredService<HomePageViewModel>(),
                _ when type == typeof(ProcessPageViewModel) => x.GetRequiredService<ProcessPageViewModel>(),
                _ when type == typeof(MacrosPageViewModel) => x.GetRequiredService<MacrosPageViewModel>(),
                _ when type == typeof(ActionsPageViewModel) => x.GetRequiredService<ActionsPageViewModel>(),
                _ when type == typeof(ReporterPageViewModel) => x.GetRequiredService<ReporterPageViewModel>(),
                _ when type == typeof(HistoryPageViewModel) => x.GetRequiredService<HistoryPageViewModel>(),
                _ when type == typeof(SettingsPageViewModel) => x.GetRequiredService<SettingsPageViewModel>(),
                _ => throw new InvalidOperationException(),
            });
        }
    }
}