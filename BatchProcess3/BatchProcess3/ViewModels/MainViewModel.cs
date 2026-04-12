using Avalonia.Svg.Skia;
using BatchProcess3.Data;
using BatchProcess3.Factories;
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
            _pageFactory = pageFactory;

            // GoToPage1("HomePage");
            GoToPage(ApplicationPageName.Home);
        }

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
        void GoToPage1(string pageName)
        {
            CurrentPage = pageName switch
            {
                "HomePage" => _pageFactory.GetPageViewModel(ApplicationPageName.Home),
                "ProcessPage" => _pageFactory.GetPageViewModel(ApplicationPageName.Process),
                "ActionsPage" => _pageFactory.GetPageViewModel(ApplicationPageName.Actions),
                "MacrosPage" => _pageFactory.GetPageViewModel(ApplicationPageName.Macros),
                "ReporterPage" => _pageFactory.GetPageViewModel(ApplicationPageName.Reporter),
                "HistoryPage" => _pageFactory.GetPageViewModel(ApplicationPageName.History),
                "SettingsPage" => _pageFactory.GetPageViewModel(ApplicationPageName.Settings),
                // _ => _pageFactory.GetPageViewModel(ApplicationPageName.Unknown),
            };
        }
        [RelayCommand]
        void GoToPage(ApplicationPageName pageName)
        {
            CurrentPage = pageName switch
            {
                ApplicationPageName.Home => _pageFactory.GetPageViewModel(ApplicationPageName.Home),
                ApplicationPageName.Process => _pageFactory.GetPageViewModel(ApplicationPageName.Process),
                ApplicationPageName.Actions => _pageFactory.GetPageViewModel(ApplicationPageName.Actions),
                ApplicationPageName.Macros => _pageFactory.GetPageViewModel(ApplicationPageName.Macros),
                ApplicationPageName.Reporter => _pageFactory.GetPageViewModel(ApplicationPageName.Reporter),
                ApplicationPageName.History => _pageFactory.GetPageViewModel(ApplicationPageName.History),
                ApplicationPageName.Settings => _pageFactory.GetPageViewModel(ApplicationPageName.Settings),
                // _ => _pageFactory.GetPageViewModel(ApplicationPageName.Unknown),
            };
        }




    }
}
