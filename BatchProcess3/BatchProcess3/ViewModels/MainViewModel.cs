using Avalonia.Svg.Skia;
using BatchProcess3.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatchProcess3.EntityFramework;
using BatchProcess3.Tools;
using BatchProcess3.Tools.Dialog;
using BatchProcess3.ViewModels.MainMenus;

namespace BatchProcess3.ViewModels
{
    public partial class MainViewModel : ViewModelBase, IDialogProvider
    {
        /// <summary>
        /// Design-time only constructor
        /// </summary>
        public MainViewModel()
        {
            CurrentPage = new SettingsPageViewModel(new DialogService(() => null), new DatabaseFactory(() => new DatabaseService(new AppDbContext())));
        }

        // 获取依赖
        public MainViewModel(PageFactory pageFactory, DatabaseFactory  databaseFactory)
        {
            // _pageFactory0 = pageFactory0;
            // _pageFactory1 = pageFactory1;
            _pageFactory = pageFactory ?? throw new ArgumentNullException(nameof(pageFactory));
            _databaseFactory = databaseFactory ?? throw new ArgumentNullException(nameof(databaseFactory));

            using var dbContext = _databaseFactory.GetDatabaseService();
            dbContext.ApplyMigrations();
            
            // GoToPage1("HomePage");
            // 注意：这里不能使用 Home 与 Process 了，会造成无限循环，导致程序启动了但不会显示窗口
            //      因为 HomePageViewModel 与 ProcessPageViewModel 的构造函数都实例化了 MainViewModel，导致出现 循环依赖（Circular Dependency） 问题
            //      
            //      无限循环的根因：
            //          循环依赖（Circular Dependency）
            //      根本原因是：
            //          MainViewModel 的构造函数里触发了导航，而导航要创建的 HomePageViewModel 又反过来依赖 MainViewModel，形成一个无法收敛的解析环。
            //      解析链：
            //          GetRequiredService<MainViewModel>()
            //           └─ MainViewModel 构造函数
            //               └─ GoToPage(Home)
            //                   └─ PageFactory.GetPageViewModel<HomePageViewModel>()
            //                       └─ GetRequiredService<HomePageViewModel>()
            //                           └─ HomePageViewModel(MainViewModel, ...)  ← 需要 MainViewModel！
            //                               └─ GetRequiredService<MainViewModel>()  ← 又回来了
            //                                   └─ MainViewModel 构造函数
            //                                       └─ GoToPage(Home)
            //                                           └─ ...  ♻️ 🔁 无限递归循环！
            //      关键点：
            //          MainViewModel 还没构造完成，就已经要求容器再给它一个 MainViewModel。这是一个典型的"构造期间向外解析服务"的反模式。
            GoToPage(ApplicationPageName.Settings);
        }

        private readonly PageFactory0 _pageFactory0;
        private readonly PageFactory1 _pageFactory1;
        private readonly PageFactory _pageFactory;
        private readonly DatabaseFactory _databaseFactory;

        public SvgImage SideMenuImage => new SvgImage { Source = SvgSource.Load($"avares://{nameof(BatchProcess3)}/Assets/Images/{(SideMenuExpanded ? "logo" : "icon")}.svg") };
        public int SomeWidth => SideMenuExpanded ? 220 : 65;
        public bool HomePageIsActive => CurrentPage.PageName == ApplicationPageName.Home;
        public bool ProcessPageIsActive => CurrentPage.PageName == ApplicationPageName.Process;
        public bool ActionsPageIsActive => CurrentPage.PageName == ApplicationPageName.Actions;
        public bool MacrosPageIsActive => CurrentPage.PageName == ApplicationPageName.Macros;
        public bool ReporterPageIsActive => CurrentPage.PageName == ApplicationPageName.Reporter;
        public bool HistoryPageIsActive => CurrentPage.PageName == ApplicationPageName.History;
        public bool SettingsPageIsActive => CurrentPage.PageName == ApplicationPageName.Settings;

        [ObservableProperty]
        private string _test = "Test Main";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SideMenuImage))]           // 修改时通知目标属性进行更新
        [NotifyPropertyChangedFor(nameof(SomeWidth))]               // 修改时通知目标属性进行更新
        private bool _sideMenuExpanded = true;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HomePageIsActive))]        // 修改时通知目标属性进行更新
        [NotifyPropertyChangedFor(nameof(ProcessPageIsActive))]     // 修改时通知目标属性进行更新
        [NotifyPropertyChangedFor(nameof(ActionsPageIsActive))]     // 修改时通知目标属性进行更新
        [NotifyPropertyChangedFor(nameof(MacrosPageIsActive))]     // 修改时通知目标属性进行更新
        [NotifyPropertyChangedFor(nameof(ReporterPageIsActive))]     // 修改时通知目标属性进行更新
        [NotifyPropertyChangedFor(nameof(HistoryPageIsActive))]     // 修改时通知目标属性进行更新
        [NotifyPropertyChangedFor(nameof(SettingsPageIsActive))]     // 修改时通知目标属性进行更新
        private PageViewModel _currentPage;

        [ObservableProperty]
        private DialogViewModel? _dialog;



        [RelayCommand]
        void SideMenuResize()
        {
            SideMenuExpanded = !SideMenuExpanded;
        }

        [RelayCommand]
        void GoToPage0(string pageName)
        {
            CurrentPage = pageName switch
            {
                "HomePage" => _pageFactory0.GetPageViewModel(ApplicationPageName.Home),
                "ProcessPage" => _pageFactory0.GetPageViewModel(ApplicationPageName.Process),
                "ActionsPage" => _pageFactory0.GetPageViewModel(ApplicationPageName.Actions),
                "MacrosPage" => _pageFactory0.GetPageViewModel(ApplicationPageName.Macros),
                "ReporterPage" => _pageFactory0.GetPageViewModel(ApplicationPageName.Reporter),
                "HistoryPage" => _pageFactory0.GetPageViewModel(ApplicationPageName.History),
                "SettingsPage" => _pageFactory0.GetPageViewModel(ApplicationPageName.Settings),
                _ => throw new ArgumentException($"Unsupported ApplicationPageName param: {pageName}")
            };
        }
        [RelayCommand]
        void GoToPage1(ApplicationPageName pageName)
        {
            CurrentPage = pageName switch
            {
                ApplicationPageName.Home => _pageFactory1.GetPageViewModel(ApplicationPageName.Home),
                ApplicationPageName.Process => _pageFactory1.GetPageViewModel(ApplicationPageName.Process),
                ApplicationPageName.Actions => _pageFactory1.GetPageViewModel(ApplicationPageName.Actions),
                ApplicationPageName.Macros => _pageFactory1.GetPageViewModel(ApplicationPageName.Macros),
                ApplicationPageName.Reporter => _pageFactory1.GetPageViewModel(ApplicationPageName.Reporter),
                ApplicationPageName.History => _pageFactory1.GetPageViewModel(ApplicationPageName.History),
                ApplicationPageName.Settings => _pageFactory1.GetPageViewModel(ApplicationPageName.Settings),
                _ => throw new ArgumentException($"Unsupported ApplicationPageName param: {pageName}")
            };
        }
        [RelayCommand]
        void GoToPage(ApplicationPageName pageName)
        {
            CurrentPage = pageName switch
            {
                // 可传入参数
                ApplicationPageName.Home => _pageFactory.GetPageViewModel<HomePageViewModel>(vm => vm.Test = "Test Home (Parameters can be passed in here)"),
                ApplicationPageName.Process => _pageFactory.GetPageViewModel<ProcessPageViewModel>(),
                ApplicationPageName.Actions => _pageFactory.GetPageViewModel<ActionsPageViewModel>(),
                ApplicationPageName.Macros => _pageFactory.GetPageViewModel<MacrosPageViewModel>(),
                ApplicationPageName.Reporter => _pageFactory.GetPageViewModel<ReporterPageViewModel>(),
                ApplicationPageName.History => _pageFactory.GetPageViewModel<HistoryPageViewModel>(),
                ApplicationPageName.Settings => _pageFactory.GetPageViewModel<SettingsPageViewModel>(),
                _ => throw new ArgumentException($"Unsupported ApplicationPageName param: {pageName}")
            };
        }




    }
}
