using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Metadata;
using BatchProcess3.Data;
using BatchProcess3.Tools.Factories;
using BatchProcess3.ViewModels;
using BatchProcess3.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using BatchProcess3.Data;
using BatchProcess3.EntityFramework;
using BatchProcess3.Tools.Extensions;
using BatchProcess3.Tools.Services;
using BatchProcess3.ViewModels.Actions;
using BatchProcess3.ViewModels.MainMenus;
using BatchProcess3.Views.Actions;
using ClassicDiagnostics.Avalonia;
using ActionsPageView = BatchProcess3.Views.MainMenus.ActionsPageView;
using ActionsPageViewModel = BatchProcess3.ViewModels.MainMenus.ActionsPageViewModel;
using HistoryPageView = BatchProcess3.Views.MainMenus.HistoryPageView;
using HistoryPageViewModel = BatchProcess3.ViewModels.MainMenus.HistoryPageViewModel;
using HomePageView = BatchProcess3.Views.MainMenus.HomePageView;
using MacrosPageView = BatchProcess3.Views.MainMenus.MacrosPageView;
using MacrosPageViewModel = BatchProcess3.ViewModels.MainMenus.MacrosPageViewModel;
using ProcessPageView = BatchProcess3.Views.MainMenus.ProcessPageView;
using ReporterPageView = BatchProcess3.Views.MainMenus.ReporterPageView;
using ReporterPageViewModel = BatchProcess3.ViewModels.MainMenus.ReporterPageViewModel;
using SettingsPageView = BatchProcess3.Views.MainMenus.SettingsPageView;
using SettingsPageViewModel = BatchProcess3.ViewModels.MainMenus.SettingsPageViewModel;

// 自定义 XML Namespace 参考链接：https://docs.avaloniaui.net/docs/guides/custom-controls/how-to-create-a-custom-controls-library#xml-namespace-definitions
// 参考视频：https://www.youtube.com/watch?v=M3CFj0x-tts&list=PLrW43fNmjaQWwIdZxjZrx5FSXcNzaucOO&index=7
[assembly: XmlnsDefinition("https://github.com/avaloniaui", "BatchProcess3.Controls")]

namespace BatchProcess3;

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

    #region App Path
    /// <summary>
    /// 程序名称，有扩展名
    /// </summary>
    public static string AppName => Path.GetFileName(AppExePath); // 等于 Process.GetCurrentProcess().MainModule.ModuleName;❌ 等于 System.IO.Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().GetName().ConstraintName);❌
    /// <summary>
    /// 程序名称，无扩展名
    /// </summary>
    public static string AppNameWithoutExtension => Path.GetFileNameWithoutExtension(AppExePath);
    //private static string AppFrameworkName => AppDomain.CurrentDomain.SetupInformation.TargetFrameworkName;  // 高版本可用⚠️
    /// <summary>
    /// 当前程序所在目录，程序自启时有效，后面两个方式无法自启
    /// </summary>
    public static string AppExeDir => AppContext.BaseDirectory; // 等于 AppDomain.CurrentDomain.BaseDirectory;    // 等于 Directory.GetCurrentDirectory();❌  // 等于 Environment.CurrentDirectory;❌
    /// <summary>
    /// 当前程序完整路径
    /// </summary>
    public static string AppExePath => Process.GetCurrentProcess().MainModule.FileName; // 等于 Environment.ProcessPath，高版本可用
    /// <summary>
    /// 当前程序版本相关信息
    /// </summary>
    public static FileVersionInfo AppVersionInfo => Process.GetCurrentProcess().MainModule.FileVersionInfo;
    /// <summary>
    /// 当前程序 .Dll 完整路径
    /// </summary>
    public static string AppDllPath => Assembly.GetExecutingAssembly().Location;  // 等于 GetType().Assembly.Location;
    /// <summary>
    /// 当前程序的 .Dll 名称
    /// </summary>
    public static string AppDllName => AppVersionInfo.InternalName; // ❌ 或者 AppVersionInfo.OriginalFilename;❌

    /// <summary>
    /// 当前用户的系统自动启动目录路径
    /// </summary>
    public static string SystemStartDirPath => Environment.GetFolderPath(Environment.SpecialFolder.Startup);
    /// <summary>
    /// 桌面目录路径
    /// </summary>
    public static string DesktopPath => Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
    /// <summary>
    /// 注册表路径
    /// </summary>
    public static string RegistryPath => @"Software\Microsoft\Windows\CurrentVersion\Run";
    #endregion App Path
    
    private void Test()
    {
        
    }
    
    
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        Test();
#if DEBUG
        /* 第三方F12诊断工具：
        光头哥的 ProDiagnostics,
        群友移植老版本的 ClassicDiagnostics.Avalonia
         */
        // this.AttachDeveloperTools();    // AvaloniaUI.DiagnosticsSupport
        this.AttachDevTools();          // ClassicDiagnostics.Avalonia
#endif
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // UI线程未捕获异常处理事件，用于捕获 WPF UI 线程中的未处理异常。
        this.Dispatcher.UnhandledException += App_DispatcherUnhandledException;
        // 非UI线程未捕获异常处理事件，用于捕获所有非 UI 线程和非 Task 线程的未处理异常。
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        // Task线程内未捕获异常处理事件，用于捕获 Task 中未处理的异常。
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        
        // ==========  依赖注入 ==========
        var services = new ServiceCollection();
        RegisterViewModels(services);
        RegisterServices(services);
        var serviceProvider = services.BuildServiceProvider();
        // ==============================

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
                
                // TestWindow
                // desktop.MainWindow = new TestWindow() { DataContext = new TestViewModel() };
                
                // ErrorWindow
                // var errorWindow = new ErrorWindow();
                // errorWindow.DataContext = new ErrorViewModel();
                // errorWindow.Show();
                break;
            case ISingleViewApplicationLifetime singleViewPlatform:
                singleViewPlatform.MainView = new MainView
                {
                    DataContext = serviceProvider.GetRequiredService<MainViewModel>()
                };
                break;
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
        // Dialog
        services.AddTransientViewModel<ConfirmDialogView, ConfirmDialogViewModel>();
        services.AddTransientViewModel<PrintSettingsView, PrintSettingsViewModel>();
    }

    private void RegisterServices(IServiceCollection services)
    {
        #region Page Navigation Services
        services.AddSingleton<PageFactory>();
        // 使用 PageFactory0 或 PageFactory1 时的依赖注入写法
        // services.AddSingleton<Func<ApplicationPageName, PageViewModel>>(x => name => name switch
        // {
        //     // Menu 相关
        //     ApplicationPageName.Home => x.GetRequiredService<HomePageViewModel>(),
        //     ApplicationPageName.Process => x.GetRequiredService<ProcessPageViewModel>(),
        //     ApplicationPageName.Actions => x.GetRequiredService<ActionsPageViewModel>(),
        //     ApplicationPageName.Macros => x.GetRequiredService<MacrosPageViewModel>(),
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
            _ when type == typeof(ActionsPageViewModel) => x.GetRequiredService<ActionsPageViewModel>(),
            _ when type == typeof(MacrosPageViewModel) => x.GetRequiredService<MacrosPageViewModel>(),
            _ when type == typeof(ReporterPageViewModel) => x.GetRequiredService<ReporterPageViewModel>(),
            _ when type == typeof(HistoryPageViewModel) => x.GetRequiredService<HistoryPageViewModel>(),
            _ when type == typeof(SettingsPageViewModel) => x.GetRequiredService<SettingsPageViewModel>(),
            _ => throw new InvalidOperationException(),
        });
        #endregion Page Navigation Services
        
        services.AddSingleton<DialogService>();
        
        services.AddTransient<PrinterService>();
        
        #region Database services
        services.AddTransient<AppDbContext>();
        services.AddTransient<DatabaseService>();
        // 添加下面两个的原因：
        //      在 Transient 的 ViewModel 中（如 HomePageViewModel），依赖注入时使用 Transient 的 DatabaseService 没有问题，
        //      但在 Singleton 的 ViewModel 中（如 MainViewModel），依赖注入时使用 Transient 的 DatabaseService，这个 DatabaseService 就变成了 Singleton，因为 MainViewModel 是 Singleton
        services.AddSingleton<DatabaseFactory>();
        services.AddSingleton<Func<DatabaseService>>(x => x.GetRequiredService<DatabaseService>);
        #endregion Database services
        
        // TopLevel provider
        services.AddSingleton<Func<TopLevel?>>(x => () =>
        {
            return ApplicationLifetime switch
            {
                IClassicDesktopStyleApplicationLifetime desktopLifetime => TopLevel.GetTopLevel(desktopLifetime.MainWindow),
                ISingleViewApplicationLifetime singleView => TopLevel.GetTopLevel(singleView.MainView),
                _ => null
            };
        });

    }

    // UI线程未捕获异常处理事件
    private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        var logContent = LogException(e.Exception);
        e.Handled = true;   // 将 e.Handled 设为 true，标识异常已被处理，防止程序崩溃。
        ShowCrashMessageInErrorWindow(e.Exception, logContent);
        //NLogger.Logger.AddFatal(e.Exception, "UI线程未处理异常");
        //Logger.AddFatal(e.Exception, "UI线程未处理异常");
        // ShowErrorDialog("发生未处理的错误");
    }

    // 非UI线程未捕获异常处理事件
    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var logContent = LogException(e.ExceptionObject as Exception);
        ShowCrashMessageInErrorWindow(e.ExceptionObject as Exception, logContent);
        // EmergencySave();
    }

    // Task线程内未捕获异常处理事件
    private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        var logContent = LogException(e.Exception);
        ShowCrashMessageInErrorWindow(e.Exception, logContent);
        e.SetObserved();    // e.SetObserved() 表示异常已处理，避免程序崩溃。设置该异常已察觉（这样处理后就不会引起程序崩溃）
    }

    private static string LogException(Exception ex)
    {
        /*
            Environment.SpecialFolder 所有路径示例：
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));                // D:\Desktop
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.Programs));             // C:\Users\38287\AppData\Roaming\Microsoft\Windows\Start Menu\Programs
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));           // C:\Users\38287\Documents
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.Personal));             // C:\Users\38287\Documents
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.Favorites));            // C:\Users\38287\Favorites
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.Startup));             // C:\Users\38287\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.Recent));               // C:\Users\38287\AppData\Roaming\Microsoft\Windows\Recent
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.SendTo));               // C:\Users\38287\AppData\Roaming\Microsoft\Windows\SendTo
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu));           // C:\Users\38287\AppData\Roaming\Microsoft\Windows\Start Menu
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));             // C:\Users\38287\Music
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));            // C:\Users\38287\Videos
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));     // D:\Desktop
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.MyComputer));           // 
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.NetworkShortcuts));     // C:\Users\38287\AppData\Roaming\Microsoft\Windows\Network Shortcuts
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts));               // C:\Windows\Fonts
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.Templates));           // C:\Users\38287\AppData\Roaming\Microsoft\Windows\Templates
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu));      // C:\ProgramData\Microsoft\Windows\Start Menu
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.CommonPrograms));       // C:\ProgramData\Microsoft\Windows\Start Menu\Programs
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup));        // C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory));// C:\Users\Public\Desktop
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));      // C:\Users\38287\AppData\Roaming
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.PrinterShortcuts));     // 
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)); // C:\Users\38287\AppData\Local
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache));        // C:\Users\38287\AppData\Local\Microsoft\Windows\INetCache
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.Cookies));              // C:\Users\38287\AppData\Local\Microsoft\Windows\INetCookies
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.History));              // C:\Users\38287\AppData\Local\Microsoft\Windows\History
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));// C:\ProgramData
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.Windows));              // C:\Windows
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.System));              // C:\Windows\system32
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));        // C:\Program Files
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));           // C:\Users\38287\Pictures
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));          // C:\Users\38287
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86));            // C:\Windows\SysWOW64
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));      // C:\Program Files (x86)
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles));   // C:\Program Files\Common Files
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86));// C:\Program Files (x86)\Common Files
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.CommonTemplates));      // C:\ProgramData\Microsoft\Windows\Templates
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments));      // C:\Users\Public\Documents
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.CommonAdminTools));     // C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Administrative Tools
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.AdminTools));           // C:\Users\38287\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Administrative Tools
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic));          // C:\Users\Public\Music
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures));       // C:\Users\Public\Pictures
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.CommonVideos));         // C:\Users\Public\Videos
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.Resources));             // C:\Windows\resources
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.LocalizedResources));    // 
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.CommonOemLinks));        // 
                // Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.CDBurning));            // C:\Users\38287\AppData\Local\Microsoft\Windows\Burn\Burn
         */
        var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        var logDirectory = Path.Combine(homeDirectory, Path.Combine(ResourceToken.AppName, "AppCrashLogs"));
        Directory.CreateDirectory(logDirectory);

        var now = DateTime.Now;
        var logFileName = $"CrashLog_{now:yyyyMMdd_HHmmssffff}.log";
        var logFilePath = Path.Combine(logDirectory, logFileName);

        var logContent = $"CrashTime: {now:yyyy-MM-dd HH:mm:ss:ffff}{Environment.NewLine}" +
                         $"Exception Type: {ex.GetType().Name}{Environment.NewLine}" +
                         $"Exception Message: {ex.Message}{Environment.NewLine}" +
                         $"Stack Info: {Environment.NewLine}{ex.StackTrace}";
        
        File.WriteAllText(logFilePath, logContent);
        return logContent;
    }

    private static void ShowCrashMessageInErrorWindow(Exception ex, string logContent)
    {
        if (Application.Current != null)
        {
            Avalonia.Threading.Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                var vm = new ErrorViewModel
                {
                    // Message = $"{ex.GetType().Name}: {ex.Message}",
                    // StackTrace = ex.StackTrace ?? "No StackTrace."
                    Message = $"{ex.Message}",
                    StackTrace = logContent
                };

                var errorWin = new ErrorWindow { DataContext = vm };

                // 显示窗口，阻塞等待用户关闭
                // errorWin.ShowDialogAsync(null).Wait();
                errorWin.Show();
            });
        }
        else
        {
            // 启动阶段崩溃，Avalonia未初始化，使用Win32原生MessageBox兜底
            MessageBoxW(
                0,
                $"程序启动失败：\n\n{ex.Message}\n\n详细日志已保存至CrashLogs目录",
                "致命错误",
                0x10); // MB_ICONERROR
        }
    }
    
    // Win32 MessageBox 导入
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int MessageBoxW(nint hWnd, string text, string caption, uint iconType);

}