using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using AvaloniaApplication1.Controls;
using AvaloniaApplication1.Tools.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaApplication1.Tools.ListBoxLog;

public partial class ListBoxLogger : ObservableObject
{
    internal ListBoxLogger(string loggerName, ListBox primaryListBox, ListBox secondListBox)
    {
        PrimaryLoggerName = loggerName;
        SecondLoggerName = $"{loggerName}_bak";
        _primaryListBoxLogger = primaryListBox ?? throw new ArgumentNullException(nameof(primaryListBox), "Param cannot be null");
        _secondListBoxLogger = secondListBox ?? throw new ArgumentNullException(nameof(secondListBox), "Param cannot be null");
        
        _primaryListBoxLogger.DoubleTapped -= PrimaryListBoxLoggerDoubleTapped;     // 解绑旧事件，避免多次绑定
        _primaryListBoxLogger.DoubleTapped += PrimaryListBoxLoggerDoubleTapped;     // 绑定双击事件
        _secondListBoxLogger.DoubleTapped -= SecondListBoxLoggerDoubleTapped;     // 解绑旧事件，避免多次绑定
        _secondListBoxLogger.DoubleTapped += SecondListBoxLoggerDoubleTapped;     // 绑定双击事件
        
        // 枚举全部初始化显示标记
        foreach (var type in Enum.GetValues<LogType>())
            _logTypeShowState[type] = false;
    }
    
    private readonly DynamicResourceExtension _dynamicResourceTextBlockTertiaryForeground = new("TextBlockTertiaryForeground");
    private readonly DynamicResourceExtension _dynamicResourceTextBlockDefaultForeground = new("TextBlockDefaultForeground");
    private readonly DynamicResourceExtension _dynamicResourceTextBlockInfoForeground = new("TextBlockInfoForeground");
    private readonly DynamicResourceExtension _dynamicResourceTextBlockSuccessForeground = new("TextBlockSuccessForeground");
    private readonly DynamicResourceExtension _dynamicResourceTextBlockWarningForeground = new("TextBlockWarningForeground");
    private readonly DynamicResourceExtension _dynamicResourceTextBlockDangerForeground = new("TextBlockDangerForeground");
    private readonly DynamicResourceExtension _dynamicResourceTextBlockFatalForeground = new("SemiAIPurple5");
    /// <summary>
    /// 主ListBox（显示全部日志）
    /// </summary>
    private ListBox? _primaryListBoxLogger;
    /// <summary>
    /// 副ListBox（显示过滤后的日志）
    /// </summary>
    private ListBox? _secondListBoxLogger;
    /// <summary>
    /// 缓存日志，便于直接对所有日志进行筛选，而不是遍历每个 StackPanel 中所有的 TextBlock 以组合成一条日志
    /// </summary>
    private List<LogMessage> _logMessagesListCache = [];
    // 用字典存储各类型显示状态，去掉一堆_isShowingLogs_XXX零散字段
    private readonly Dictionary<LogType, bool> _logTypeShowState = new();
    private bool _isConfirmFiltering;

    /// <summary>
    /// 是否过滤日志，false 显示主ListBox（显示全部日志），true 显示副ListBox（显示过滤后的日志）
    /// </summary>
    [ObservableProperty] private string _primaryLoggerName;
    [ObservableProperty] private string _secondLoggerName;
    /// <summary>
    /// false 按时间正排序；true 按时间倒排序
    /// </summary>
    [ObservableProperty] private bool _desc;
    [ObservableProperty] private int _maxLogCount = 200;
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(TotalLogCount))] private int _logCount_Tip;
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(TotalLogCount))] private int _logCount_Default;
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(TotalLogCount))] private int _logCount_Info;
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(TotalLogCount))] private int _logCount_Success;
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(TotalLogCount))] private int _logCount_Warning;
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(TotalLogCount))] private int _logCount_Error;
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(TotalLogCount))] private int _logCount_Fatal;
    public int TotalLogCount => LogCount_Tip + LogCount_Default + LogCount_Info + LogCount_Success +
                                LogCount_Warning + LogCount_Error + LogCount_Fatal;

    /// <summary>
    /// 取消绑定日志ListBox
    /// </summary>
    public void UnregisterListBoxLogger()
    {
        ClearAllLogs();
        _logMessagesListCache = null;
        _primaryListBoxLogger?.DoubleTapped -= PrimaryListBoxLoggerDoubleTapped;
        _primaryListBoxLogger = null;
        _secondListBoxLogger?.DoubleTapped -= SecondListBoxLoggerDoubleTapped;
        _secondListBoxLogger = null;
    }

    #region 双击事件复制文本
    private void PrimaryListBoxLoggerDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (_primaryListBoxLogger?.SelectedItem is not StackPanel logPanel)
            return;

        CopyLogTextToClipboard(logPanel);
    }
    private void SecondListBoxLoggerDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (_secondListBoxLogger?.SelectedItem is not StackPanel logPanel)
            return;

        CopyLogTextToClipboard(logPanel);
    }

    private void CopyLogTextToClipboard(StackPanel logPanel)
    {
        // 递归提取当前日志项内所有文本
        string allText = GetAllTextFromVisual(logPanel);
        if (string.IsNullOrWhiteSpace(allText))
            return;
        
        // 获取剪贴板
        var clipboard = ResourceHelper.ResolveDefaultTopLevel()?.Clipboard;
        // 写入剪贴板
        clipboard?.SetTextAsync(allText);
        // 通知
        WeakReferenceMessenger.Default.Send($"{PrimaryLoggerName}|已复制到剪贴板", PrimaryLoggerName);
    }

    /// <summary>
    /// 递归遍历控件，提取所有 TextBlock 文本
    /// </summary>
    private string GetAllTextFromVisual(Control control)
    {
        StringBuilder sb = new StringBuilder();

        if (control is TextBlock tb)
        {
            sb.Append(tb.Text);
        }

        // 遍历子控件
        if (control is Panel panel)
        {
            foreach (var child in panel.Children)
            {
                if (child is Control childCtrl)
                {
                    sb.Append(GetAllTextFromVisual(childCtrl));
                }
            }
        }

        return sb.ToString();
    }
    #endregion 双击事件复制文本

    /// <summary>
    /// 添加日志
    /// </summary>
    /// <param name="logMessage">日志对象</param>
    private void Log(LogMessage logMessage)
    {
        if (_primaryListBoxLogger == null)
            throw new ArgumentNullException(nameof(_primaryListBoxLogger), "LogListBox not registered");

        // Create a new StackPanel for the log entry
        StackPanel logEntry = new StackPanel { Tag = logMessage.LogType.ToString() };
        // Create a horizontal StackPanel for the timestamp and titles
        StackPanel headerPanel = new StackPanel { Orientation = Orientation.Horizontal };

        string tempText = "";
        
        // 统一获取当前时间，避免前后时间不一致
        var now = DateTime.Now;
        // Add timestamp if required
        if (logMessage.ShowDate || logMessage.ShowTime)
        {
            string timestamp = "";
            if (logMessage.ShowDate) 
                timestamp += now.ToString("yyyy-MM-dd");
            if (logMessage.ShowTime) 
                timestamp += (timestamp.Length > 0 ? " " : "") + now.ToString("HH:mm:ss");
            if (logMessage.ShowTime && logMessage.ShowMilliseconds)     // 等于 (logMessage is { ShowTime: true, ShowMilliseconds: true })
                timestamp += now.ToString($".{new string('f', logMessage.MillisecondsLength)}");
            
            logMessage.DateTimeStr = timestamp;
            TextBlock timestampText = new TextBlock { Text = $"[{timestamp}] " };
            BindForeground(logMessage.LogType, timestampText);
            headerPanel.Children.Add(timestampText);
        }
        
        // Add logTypeText
        // if (logMessage.ShowLogType)
        // {
            TextBlock typeText = new TextBlock
            {
                Text = $"[{logMessage.LogType.ToString().Substring(0, 3).ToUpperInvariant()}] ", 
                IsVisible = logMessage.ShowLogType, // 用 IsVisible 代替 if 判断，这样在双击选中项复制时就会有 LogType 文本
            };
            BindForeground(logMessage.LogType, typeText);
            headerPanel.Children.Add(typeText);
        // }
        
        // Add title
        if (!string.IsNullOrWhiteSpace(logMessage.Title))
        {
            tempText = $"[{logMessage.Title}] ";
            TextBlock titleText = !logMessage.BoldTitleFont
                ? new TextBlock { Text = tempText, TextWrapping = TextWrapping.Wrap }
                : new TextBlock { Text = tempText, TextWrapping = TextWrapping.Wrap, FontWeight = FontWeight.Bold };
            BindForeground(logMessage.LogType, titleText);
            headerPanel.Children.Add(titleText);
        }
        
        // Add subtitle
        if (!string.IsNullOrWhiteSpace(logMessage.SubTitle))
        {
            tempText = $"[{logMessage.SubTitle}] ";
            TextBlock subTitleText = !logMessage.BoldSubTitleFont
                ? new TextBlock { Text = tempText, TextWrapping = TextWrapping.Wrap }
                : new TextBlock { Text = tempText, TextWrapping = TextWrapping.Wrap, FontWeight = FontWeight.Bold };
            BindForeground(logMessage.LogType, subTitleText);
            headerPanel.Children.Add(subTitleText);
        }
        
        // Add otherInfo
        if (!string.IsNullOrWhiteSpace(logMessage.OtherInfo))
        {
            tempText = $"[{logMessage.OtherInfo}] ";
            TextBlock otherInfoText = !logMessage.BoldOtherInfoFont
                ? new TextBlock { Text = tempText, TextWrapping = TextWrapping.Wrap }
                : new TextBlock { Text = tempText, TextWrapping = TextWrapping.Wrap, FontWeight = FontWeight.Bold };
            BindForeground(logMessage.LogType, otherInfoText);
            headerPanel.Children.Add(otherInfoText);
        }

        // Add the header panel to the log entry
        logEntry.Children.Add(headerPanel);

        // Add the message
        tempText = !string.IsNullOrWhiteSpace(logMessage.Message) ? logMessage.Message : "Empty Message";
        TextBlock messageText = !logMessage.BoldMessageFont 
            ? new TextBlock { Text = tempText, TextWrapping = TextWrapping.WrapWithOverflow }
            : new TextBlock { Text = tempText, TextWrapping = TextWrapping.WrapWithOverflow, FontWeight = FontWeight.Bold };
        BindForeground(logMessage.LogType, messageText);
        tempText = string.Empty;
        
        // Add the message TextBlock to the log entry
        logEntry.Children.Add(messageText);

        // Add the log entry to the ListBox
        if (!Desc)
        {
            // 升序
            // Ensure we only keep MaxLogCount entries
            while (_primaryListBoxLogger.Items.Count >= MaxLogCount)
            {
                // 超出最大条数移除第一项
                StackPanel toRemove = (_primaryListBoxLogger.Items[0] as StackPanel)!;
                // 删除日志
                _primaryListBoxLogger.Items.Remove(toRemove);
                CalculateLogCount(GetLogTypeToBeRemoved(toRemove), false);
                toRemove.Tag = null;
                toRemove.Children.Clear();
                // 删除缓存的日志
                _logMessagesListCache?.RemoveAt(0);
            }
            
            // 添加日志
            _primaryListBoxLogger.Items.Add(logEntry);
            // 添加缓存日志
            _logMessagesListCache?.Add(logMessage);
            CalculateLogCount(logMessage.LogType, true);
        }
        else
        {
            // 降序
            // Ensure we only keep MaxLogCount entries
            while (_primaryListBoxLogger.Items.Count >= MaxLogCount)
            {
                // 超出最大条数移除最后一项
                StackPanel toRemove = (_primaryListBoxLogger.Items[^1] as StackPanel)!;   // 等于 _logListBox.Items[_logListBox.Items.Count - 1];
                // 删除日志
                _primaryListBoxLogger.Items.Remove(toRemove);
                CalculateLogCount(GetLogTypeToBeRemoved(toRemove), false);
                toRemove.Tag = null;
                toRemove.Children.Clear();
                // 删除缓存的日志
                _logMessagesListCache?.Remove(_logMessagesListCache[^1]);
            }
            
            // 添加日志
            _primaryListBoxLogger.Items.Insert(0, logEntry);
            // 添加缓存日志
            _logMessagesListCache?.Insert(0, logMessage);
            CalculateLogCount(logMessage.LogType, true);
        }
        
        // 滚动到当前日志项并选中
        // 使用 Dispatcher 防止报错：System.InvalidOperationException: Invalid Arrange rectangle.
        _primaryListBoxLogger.Dispatcher.InvokeAsync(() =>
        {
            _primaryListBoxLogger.ScrollIntoView(logEntry);
            _primaryListBoxLogger.SelectedItem = logEntry;
        });
    }
    
    /// <summary>
    /// 添加 Tip 日志
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="subTitle">副标题</param>
    /// <param name="otherInfo">其他信息</param>
    /// <param name="message">消息</param>
    /// <param name="boldTitleFont">是否加粗标题</param>
    /// <param name="boldSubTitleFont">是否加粗副标题</param>
    /// <param name="boldOtherInfoFont">是否加粗其他信息</param>
    /// <param name="boldMessageFont">是否加粗消息</param>
    /// <param name="showLogType">是否显示日志类型</param>
    /// <param name="showDate">是否显示日期</param>
    /// <param name="showTime">是否显示时间</param>
    /// <param name="showMilliseconds">是否显示毫秒</param>
    /// <param name="millisecondsLength">显示毫秒长度</param>
    public void TipLog(string title, string subTitle, string otherInfo, string message,
        bool boldTitleFont = false, bool boldSubTitleFont = false, bool boldOtherInfoFont = false, bool boldMessageFont = false, 
        bool showLogType = true, bool showDate = true, bool showTime = true, bool showMilliseconds = true, int millisecondsLength = 4)
    {
        LogMessage logMessage = new()
        {
            LogType = LogType.Tip,
            Title = title,
            SubTitle = subTitle,
            OtherInfo = otherInfo,
            Message = message,
            BoldTitleFont = boldTitleFont,
            BoldSubTitleFont = boldSubTitleFont,
            BoldOtherInfoFont = boldOtherInfoFont,
            BoldMessageFont = boldMessageFont,
            ShowLogType = showLogType,
            ShowDate = showDate,
            ShowTime = showTime,
            ShowMilliseconds = showMilliseconds,
            MillisecondsLength = millisecondsLength,
        };
        Log(logMessage);
        
        // 在过滤时新增的日志处理逻辑
        if (_logTypeShowState[LogType.Tip])
            GetLogsByLogType(LogType.Tip, logMessage);
    }

    /// <summary>
    /// 添加 Default 日志
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="subTitle">副标题</param>
    /// <param name="otherInfo">其他信息</param>
    /// <param name="message">消息</param>
    /// <param name="boldTitleFont">是否加粗标题</param>
    /// <param name="boldSubTitleFont">是否加粗副标题</param>
    /// <param name="boldOtherInfoFont">是否加粗其他信息</param>
    /// <param name="boldMessageFont">是否加粗消息</param>
    /// <param name="showLogType">是否显示日志类型</param>
    /// <param name="showDate">是否显示日期</param>
    /// <param name="showTime">是否显示时间</param>
    /// <param name="showMilliseconds">是否显示毫秒</param>
    /// <param name="millisecondsLength">显示毫秒长度</param>
    public void DefaultLog(string title, string subTitle, string otherInfo, string message,
        bool boldTitleFont = false, bool boldSubTitleFont = false, bool boldOtherInfoFont = false, bool boldMessageFont = false, 
        bool showLogType = true, bool showDate = true, bool showTime = true, bool showMilliseconds = true, int millisecondsLength = 4)
    {
        LogMessage logMessage = new()
        {
            LogType = LogType.Default,
            Title = title,
            SubTitle = subTitle,
            OtherInfo = otherInfo,
            Message = message,
            BoldTitleFont = boldTitleFont,
            BoldSubTitleFont = boldSubTitleFont,
            BoldOtherInfoFont = boldOtherInfoFont,
            BoldMessageFont = boldMessageFont,
            ShowLogType = showLogType,
            ShowDate = showDate,
            ShowTime = showTime,
            ShowMilliseconds = showMilliseconds,
            MillisecondsLength = millisecondsLength,
        };
        Log(logMessage);
        
        // 在过滤时新增的日志处理逻辑
        if (_logTypeShowState[LogType.Default])
            GetLogsByLogType(LogType.Default, logMessage);
    }

    /// <summary>
    /// 添加 Info 日志
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="subTitle">副标题</param>
    /// <param name="otherInfo">其他信息</param>
    /// <param name="message">消息</param>
    /// <param name="boldTitleFont">是否加粗标题</param>
    /// <param name="boldSubTitleFont">是否加粗副标题</param>
    /// <param name="boldOtherInfoFont">是否加粗其他信息</param>
    /// <param name="boldMessageFont">是否加粗消息</param>
    /// <param name="showLogType">是否显示日志类型</param>
    /// <param name="showDate">是否显示日期</param>
    /// <param name="showTime">是否显示时间</param>
    /// <param name="showMilliseconds">是否显示毫秒</param>
    /// <param name="millisecondsLength">显示毫秒长度</param>
    public void InfoLog(string title, string subTitle, string otherInfo, string message,
        bool boldTitleFont = false, bool boldSubTitleFont = false, bool boldOtherInfoFont = false, bool boldMessageFont = false, 
        bool showLogType = true, bool showDate = true, bool showTime = true, bool showMilliseconds = true, int millisecondsLength = 4)
    {
        LogMessage logMessage = new()
        {
            LogType = LogType.Info,
            Title = title,
            SubTitle = subTitle,
            OtherInfo = otherInfo,
            Message = message,
            BoldTitleFont = boldTitleFont,
            BoldSubTitleFont = boldSubTitleFont,
            BoldOtherInfoFont = boldOtherInfoFont,
            BoldMessageFont = boldMessageFont,
            ShowLogType = showLogType,
            ShowDate = showDate,
            ShowTime = showTime,
            ShowMilliseconds = showMilliseconds,
            MillisecondsLength = millisecondsLength,
        };
        Log(logMessage);
        
        // 在过滤时新增的日志处理逻辑
        if (_logTypeShowState[LogType.Info])
            GetLogsByLogType(LogType.Info, logMessage);
    }

    /// <summary>
    /// 添加 Success 日志
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="subTitle">副标题</param>
    /// <param name="otherInfo">其他信息</param>
    /// <param name="message">消息</param>
    /// <param name="boldTitleFont">是否加粗标题</param>
    /// <param name="boldSubTitleFont">是否加粗副标题</param>
    /// <param name="boldOtherInfoFont">是否加粗其他信息</param>
    /// <param name="boldMessageFont">是否加粗消息</param>
    /// <param name="showLogType">是否显示日志类型</param>
    /// <param name="showDate">是否显示日期</param>
    /// <param name="showTime">是否显示时间</param>
    /// <param name="showMilliseconds">是否显示毫秒</param>
    /// <param name="millisecondsLength">显示毫秒长度</param>
    public void SuccessLog(string title, string subTitle, string otherInfo, string message,
        bool boldTitleFont = false, bool boldSubTitleFont = false, bool boldOtherInfoFont = false, bool boldMessageFont = false, 
        bool showLogType = true, bool showDate = true, bool showTime = true, bool showMilliseconds = true, int millisecondsLength = 4)
    {
        LogMessage logMessage = new()
        {
            LogType = LogType.Success,
            Title = title,
            SubTitle = subTitle,
            OtherInfo = otherInfo,
            Message = message,
            BoldTitleFont = boldTitleFont,
            BoldSubTitleFont = boldSubTitleFont,
            BoldOtherInfoFont = boldOtherInfoFont,
            BoldMessageFont = boldMessageFont,
            ShowLogType = showLogType,
            ShowDate = showDate,
            ShowTime = showTime,
            ShowMilliseconds = showMilliseconds,
            MillisecondsLength = millisecondsLength,
        };
        Log(logMessage);
        
        // 在过滤时新增的日志处理逻辑
        if (_logTypeShowState[LogType.Success])
            GetLogsByLogType(LogType.Success, logMessage);
    }

    /// <summary>
    /// 添加 Warning 日志
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="subTitle">副标题</param>
    /// <param name="otherInfo">其他信息</param>
    /// <param name="message">消息</param>
    /// <param name="boldTitleFont">是否加粗标题</param>
    /// <param name="boldSubTitleFont">是否加粗副标题</param>
    /// <param name="boldOtherInfoFont">是否加粗其他信息</param>
    /// <param name="boldMessageFont">是否加粗消息</param>
    /// <param name="showLogType">是否显示日志类型</param>
    /// <param name="showDate">是否显示日期</param>
    /// <param name="showTime">是否显示时间</param>
    /// <param name="showMilliseconds">是否显示毫秒</param>
    /// <param name="millisecondsLength">显示毫秒长度</param>
    public void WarningLog(string title, string subTitle, string otherInfo, string message,
        bool boldTitleFont = false, bool boldSubTitleFont = false, bool boldOtherInfoFont = false, bool boldMessageFont = false, 
        bool showLogType = true, bool showDate = true, bool showTime = true, bool showMilliseconds = true, int millisecondsLength = 4)
    {
        LogMessage logMessage = new()
        {
            LogType = LogType.Warning,
            Title = title,
            SubTitle = subTitle,
            OtherInfo = otherInfo,
            Message = message,
            BoldTitleFont = boldTitleFont,
            BoldSubTitleFont = boldSubTitleFont,
            BoldOtherInfoFont = boldOtherInfoFont,
            BoldMessageFont = boldMessageFont,
            ShowLogType = showLogType,
            ShowDate = showDate,
            ShowTime = showTime,
            ShowMilliseconds = showMilliseconds,
            MillisecondsLength = millisecondsLength,
        };
        Log(logMessage);
        
        // 在过滤时新增的日志处理逻辑
        if (_logTypeShowState[LogType.Warning])
            GetLogsByLogType(LogType.Warning, logMessage);
    }

    /// <summary>
    /// 添加 Error 日志
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="subTitle">副标题</param>
    /// <param name="otherInfo">其他信息</param>
    /// <param name="message">消息</param>
    /// <param name="boldTitleFont">是否加粗标题</param>
    /// <param name="boldSubTitleFont">是否加粗副标题</param>
    /// <param name="boldOtherInfoFont">是否加粗其他信息</param>
    /// <param name="boldMessageFont">是否加粗消息</param>
    /// <param name="showLogType">是否显示日志类型</param>
    /// <param name="showDate">是否显示日期</param>
    /// <param name="showTime">是否显示时间</param>
    /// <param name="showMilliseconds">是否显示毫秒</param>
    /// <param name="millisecondsLength">显示毫秒长度</param>
    public void ErrorLog(string title, string subTitle, string otherInfo, string message,
        bool boldTitleFont = false, bool boldSubTitleFont = false, bool boldOtherInfoFont = false, bool boldMessageFont = false, 
        bool showLogType = true, bool showDate = true, bool showTime = true, bool showMilliseconds = true, int millisecondsLength = 4)
    {
        LogMessage logMessage = new()
        {
            LogType = LogType.Error,
            Title = title,
            SubTitle = subTitle,
            OtherInfo = otherInfo,
            Message = message,
            BoldTitleFont = boldTitleFont,
            BoldSubTitleFont = boldSubTitleFont,
            BoldOtherInfoFont = boldOtherInfoFont,
            BoldMessageFont = boldMessageFont,
            ShowLogType = showLogType,
            ShowDate = showDate,
            ShowTime = showTime,
            ShowMilliseconds = showMilliseconds,
            MillisecondsLength = millisecondsLength,
        };
        Log(logMessage);
        
        // 在过滤时新增的日志处理逻辑
        if (_logTypeShowState[LogType.Error])
            GetLogsByLogType(LogType.Error, logMessage);
    }

    /// <summary>
    /// 添加 Fatal 日志
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="subTitle">副标题</param>
    /// <param name="otherInfo">其他信息</param>
    /// <param name="message">消息</param>
    /// <param name="boldTitleFont">是否加粗标题</param>
    /// <param name="boldSubTitleFont">是否加粗副标题</param>
    /// <param name="boldOtherInfoFont">是否加粗其他信息</param>
    /// <param name="boldMessageFont">是否加粗消息</param>
    /// <param name="showLogType">是否显示日志类型</param>
    /// <param name="showDate">是否显示日期</param>
    /// <param name="showTime">是否显示时间</param>
    /// <param name="showMilliseconds">是否显示毫秒</param>
    /// <param name="millisecondsLength">显示毫秒长度</param>
    public void FatalLog(string title, string subTitle, string otherInfo, string message,
        bool boldTitleFont = false, bool boldSubTitleFont = false, bool boldOtherInfoFont = false, bool boldMessageFont = false, 
        bool showLogType = true, bool showDate = true, bool showTime = true, bool showMilliseconds = true, int millisecondsLength = 4)
    {
        LogMessage logMessage = new()
        {
            LogType = LogType.Fatal,
            Title = title,
            SubTitle = subTitle,
            OtherInfo = otherInfo,
            Message = message,
            BoldTitleFont = boldTitleFont,
            BoldSubTitleFont = boldSubTitleFont,
            BoldOtherInfoFont = boldOtherInfoFont,
            BoldMessageFont = boldMessageFont,
            ShowLogType = showLogType,
            ShowDate = showDate,
            ShowTime = showTime,
            ShowMilliseconds = showMilliseconds,
            MillisecondsLength = millisecondsLength,
        };
        Log(logMessage);
        
        // 在过滤时新增的日志处理逻辑
        if (_logTypeShowState[LogType.Fatal])
            GetLogsByLogType(LogType.Fatal, logMessage);
    }
    

    /// <summary>
    /// 
    /// </summary>
    /// <param name="checkedFilterOptions"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void FilterLogs(List<FilterOption>? checkedFilterOptions)
    {
        if (_primaryListBoxLogger == null)
            throw new ArgumentNullException(nameof(_primaryListBoxLogger), "LogListBox not registered");
        if (_secondListBoxLogger == null)
            throw new ArgumentNullException(nameof(_secondListBoxLogger), "LogListBox not registered");

        // 是否已过滤日志，
        _isConfirmFiltering = (checkedFilterOptions != null && checkedFilterOptions.Count != 0);
        _primaryListBoxLogger.IsVisible = !_isConfirmFiltering;
        _secondListBoxLogger.IsVisible = _isConfirmFiltering;
        
        // TODO: 未完成
        
        // 组合条件
        List<LogMessage> filteredResults = _logMessagesListCache!.Where(x => x.LogType == LogType.Success).ToList();
        
        if (_isConfirmFiltering && filteredResults.Count > 0)
        {
            _secondListBoxLogger.Items.Clear();
            foreach (var oldLogMessage in filteredResults)
            {
                RestoreLogFromOldLogMessage(oldLogMessage);
            }
        }
    }

    /// <summary>
    /// 获取指定类型的日志
    /// </summary>
    /// <param name="logType"></param>
    /// <param name="newLogWhenFiltering">不需要传入该参数，该参数仅是为了方便（非 null 表示是在过滤时新增的日志）</param>
    /// <exception cref="ArgumentNullException"></exception>
    public void GetLogsByLogType(LogType logType, LogMessage? newLogWhenFiltering = null)
    {
        if (_primaryListBoxLogger == null)
            throw new ArgumentNullException(nameof(_primaryListBoxLogger), "LogListBox not registered");
        if (_secondListBoxLogger == null)
            throw new ArgumentNullException(nameof(_secondListBoxLogger), "LogListBox not registered");

        // 检查调用者的类型
        var callingType = new System.Diagnostics.StackTrace().GetFrame(1)?.GetMethod()?.DeclaringType;
        // 若调用者不是 ListBoxLogger 类，将参数设置为null，以避出现非预期问题
        if (callingType != typeof(ListBoxLogger))
            newLogWhenFiltering = null;
        
        if (newLogWhenFiltering == null)
        {
            // 已经是当前选中类型，直接退出（防重复点击刷新）
            if (_logTypeShowState[logType])
                return;
            
            // 全部置false，仅当前选中类型置true（替换一堆赋值）
            foreach (var kv in _logTypeShowState.ToList())
                _logTypeShowState[kv.Key] = false;
            _logTypeShowState[logType] = true;

            if (logType == LogType.Total)
            {
                UpdateLoggerVisible(isPrimaryListBoxLoggerVisible: true);
                if (_primaryListBoxLogger.Items.Count == 0)
                    return;
                
                // 防止在过滤时添加日志，而 _primaryListBoxLogger 没有滚动到最新日志
                _primaryListBoxLogger.Dispatcher.InvokeAsync(() =>
                {
                    object selectItem = !Desc 
                        ? _primaryListBoxLogger.Items[^1]! // 正序：最后一条
                        : _primaryListBoxLogger.Items[0]!; // 倒序：第一条
            
                    _primaryListBoxLogger.SelectedItem = selectItem;
                    _primaryListBoxLogger.ScrollIntoView(selectItem);
                });
                return;
            }
            
            // 切换到第二个ListBox展示筛选数据
            UpdateLoggerVisible(isPrimaryListBoxLoggerVisible: false);
            
            // 筛选缓存日志
            var filteredResults = _logMessagesListCache!.Where(x => x.LogType == logType).ToList();
            if (filteredResults.Count == 0)
            {
                _secondListBoxLogger.Items.Clear();
                return;
            }
            
            // 清空旧数据 + 批量恢复日志到UI
            _secondListBoxLogger.Items.Clear();
            foreach (var oldLogMessage in filteredResults)
            {
                RestoreLogFromOldLogMessage(oldLogMessage);
            }
        }
        else
        {
            RestoreLogFromOldLogMessage(newLogWhenFiltering);
        }
        
        // _secondListBoxLogger 滚动到最新日志
        if(_secondListBoxLogger.Items.Count == 0) 
            return;
        _secondListBoxLogger.Dispatcher.InvokeAsync(() =>
        {
            object selectItem = !Desc 
                ? _secondListBoxLogger.Items[^1]! // 正序：最后一条
                : _secondListBoxLogger.Items[0]!; // 倒序：第一条
            
            _secondListBoxLogger.SelectedItem = selectItem;
            _secondListBoxLogger.ScrollIntoView(selectItem);
        });
    }

    private void UpdateLoggerVisible(bool isPrimaryListBoxLoggerVisible)
    {
        if (isPrimaryListBoxLoggerVisible)
        {
            _primaryListBoxLogger?.IsVisible = true;
            _secondListBoxLogger?.IsVisible = false;
        }
        else
        {
            _primaryListBoxLogger?.IsVisible = false;
            _secondListBoxLogger?.IsVisible = true;
        }
    }
    
    /// <summary>
    /// 将旧 LogMessage 恢复为日志
    /// </summary>
    /// <param name="oldLogMessage"></param>
    private void RestoreLogFromOldLogMessage(LogMessage oldLogMessage)
    {
        if (_secondListBoxLogger == null)
            throw new ArgumentNullException(nameof(_secondListBoxLogger), "LogListBox not registered");

        // Create a new StackPanel for the log entry
        StackPanel logEntry = new StackPanel { Tag = oldLogMessage.LogType.ToString() };
        // Create a horizontal StackPanel for the timestamp and titles
        StackPanel headerPanel = new StackPanel { Orientation = Orientation.Horizontal };

        string tempText = "";
        string demoDateTimeStr = "2026-05-31 12:42.1234";
        
        var parts = oldLogMessage.DateTimeStr.Split(new char[] { ' ', '.' }, StringSplitOptions.RemoveEmptyEntries);
        string dateStr = parts[0];                  // "2026-05-31"
        string timeStr = parts[1];                  // "12:42"
        string millisecondsStr = $".{parts[2]}";    // ".1234"
        
        // Add timestamp if required
        if (oldLogMessage.ShowDate || oldLogMessage.ShowTime)
        {
            string timestamp = "";
            if (oldLogMessage.ShowDate) 
                timestamp += dateStr;
            if (oldLogMessage.ShowTime) 
                timestamp += (timestamp.Length > 0 ? " " : "") + timeStr;
            if (oldLogMessage.ShowTime && oldLogMessage.ShowMilliseconds)     // 等于 (logMessage is { ShowTime: true, ShowMilliseconds: true })
                timestamp += millisecondsStr;
            
            oldLogMessage.DateTimeStr = timestamp;
            TextBlock timestampText = new TextBlock { Text = $"[{timestamp}] " };
            BindForeground(oldLogMessage.LogType, timestampText);
            headerPanel.Children.Add(timestampText);
        }
        
        // Add logTypeText
        // if (logMessage.ShowLogType)
        // {
            TextBlock typeText = new TextBlock
            {
                Text = $"[{oldLogMessage.LogType.ToString().Substring(0, 3).ToUpperInvariant()}] ", 
                IsVisible = oldLogMessage.ShowLogType, // 用 IsVisible 代替 if 判断，这样在双击选中项复制时就会有 LogType 文本
            };
            BindForeground(oldLogMessage.LogType, typeText);
            headerPanel.Children.Add(typeText);
        // }
        
        // Add title
        if (!string.IsNullOrWhiteSpace(oldLogMessage.Title))
        {
            tempText = $"[{oldLogMessage.Title}] ";
            TextBlock titleText = !oldLogMessage.BoldTitleFont
                ? new TextBlock { Text = tempText, TextWrapping = TextWrapping.Wrap }
                : new TextBlock { Text = tempText, TextWrapping = TextWrapping.Wrap, FontWeight = FontWeight.Bold };
            BindForeground(oldLogMessage.LogType, titleText);
            headerPanel.Children.Add(titleText);
        }
        
        // Add subtitle
        if (!string.IsNullOrWhiteSpace(oldLogMessage.SubTitle))
        {
            tempText = $"[{oldLogMessage.SubTitle}] ";
            TextBlock subTitleText = !oldLogMessage.BoldSubTitleFont
                ? new TextBlock { Text = tempText, TextWrapping = TextWrapping.Wrap }
                : new TextBlock { Text = tempText, TextWrapping = TextWrapping.Wrap, FontWeight = FontWeight.Bold };
            BindForeground(oldLogMessage.LogType, subTitleText);
            headerPanel.Children.Add(subTitleText);
        }
        
        // Add otherInfo
        if (!string.IsNullOrWhiteSpace(oldLogMessage.OtherInfo))
        {
            tempText = $"[{oldLogMessage.OtherInfo}] ";
            TextBlock otherInfoText = !oldLogMessage.BoldOtherInfoFont
                ? new TextBlock { Text = tempText, TextWrapping = TextWrapping.Wrap }
                : new TextBlock { Text = tempText, TextWrapping = TextWrapping.Wrap, FontWeight = FontWeight.Bold };
            BindForeground(oldLogMessage.LogType, otherInfoText);
            headerPanel.Children.Add(otherInfoText);
        }

        // Add the header panel to the log entry
        logEntry.Children.Add(headerPanel);

        // Add the message
        tempText = !string.IsNullOrWhiteSpace(oldLogMessage.Message) ? oldLogMessage.Message : "Empty Message";
        TextBlock messageText = !oldLogMessage.BoldMessageFont 
            ? new TextBlock { Text = tempText, TextWrapping = TextWrapping.WrapWithOverflow }
            : new TextBlock { Text = tempText, TextWrapping = TextWrapping.WrapWithOverflow, FontWeight = FontWeight.Bold };
        BindForeground(oldLogMessage.LogType, messageText);
        tempText = string.Empty;
        
        // Add the message TextBlock to the log entry
        logEntry.Children.Add(messageText);

        // Add the log entry to the ListBox
        if (!Desc)
        {
            // 降序
            // 添加日志
            _secondListBoxLogger.Items.Insert(0, logEntry);
        }
        else
        {
            // 升序
            // 添加日志
            _secondListBoxLogger.Items.Add(logEntry);
        }
    }

    /// <summary>
    /// 清空所有日志
    /// </summary>
    public void ClearAllLogs()
    {
        _isConfirmFiltering = false;
        UpdateLoggerVisible(isPrimaryListBoxLoggerVisible: true);
        _primaryListBoxLogger?.Items.Clear();
        _secondListBoxLogger?.Items.Clear();
        _logMessagesListCache?.Clear();
        LogCount_Tip = 0;
        LogCount_Default = 0;
        LogCount_Info = 0;
        LogCount_Success = 0;
        LogCount_Warning = 0;
        LogCount_Error = 0;
        LogCount_Fatal = 0;
    }

    private LogType GetLogTypeToBeRemoved(StackPanel logPanel)
    {
        var tag = logPanel.Tag;
        if (tag == null || string.IsNullOrWhiteSpace(tag.ToString()))
            return LogType.Tip;
        
        string logTypeString = tag.ToString()!.Trim('[').Trim(' ').Trim(']');
        
        return Enum.TryParse<LogType>(logTypeString, out var logType) ? logType : LogType.Tip;

        // 参考示例 CultureInfo.CurrentCulture.TextInfo
        // 将 全大全的字符串 转换为 首字母大写的字符串
        // TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
        // string str = textInfo.ToTitleCase(logTypeString.ToLower()).Substring(0, 3);
        // return str switch
        // {
        //     "Tip" => LogType.Tip,
        //     "Def" => LogType.Default,
        //     "Inf" => LogType.Info,
        //     "Suc" => LogType.Success,
        //     "War" => LogType.Warning,
        //     "Err" => LogType.Error,
        //     "Fat" => LogType.Fatal,
        //     _ => LogType.Tip
        // };
    }
    
    private void CalculateLogCount(LogType logType, bool add)
    {
        switch (logType)
        {
            case LogType.Tip: if (add) LogCount_Tip++; else LogCount_Tip--; break;
            case LogType.Default: if (add) LogCount_Default++; else LogCount_Default--; break;
            case LogType.Info: if (add) LogCount_Info++; else LogCount_Info--; break;
            case LogType.Success: if (add) LogCount_Success++; else LogCount_Success--; break;
            case LogType.Warning: if (add) LogCount_Warning++; else LogCount_Warning--; break;
            case LogType.Error: if (add) LogCount_Error++; else LogCount_Error--; break;
            case LogType.Fatal: if (add) LogCount_Fatal++; else LogCount_Fatal--; break;
            // default: break;
        }
    }

    private void BindForeground(LogType logType, TextBlock textBlock)
    {
        switch (logType)
        {
            case LogType.Tip: textBlock.Bind(TextBlock.ForegroundProperty, _dynamicResourceTextBlockTertiaryForeground); return;
            case LogType.Default: textBlock.Bind(TextBlock.ForegroundProperty, _dynamicResourceTextBlockDefaultForeground); return;
            case LogType.Info: textBlock.Bind(TextBlock.ForegroundProperty, _dynamicResourceTextBlockInfoForeground); return;
            case LogType.Success: textBlock.Bind(TextBlock.ForegroundProperty, _dynamicResourceTextBlockSuccessForeground); return;
            case LogType.Warning: textBlock.Bind(TextBlock.ForegroundProperty, _dynamicResourceTextBlockWarningForeground); return;
            case LogType.Error: textBlock.Bind(TextBlock.ForegroundProperty, _dynamicResourceTextBlockDangerForeground); return;
            case LogType.Fatal: textBlock.Bind(TextBlock.ForegroundProperty, _dynamicResourceTextBlockFatalForeground); return;
            default: textBlock.Bind(TextBlock.ForegroundProperty, _dynamicResourceTextBlockTertiaryForeground); return;
        }
    }
    
}