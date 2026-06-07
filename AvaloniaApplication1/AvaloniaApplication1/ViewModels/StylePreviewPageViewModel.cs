using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using AvaloniaApplication1.Controls;
using AvaloniaApplication1.Data;
using AvaloniaApplication1.Tools.ListBoxLog;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ursa.Controls;

namespace AvaloniaApplication1.ViewModels;

public partial class StylePreviewPageViewModel : PageViewModel
{
    public StylePreviewPageViewModel() : base(ApplicationPageName.StylePreview)
    {
        #region Demo-SearchBox
        FilterOptionsList =
        [
            new FilterOption { Id = "N1", Title = nameof(LogMessage.DateTimeStr), IsChecked = false, Count = 59 },
            new FilterOption { Id = "N3", Title = nameof(LogMessage.Title), IsChecked = false, Count = 12 },
            new FilterOption { Id = "N4", Title = nameof(LogMessage.SubTitle), IsChecked = false, Count = 36 },
            new FilterOption { Id = "N5", Title = nameof(LogMessage.OtherInfo), IsChecked = false, Count = 1 },
            new FilterOption { Id = "N6", Title = nameof(LogMessage.Message), IsChecked = false, Count = 22 },
        ];
        #endregion Demo-SearchBox
    }

    [ObservableProperty] 
    private string _greeting = "Welcome to Avalonia!";

    #region MenuItem
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
    #endregion MenuItem



    #region Demo-SearchBox
    [ObservableProperty] private ObservableCollection<FilterOption> _filterOptionsList;

    [RelayCommand]
    private void FilterConfirm(object? obj)
    {
        // TODO: Refresh data source
        if (obj == null)
            return;
        
        // 示例：从该 Command 的 CommandParameter 获取已选中的筛选项
        // List<FilterOption> checkedFilterOptions = (obj as List<FilterOption>)!;
        
    }
    
    [RelayCommand]
    private void Search(string keyword)
    {
        ListBoxLogger.SearchLogs(keyword, FilterOptionsList);
        
        NotificationType notificationType = keyword switch
        {
            "Large" => NotificationType.Warning,
            "Default" => NotificationType.Success,
            "Small" => NotificationType.Information,
            _ => NotificationType.Error
        };
        
        App.WindowToastManager?.Show(
            new Toast($"[Command Search] 参数：{keyword}"),
            type: notificationType,
            showIcon: true,
            showClose: true,
            onClose: OnToastClose,
            classes: ["Light"]);
    }

    [RelayCommand]
    private void ClearSearch()
    {
        // Some logic
        
        App.WindowToastManager?.Show(
            new Toast($"[Command ClearSearch] SearchBar content cleared"),
            type: NotificationType.Information,
            showIcon: true,
            showClose: true,
            onClose: OnToastClose,
            classes: ["Light"]);
    }

    #endregion Demo-SearchBox
    
    
    
    #region Demo-ListBoxLogger
    // 1. 创建 Random 实例（不要频繁 new，建议全局/静态复用）
    private static readonly Random _random = new Random();

    public ListBoxLogger ListBoxLogger { get; set { SetProperty(ref field, value); IsLogListBoxRegistered = true; } }
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(RegisterButtonContent))] private bool _isLogListBoxRegistered;
    public string RegisterButtonContent => !IsLogListBoxRegistered ? "Register" : "Registered";
    [ObservableProperty] private string _logTitle = "Title";
    [ObservableProperty] private string _logSubTitle = "SubTitle";
    [ObservableProperty] private string _otherInfo = "OtherInfo";
    [ObservableProperty] private string _message = "Message";
    [ObservableProperty] private bool _boldTitleFont;
    [ObservableProperty] private bool _boldSubTitleFont;
    [ObservableProperty] private bool _boldOtherInfoFont;
    [ObservableProperty] private bool _boldMessageFont;
    [ObservableProperty] private bool _showLogType = true;
    [ObservableProperty] private bool _showDate = true;
    [ObservableProperty] private bool _showTime = true;
    [ObservableProperty] private bool _showMilliseconds = true;
    [ObservableProperty] private int _millisecondsLength = 4;
    
    [ObservableProperty] private int _maxRandomNumber = 10;

    [RelayCommand]
    private void ClearLog()
    {
        // ListBoxLoggerManager.ClearAllLogs(MessageToken.ListBoxLogger_StylePreviewPage);
        ListBoxLogger.ClearAllLogs();
    }
    [RelayCommand]
    private void NewTipLog()
    {
        // 2. 生成 1~10 随机整数
        // Random.Next(minValue, maxValue) 规则：左闭右开，即包含最小值、不包含最大值。
        // 要生成 1 ~ 10（含两端），需要写成 Next(1, 11)。
        var random = _random.Next(1, MaxRandomNumber + 1);
        int i = 0;
        while (i < random)
        {
            ListBoxLoggerManager.TipLog(MessageToken.ListBoxLogger_StylePreviewPage, 
                LogTitle, LogSubTitle, OtherInfo, Message, 
                BoldTitleFont, BoldSubTitleFont, BoldOtherInfoFont, BoldMessageFont,
                ShowLogType, ShowDate, ShowTime, ShowMilliseconds, MillisecondsLength);
            i++;
        }
    }
    [RelayCommand]
    private void NewDefaultLog()
    {
        var random = _random.Next(1, MaxRandomNumber + 1);
        int i = 0;
        while (i < random)
        {
            ListBoxLogger.DefaultLog(LogTitle, LogSubTitle, OtherInfo, Message, 
                BoldTitleFont, BoldSubTitleFont, BoldOtherInfoFont, BoldMessageFont,
                ShowLogType, ShowDate, ShowTime, ShowMilliseconds, MillisecondsLength);
            i++;
        }
    }
    [RelayCommand]
    private void NewInfoLog()
    {
        var random = _random.Next(1, MaxRandomNumber + 1);
        int i = 0;
        while (i < random)
        {
            ListBoxLogger.InfoLog(LogTitle, LogSubTitle, OtherInfo, Message, 
                BoldTitleFont, BoldSubTitleFont, BoldOtherInfoFont, BoldMessageFont,
                ShowLogType, ShowDate, ShowTime, ShowMilliseconds, MillisecondsLength);
            i++;
        }
    }
    [RelayCommand]
    private void NewSuccessLog()
    {
        var random = _random.Next(1, MaxRandomNumber + 1);
        int i = 0;
        while (i < random)
        {
            ListBoxLogger.SuccessLog(LogTitle, LogSubTitle, OtherInfo, Message, 
                BoldTitleFont, BoldSubTitleFont, BoldOtherInfoFont, BoldMessageFont,
                ShowLogType, ShowDate, ShowTime, ShowMilliseconds, MillisecondsLength);
            i++;
        }
    }
    [RelayCommand]
    private void NewWarningLog()
    {
        var random = _random.Next(1, MaxRandomNumber + 1);
        int i = 0;
        while (i < random)
        {
            ListBoxLogger.WarningLog(LogTitle, LogSubTitle, OtherInfo, Message, 
                BoldTitleFont, BoldSubTitleFont, BoldOtherInfoFont, BoldMessageFont,
                ShowLogType, ShowDate, ShowTime, ShowMilliseconds, MillisecondsLength);
            i++;
        }
    }
    [RelayCommand]
    private void NewErrorLog()
    {
        var random = _random.Next(1, MaxRandomNumber + 1);
        int i = 0;
        while (i < random)
        {
            ListBoxLogger.ErrorLog(LogTitle, LogSubTitle, OtherInfo, Message, 
                BoldTitleFont, BoldSubTitleFont, BoldOtherInfoFont, BoldMessageFont,
                ShowLogType, ShowDate, ShowTime, ShowMilliseconds, MillisecondsLength);
            i++;
        }
    }
    [RelayCommand]
    private void NewFatalLog()
    {
        var random = _random.Next(1, MaxRandomNumber + 1);
        int i = 0;
        while (i < random)
        {
            ListBoxLogger.FatalLog(LogTitle, LogSubTitle, OtherInfo, Message, 
                BoldTitleFont, BoldSubTitleFont, BoldOtherInfoFont, BoldMessageFont,
                ShowLogType, ShowDate, ShowTime, ShowMilliseconds, MillisecondsLength);
            i++;
        }
    }
    [RelayCommand]
    private void GetLogs(LogType logType)
    {
        ListBoxLogger.FilterLogsByLogType(logType);
    }

    private void Test()
    {
        
    }

    private void OnToastClose(MessageCloseReason closeReason)
    {
        var reason = closeReason;
    }
    #endregion Demo-ListBoxLogger

    
    
}