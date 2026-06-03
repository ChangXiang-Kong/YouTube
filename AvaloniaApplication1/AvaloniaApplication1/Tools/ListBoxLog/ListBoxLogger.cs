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
    private List<LogMessage>? _logMessagesListCache = [];

    /// <summary>
    /// 是否过滤日志，false 显示主ListBox（显示全部日志），true 显示副ListBox（显示过滤后的日志）
    /// </summary>
    [ObservableProperty] private bool _isConfirmFiltering;
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
        Clear();
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
    public void Log(LogMessage logMessage)
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
        IsConfirmFiltering = (checkedFilterOptions != null && checkedFilterOptions.Count != 0);
        
        // 组合条件
        List<LogMessage> filteredResults = _logMessagesListCache!.Where(x => x.LogType == LogType.Success).ToList();
        
        if (IsConfirmFiltering && filteredResults.Count > 0)
        {
            _secondListBoxLogger.Items.Clear();
            foreach (var oldLogMessage in filteredResults)
            {
                RestoreLogFromOldLogMessage(oldLogMessage);
            }
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
            // 升序
            // 添加日志
            _secondListBoxLogger.Items.Add(logEntry);
        }
        else
        {
            // 降序
            // 添加日志
            _secondListBoxLogger.Items.Insert(0, logEntry);
        }
        
        // 滚动到当前日志项并选中
        // 使用 Dispatcher 防止报错：System.InvalidOperationException: Invalid Arrange rectangle.
        _secondListBoxLogger.Dispatcher.InvokeAsync(() =>
        {
            _secondListBoxLogger.ScrollIntoView(logEntry);
            _secondListBoxLogger.SelectedItem = logEntry;
        });
    }

    /// <summary>
    /// 清空所有日志
    /// </summary>
    public void Clear()
    {
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
        IsConfirmFiltering = false;
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