using Avalonia.Svg.Skia;
using AvaloniaUIRealWorld.Data;
using AvaloniaUIRealWorld.Factories;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AvaloniaUIRealWorld.Data.EnumValues;

namespace AvaloniaUIRealWorld.ViewModels
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

            GoToPage("HomePage");
        }

        private PageFactory _pageFactory;

        public SvgImage SideMenuImage => new SvgImage { Source = SvgSource.Load($"avares://{nameof(AvaloniaUIRealWorld)}/Assets/Images/{(SideMenuExpanded ? "logo" : "icon")}.svg") };
        public int SomeWidth => SideMenuExpanded ? 220 : 65;
        public bool HomePageIsActive => CurrentPage.PageName == ApplicationPageNames.Home;
        public bool ProcessPageIsActive => CurrentPage.PageName == ApplicationPageNames.Process;
        public bool ActionsPageIsActive => CurrentPage.PageName == ApplicationPageNames.Actions;
        public bool MacrosPageIsActive => CurrentPage.PageName == ApplicationPageNames.Macros;
        public bool ReporterPageIsActive => CurrentPage.PageName == ApplicationPageNames.Reporter;
        public bool HistoryPageIsActive => CurrentPage.PageName == ApplicationPageNames.History;
        public bool SettingsPageIsActive => CurrentPage.PageName == ApplicationPageNames.Settings;
        public bool ErrorPageIsActive => CurrentPage.PageName == ApplicationPageNames.Error;

        [ObservableProperty]
        private string _Test = "Test Main";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SideMenuImage))]           // 修改时通知目标属性进行更新
        [NotifyPropertyChangedFor(nameof(SomeWidth))]               // 修改时通知目标属性进行更新
        private bool _SideMenuExpanded = true;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HomePageIsActive))]        // 修改时通知目标属性进行更新
        [NotifyPropertyChangedFor(nameof(ProcessPageIsActive))]     // 修改时通知目标属性进行更新
        [NotifyPropertyChangedFor(nameof(ActionsPageIsActive))]     // 修改时通知目标属性进行更新
        [NotifyPropertyChangedFor(nameof(MacrosPageIsActive))]     // 修改时通知目标属性进行更新
        [NotifyPropertyChangedFor(nameof(ReporterPageIsActive))]     // 修改时通知目标属性进行更新
        [NotifyPropertyChangedFor(nameof(HistoryPageIsActive))]     // 修改时通知目标属性进行更新
        [NotifyPropertyChangedFor(nameof(SettingsPageIsActive))]     // 修改时通知目标属性进行更新
        [NotifyPropertyChangedFor(nameof(ErrorPageIsActive))]     // 修改时通知目标属性进行更新
        private PageViewModel _CurrentPage;





        [RelayCommand]
        void SideMenuResize()
        {
            SideMenuExpanded = !SideMenuExpanded;
        }

        [RelayCommand]
        void GoToPage(string pageName)
        {
            CurrentPage = pageName switch
            {
                "HomePage" => _pageFactory.GetPageViewModel(ApplicationPageNames.Home),
                "ProcessPage" => _pageFactory.GetPageViewModel(ApplicationPageNames.Process),
                "ActionsPage" => _pageFactory.GetPageViewModel(ApplicationPageNames.Actions),
                "MacrosPage" => _pageFactory.GetPageViewModel(ApplicationPageNames.Macros),
                "ReporterPage" => _pageFactory.GetPageViewModel(ApplicationPageNames.Reporter),
                "HistoryPage" => _pageFactory.GetPageViewModel(ApplicationPageNames.History),
                "SettingsPage" => _pageFactory.GetPageViewModel(ApplicationPageNames.Settings),
                "ErrorPage" => _pageFactory.GetPageViewModel(ApplicationPageNames.Error),
                _ => _pageFactory.GetPageViewModel(ApplicationPageNames.Error),
            };
        }




    }
}
