using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using Avalonia.Metadata;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Tools.Extensions;
using AvaloniaApplication1.Tools.Factories;
using AvaloniaApplication1.Tools.Services;
using AvaloniaApplication1.ViewModels;
using AvaloniaApplication1.Views;
using ClassicDiagnostics.Avalonia;
using Microsoft.Extensions.DependencyInjection;
using AppSettings = AvaloniaApplication1.Models.AppSettings;

// 自定义 XML Namespace 参考链接：https://docs.avaloniaui.net/docs/guides/custom-controls/how-to-create-a-custom-controls-library#xml-namespace-definitions
// 参考视频：https://www.youtube.com/watch?v=M3CFj0x-tts&list=PLrW43fNmjaQWwIdZxjZrx5FSXcNzaucOO&index=7
[assembly: XmlnsDefinition("https://github.com/avaloniaui", "AvaloniaApplication1.Controls")]
// [assembly: XmlnsPrefix("https://irihi.tech/semi", "semi")]
// [assembly: XmlnsDefinition("https://irihi.tech/semi", "Semi.Avalonia")]

namespace AvaloniaApplication1;

public partial class App : Application
{
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
    public static Dictionary<Type, Type> ViewModelMappings { get; } = new();

    // NavDemo -- 全局单例：导航服务（不使用依赖注入的方式，原因参考下面的说明）
    public static INavigationService? NavigationService { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
#if DEBUG
        /* 第三方F12诊断工具：
        光头哥的 ProDiagnostics,
        群友移植老版本的 ClassicDiagnostics.Avalonia
         */
        // this.AttachDeveloperTools();    // AvaloniaUI.DiagnosticsSupport
        this.AttachDevTools();          // ClassicDiagnostics.Avalonia
#endif
        this.DataContext = new ApplicationViewModel();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        // =====  依赖注入 =====
        RegisterViewModels(services);
        RegisterServices(services);
        var serviceProvider = services.BuildServiceProvider();
        
        // NavDemo
        // 要在 desktop.MainWindow = new MainWindow() 之前获取 NavigationService，
        // 因为 MainWindow.axaml.cs 在初始化导航服务时需要调用 App.NavigationService?.Initialize(MainNav);
        NavigationService = new NavigationService(serviceProvider.GetRequiredService<PageNavigationFactory>());

        switch (ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktop:
                // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
                // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
                // DisableAvaloniaDataAnnotationValidation();

                // Line below is needed to remove Avalonia data validation.
                // Without this line you will get duplicate validations from both Avalonia and CT
                // BindingPlugins.DataValidators.RemoveAt(0);

                desktop.MainWindow = new MainWindow
                {
                    // DataContext = serviceProvider.GetService<MainViewModel>()          // 参数 T 可为空，为空时不会报错
                    DataContext = serviceProvider.GetRequiredService<MainViewModel>() // 参数 T 为空时报错
                };
                break;
            case ISingleViewApplicationLifetime singleViewPlatform:
                singleViewPlatform.MainView = new MainView
                {
                    DataContext = new MainViewModel()
                };
                break;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void RegisterViewModels(IServiceCollection services)
    {
        services.AddSingletonViewModel<MainView, MainViewModel>();
        services.AddTransientViewModel<StylePreviewPage, StylePreviewPageViewModel>();
        services.AddTransientViewModel<SettingsWindow, SettingsWindowViewModel>();
        // NavDemo
        services.AddTransientViewModel<HomePage_NavDemo, HomePageViewModel_NavDemo>();
        services.AddTransientViewModel<SettingsPage_NavDemo, SettingsPageViewModel_NavDemo>();
        services.AddTransientViewModel<DetailPage_NavDemo, DetailPageViewModel_NavDemo>();
    }

    private void RegisterServices(IServiceCollection services)
    {
        // 根据 Type 获取对应的 PageViewModel，会在 PageFactory 与 PageNavigationFactory 中用到
        // 使用 PageFactory0 或 PageFactory1 时的依赖注入写法
        // services.AddSingleton<Func<ApplicationPageName, PageViewModel>>(x => name => name switch
        // {
        //     // Menu 相关
        //     ApplicationPageName.Home => x.GetRequiredService<HomePageViewModel>(),
        //     ApplicationPageName.Settings => x.GetRequiredService<SettingsPageViewModel>(),
        //     _ => throw new InvalidOperationException(),
        // });
        // 使用 PageFactory 时的依赖注入写法
        services.AddSingleton<Func<Type, PageViewModel>>(x => type => type switch
        {
            // Menu 相关
            _ when type == typeof(StylePreviewPageViewModel) => x.GetRequiredService<StylePreviewPageViewModel>(),
            _ when type == typeof(SettingsWindowViewModel) => x.GetRequiredService<SettingsWindowViewModel>(),
            // NavDemo
            _ when type == typeof(HomePageViewModel_NavDemo) => x.GetRequiredService<HomePageViewModel_NavDemo>(),
            _ when type == typeof(DetailPageViewModel_NavDemo) => x.GetRequiredService<DetailPageViewModel_NavDemo>(),
            _ when type == typeof(SettingsPageViewModel_NavDemo) => x.GetRequiredService<SettingsPageViewModel_NavDemo>(),
            _ => throw new InvalidOperationException(),
        });
        services.AddSingleton<PageFactory>();
        // NavDemo
        services.AddSingleton<PageNavigationFactory>();
        /*
        说明：这里不使用依赖注入 NavigationService 的方式，而是直接使用静态属性：public static INavigationService? NavigationService { get; private set; }
        原因：若使用依赖注入 NavigationService，则每次在使用时都要在构造函数中添加一些代码，反而比较麻烦
        使用依赖注入时要添加的代码示例：
            public HomePageViewModel(INavigationService navigationService) : base(ApplicationPageName.Home)
            {
                _navigationService = navigationService;
            }
            private readonly INavigationService _navigationService;
        */
        // services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<AppSettings>();
    }

    // private void DisableAvaloniaDataAnnotationValidation()
    // {
    //     // Get an array of plugins to remove
    //     var dataValidationPluginsToRemove =
    //         BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();
    //
    //     // remove each entry found
    //     foreach (var plugin in dataValidationPluginsToRemove)
    //     {
    //         BindingPlugins.DataValidators.Remove(plugin);
    //     }
    // }
}