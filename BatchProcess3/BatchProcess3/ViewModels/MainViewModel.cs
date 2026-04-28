using Avalonia.Svg.Skia;
using BatchProcess3.Data;
using BatchProcess3.Tools.Factories;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatchProcess3.Data;

namespace BatchProcess3.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Disign-time only constructor
        /// </summary>
        public MainViewModel()
        {
            CurrentPage = new SettingsPageViewModel();
        }

        // 获取依赖
        public MainViewModel(PageFactory pageFactory)
        {
            // _pageFactory0 = pageFactory0;
            // _pageFactory1 = pageFactory1;
            _pageFactory = pageFactory;

            // GoToPage1("HomePage");
            GoToPage(ApplicationPageName.Home);
        }

        private PageFactory0 _pageFactory0;
        private PageFactory1 _pageFactory1;
        private PageFactory _pageFactory;

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
                // _ => _pageFactory.GetPageViewModel(ApplicationPageName.Unknown),
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
                // _ => _pageFactory.GetPageViewModel(ApplicationPageName.Unknown),
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
                // _ => _pageFactory.GetPageViewModel(ApplicationPageName.Unknown),
            };
        }




    }
}
