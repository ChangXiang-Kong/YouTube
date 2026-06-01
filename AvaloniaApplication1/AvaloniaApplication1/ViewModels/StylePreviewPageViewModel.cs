using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using AvaloniaApplication1.Data;
using AvaloniaApplication1.Tools.ListBoxLogger;
using AvaloniaApplication1.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ursa.Controls;

namespace AvaloniaApplication1.ViewModels;

public partial class StylePreviewPageViewModel() : PageViewModel(ApplicationPageName.StylePreview)
{
    [ObservableProperty] 
    private ListBoxLog _listBoxLog;
    
    [ObservableProperty] 
    private string _greeting = "Welcome to Avalonia!";
    
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


    #region LogDemo

    public readonly string LoggerName = $"ListBoxLogger_{nameof(StylePreviewPage)}";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(RegisterButtonContent))] private bool _isLogListBoxRegistered;
    public string RegisterButtonContent => !IsLogListBoxRegistered ? "Register" : "Registered";
    [ObservableProperty] private string? _logTitle = "Title";
    [ObservableProperty] private string? _logSubTitle = "SubTitle";
    [ObservableProperty] private string? _otherInfo = "OtherInfo";
    [ObservableProperty] private string? _message = "Message";
    [ObservableProperty] private bool _boldTitleFont;
    [ObservableProperty] private bool _boldSubTitleFont;
    [ObservableProperty] private bool _boldOtherInfoFont;
    [ObservableProperty] private bool _boldMessageFont;
    [ObservableProperty] private bool _showLogType = true;
    [ObservableProperty] private bool _showDate = true;
    [ObservableProperty] private bool _showTime = true;
    [ObservableProperty] private bool _showMilliseconds = true;
    [ObservableProperty] private int _millisecondsLength = 4;

    [RelayCommand]
    private void RegisterLogListBox(object? obj)
    {
        if (obj == null)
            return;
        
        ListBox listBox = (obj as ListBox)!;
        if (!ListBoxLogManager.Logger.TryRegisterLogListBox(LoggerName, listBox)) 
            return;
        
        ListBoxLog = ListBoxLogManager.Logger.GetLoggerByName(LoggerName);
        IsLogListBoxRegistered = true;
        App.WindowToastManager?.Show(
            new Toast("注册成功"),
            type: NotificationType.Success,
            showIcon: true,
            showClose: true,
            onClose: OnToastClose,
            classes: ["Light"]);
    }
    [RelayCommand]
    private void ClearLog()
    {
        ListBoxLogManager.Logger.Clear(LoggerName);
    }
    [RelayCommand]
    private void NewTipLog()
    {
        ListBoxLogManager.Logger.TipLog(LoggerName, 
            LogTitle, LogSubTitle, OtherInfo, Message, 
            BoldTitleFont, BoldSubTitleFont, BoldOtherInfoFont, BoldMessageFont,
            ShowLogType, ShowDate, ShowTime, ShowMilliseconds, MillisecondsLength);
    }
    [RelayCommand]
    private void NewDefaultLog()
    {
        ListBoxLogManager.Logger.DefaultLog(LoggerName, 
            LogTitle, LogSubTitle, OtherInfo, Message, 
            BoldTitleFont, BoldSubTitleFont, BoldOtherInfoFont, BoldMessageFont,
            ShowLogType, ShowDate, ShowTime, ShowMilliseconds, MillisecondsLength);
    }
    [RelayCommand]
    private void NewInfoLog()
    {
        ListBoxLogManager.Logger.InfoLog(LoggerName, 
            LogTitle, LogSubTitle, OtherInfo, Message, 
            BoldTitleFont, BoldSubTitleFont, BoldOtherInfoFont, BoldMessageFont,
            ShowLogType, ShowDate, ShowTime, ShowMilliseconds, MillisecondsLength);
    }
    [RelayCommand]
    private void NewSuccessLog()
    {
        ListBoxLogManager.Logger.SuccessLog(LoggerName, 
            LogTitle, LogSubTitle, OtherInfo, Message, 
            BoldTitleFont, BoldSubTitleFont, BoldOtherInfoFont, BoldMessageFont,
            ShowLogType, ShowDate, ShowTime, ShowMilliseconds, MillisecondsLength);
    }
    [RelayCommand]
    private void NewWarningLog()
    {
        ListBoxLogManager.Logger.WarningLog(LoggerName, 
            LogTitle, LogSubTitle, OtherInfo, Message, 
            BoldTitleFont, BoldSubTitleFont, BoldOtherInfoFont, BoldMessageFont,
            ShowLogType, ShowDate, ShowTime, ShowMilliseconds, MillisecondsLength);
    }
    [RelayCommand]
    private void NewErrorLog()
    {
        ListBoxLogManager.Logger.ErrorLog(LoggerName, 
            LogTitle, LogSubTitle, OtherInfo, Message, 
            BoldTitleFont, BoldSubTitleFont, BoldOtherInfoFont, BoldMessageFont,
            ShowLogType, ShowDate, ShowTime, ShowMilliseconds, MillisecondsLength);
    }
    [RelayCommand]
    private void NewFatalLog()
    {
        ListBoxLogManager.Logger.FatalLog(LoggerName, 
            LogTitle, LogSubTitle, OtherInfo, Message, 
            BoldTitleFont, BoldSubTitleFont, BoldOtherInfoFont, BoldMessageFont,
            ShowLogType, ShowDate, ShowTime, ShowMilliseconds, MillisecondsLength);
    }

    #endregion LogDemo

    private void OnToastClose(MessageCloseReason closeReason)
    {
        var reason = closeReason;
    }

    
    
}