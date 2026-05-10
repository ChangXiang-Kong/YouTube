using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using AvaloniaApplication1.Controls.Models;
using AvaloniaApplication1.Data;
using AvaloniaApplication1.Tools.Factories;
using AvaloniaApplication1.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ursa.Common;
using Ursa.Controls;
using Ursa.Controls.Options;

namespace AvaloniaApplication1.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    /// <summary>
    /// Disign-time only constructor
    /// </summary>
    public MainViewModel()
    {
        CurrentPage = new StylePreviewPageViewModel();
    }

    // 获取依赖
    public MainViewModel(PageFactory pageFactory, AppSettings appSettings)
    {
        // _pageFactory0 = pageFactory0;
        // _pageFactory1 = pageFactory1;
        _pageFactory = pageFactory;
        AppSettings = appSettings;

        // GoToPage(ApplicationPageName.StylePreview);
    }

    private PageFactory0 _pageFactory0;
    private PageFactory1 _pageFactory1;
    private PageFactory _pageFactory;
    public AppSettings AppSettings { get; }

    public bool StylePreviewPageIsActive => CurrentPage.PageName == ApplicationPageName.StylePreview;

    [ObservableProperty] 
    private string _greeting = "Welcome to Avalonia!";

    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(StylePreviewPageIsActive))]
    private PageViewModel _currentPage;

    

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
        switch (pageName)
        {
            case ApplicationPageName.StylePreview:
                // 可传入参数
                CurrentPage = _pageFactory.GetPageViewModel<StylePreviewPageViewModel>(vm =>
                {
                    // Some logic
                    // vm.Greeting = "Test StylePreview (Parameters can be passed in here)";
                });
                break;
            case ApplicationPageName.Settings:
                // TODO: Test Only
                var window = new SettingsWindow();
                window.DataContext = _pageFactory.GetPageViewModel<SettingsWindowViewModel>(vm =>
                {

                });
                window.Show();
                
                // CurrentPage = _pageFactory.GetPageViewModel<SettingsWindowViewModel>(vm =>
                // {
                //     // Some logic
                //     // vm.Greeting = "Test StylePreview (Parameters can be passed in here)";
                // });
                break;
            default:
                // CurrentPage = _pageFactory.GetPageViewModel<>(ApplicationPageName.Unknown);
                break;
        }
    }

    [ObservableProperty]
    private MenuItem? _selectedMenuItem;
    
    public ObservableCollection<MenuItem> MenuItems { get; set; } = new ObservableCollection<MenuItem>
    {
        new MenuItem { Header = "Introduction" , Children =
        {
            new MenuItem() { Header = "Getting Started", Children =
            {
                new MenuItem() { Header = "Code of Conduct" },
                new MenuItem() { Header = "How to Contribute" },
                new MenuItem() { Header = "Development Workflow" },
            }},
            new MenuItem() { Header = "Design Principles"},
            new MenuItem() { Header = "Contributing", Children =
            {
                new MenuItem() { Header = "Code of Conduct" },
                new MenuItem() { Header = "How to Contribute" },
                new MenuItem() { Header = "Development Workflow" },
            }},
        }},
        new MenuItem { Header = "Controls", IsSeparator = true},
        new MenuItem { Header = "Badge" },
        new MenuItem { Header = "Banner" },
        new MenuItem { Header = "ButtonGroup" },
        new MenuItem { Header = "Class Input" },
        new MenuItem { Header = "Dialog" },
        new MenuItem { Header = "Divider" },
        new MenuItem { Header = "Drawer" },
        new MenuItem { Header = "DualBadge" },
        new MenuItem { Header = "EnumSelector" },
        new MenuItem { Header = "ImageViewer" },
        new MenuItem { Header = "IPv4Box" },
        new MenuItem { Header = "IconButton" },
        new MenuItem { Header = "KeyGestureInput" },
        new MenuItem { Header = "Loading" },
        new MenuItem { Header = "MessageBox" },
        new MenuItem { Header = "Navigation" },
        new MenuItem { Header = "NavMenu" },
        new MenuItem { Header = "NumericUpDown" },
        new MenuItem { Header = "Pagination" },
        new MenuItem { Header = "RangeSlider" },
        new MenuItem { Header = "SelectionList" },
        new MenuItem { Header = "TagInput" },
        new MenuItem { Header = "Timeline" },
        new MenuItem { Header = "TwoTonePathIcon" },
        new MenuItem { Header = "ThemeToggler" }
    };

    [RelayCommand]
    private void RandomSelectMenuItem()
    {
        var items = GetLeaves();
        var index = new Random().Next(items.Count);
        SelectedMenuItem = items[index];
    }
    
    private List<MenuItem> GetLeaves()
    {
        List<MenuItem> items = new();
        foreach (var item in MenuItems)
        {
            items.AddRange(item.GetLeaves());
        }

        return items;
    }
    
    
}

public class MenuItem
{
    static Random r = new Random();
    
    public string? Header { get; set; }
    public int IconIndex { get; set; }
    public bool IsSeparator { get; set; }
    public ICommand NavigationCommand { get; set; }

    public MenuItem()
    {
        NavigationCommand = new AsyncRelayCommand(OnNavigate);
        IconIndex = r.Next(100);
    }

    private async Task OnNavigate()
    {
        await OverlayMessageBox.ShowAsync(Header??string.Empty, "Navigation Result");
    }

    public ObservableCollection<MenuItem> Children { get; set; } = new ObservableCollection<MenuItem>();

    public IEnumerable<MenuItem> GetLeaves()
    {
        if (this.Children.Count == 0)
        {
            yield return this;
            yield break;
        }

        foreach (var child in Children)
        {
            var items = child.GetLeaves();
            foreach (var item in items)
            {
                yield return item;
            }
        }
    }
}