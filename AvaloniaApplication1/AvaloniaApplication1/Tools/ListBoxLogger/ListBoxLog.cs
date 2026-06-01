using System;
using System.Globalization;
using System.Text;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using AvaloniaApplication1.Tools.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaApplication1.Tools.ListBoxLogger;

public partial class ListBoxLog : ObservableObject
{
    internal ListBoxLog(ListBox listBox, string name, bool desc)
    {
        _logListBox = listBox ?? throw new ArgumentNullException(nameof(listBox), "Param cannot be null");
        Name = name;
        Desc = desc;
        // 解绑旧事件，避免多次绑定
        _logListBox.DoubleTapped -= LogListBox_DoubleTapped;
        // 绑定双击事件
        _logListBox.DoubleTapped += LogListBox_DoubleTapped;
    }
    
    private ListBox? _logListBox;
    private readonly DynamicResourceExtension _dynamicResourceTextBlockTertiaryForeground = new("TextBlockTertiaryForeground");
    private readonly DynamicResourceExtension _dynamicResourceTextBlockDefaultForeground = new("TextBlockDefaultForeground");
    private readonly DynamicResourceExtension _dynamicResourceTextBlockInfoForeground = new("TextBlockInfoForeground");
    private readonly DynamicResourceExtension _dynamicResourceTextBlockSuccessForeground = new("TextBlockSuccessForeground");
    private readonly DynamicResourceExtension _dynamicResourceTextBlockWarningForeground = new("TextBlockWarningForeground");
    private readonly DynamicResourceExtension _dynamicResourceTextBlockDangerForeground = new("TextBlockDangerForeground");
    private readonly DynamicResourceExtension _dynamicResourceTextBlockFatalForeground = new("SemiAIPurple5");

    [ObservableProperty] private string _name;
    /// <summary>
    /// false 按时间正排序；true 按时间倒排序
    /// </summary>
    [ObservableProperty] private bool _desc;
    [ObservableProperty] private int _maxLogCount = 10;
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
    public void UnregisterLogListBox()
    {
        _logListBox?.DoubleTapped -= LogListBox_DoubleTapped;
        _logListBox = null;
    }

    #region 双击复制文本事件
    
    private void LogListBox_DoubleTapped(object? sender, TappedEventArgs e)
    {
        if (_logListBox?.SelectedItem is not StackPanel logPanel)
            return;

        // 递归提取当前日志项内所有文本
        string allText = GetAllTextFromVisual(logPanel);
        if (string.IsNullOrWhiteSpace(allText))
            return;
        
        // 获取剪贴板
        var clipboard = ResourceHelper.ResolveDefaultTopLevel()?.Clipboard;
        // 写入剪贴板
        clipboard?.SetTextAsync(allText);
        // 通知
        WeakReferenceMessenger.Default.Send($"{Name}|已复制到剪贴板", $"{Name}");
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
    
    #endregion 双击复制文本事件

    /// <summary>
    /// 添加日志
    /// </summary>
    /// <param name="logMessage">日志对象</param>
    public void Log(LogMessage logMessage)
    {
        if (_logListBox == null)
            throw new ArgumentNullException(nameof(_logListBox), "LogListBox not registered");

        CalculateLogCount(logMessage.LogType, true);

        // Create a new StackPanel for the log entry
        StackPanel logEntry = new StackPanel();
        // Create a horizontal StackPanel for the timestamp and titles
        StackPanel headerPanel = new StackPanel { Orientation = Orientation.Horizontal };
        
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
            TextBlock titleText = !logMessage.BoldTitleFont
                ? new TextBlock { Text = $"[{logMessage.Title}] ", TextWrapping = TextWrapping.Wrap }
                : new TextBlock { Text = $"[{logMessage.Title}] ", TextWrapping = TextWrapping.Wrap, FontWeight = FontWeight.Bold };
            BindForeground(logMessage.LogType, titleText);
            headerPanel.Children.Add(titleText);
        }
        
        // Add subtitle
        if (!string.IsNullOrWhiteSpace(logMessage.SubTitle))
        {
            TextBlock subTitleText = !logMessage.BoldSubTitleFont
                ? new TextBlock { Text = $"[{logMessage.SubTitle}] ", TextWrapping = TextWrapping.Wrap }
                : new TextBlock { Text = $"[{logMessage.SubTitle}] ", TextWrapping = TextWrapping.Wrap, FontWeight = FontWeight.Bold };
            BindForeground(logMessage.LogType, subTitleText);
            headerPanel.Children.Add(subTitleText);
        }
        
        // Add otherInfo
        if (!string.IsNullOrWhiteSpace(logMessage.OtherInfo))
        {
            TextBlock otherInfoText = !logMessage.BoldOtherInfoFont
                ? new TextBlock { Text = $"[{logMessage.OtherInfo}] ", TextWrapping = TextWrapping.Wrap }
                : new TextBlock { Text = $"[{logMessage.OtherInfo}] ", TextWrapping = TextWrapping.Wrap, FontWeight = FontWeight.Bold };
            BindForeground(logMessage.LogType, otherInfoText);
            headerPanel.Children.Add(otherInfoText);
        }

        // Add the header panel to the log entry
        logEntry.Children.Add(headerPanel);

        // Add the message
        TextBlock messageText = !string.IsNullOrWhiteSpace(logMessage.Message) 
            ? !logMessage.BoldMessageFont 
                ? new TextBlock { Text = logMessage.Message, TextWrapping = TextWrapping.WrapWithOverflow }
                : new TextBlock { Text = logMessage.Message, TextWrapping = TextWrapping.WrapWithOverflow, FontWeight = FontWeight.Bold }
            : !logMessage.BoldMessageFont 
                ? new TextBlock { Text = "Empty Message" }
                : new TextBlock { Text = "Empty Message", FontWeight = FontWeight.Bold  };
        BindForeground(logMessage.LogType, messageText);
        
        // Add the message TextBlock to the log entry
        logEntry.Children.Add(messageText);

        // Add the log entry to the ListBox
        if (Desc)
        {
            _logListBox.Items.Insert(0, logEntry);

            // Ensure we only keep MaxLogCount entries
            // 超出最大条数移除最后一项
            while (_logListBox.Items.Count > MaxLogCount)
            {
                StackPanel toRemove = (_logListBox.Items[^1] as StackPanel)!;   // 等于 _logListBox.Items[_logListBox.Items.Count - 1];
                _logListBox.Items.Remove(toRemove);
                CalculateLogCount(GetLogTypeToBeRemoved(toRemove), false);
            }
        }
        else
        {
            _logListBox.Items.Add(logEntry);

            // Ensure we only keep MaxLogCount entries
            // 超出最大条数移除第一项
            while (_logListBox.Items.Count > MaxLogCount)
            {
                StackPanel toRemove = (_logListBox.Items[0] as StackPanel)!;
                _logListBox.Items.Remove(toRemove);
                CalculateLogCount(GetLogTypeToBeRemoved(toRemove), false);
            }
        }
        
        // 滚动到当前日志项并选中
        // 使用 Dispatcher 防止报错：System.InvalidOperationException: Invalid Arrange rectangle.
        _logListBox.Dispatcher.InvokeAsync(() =>
        {
            _logListBox.ScrollIntoView(logEntry);
            _logListBox.SelectedItem = logEntry;
        });
    }

    /// <summary>
    /// 清空所有日志
    /// </summary>
    public void Clear()
    {
        _logListBox?.Items.Clear();
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
        if (logPanel.Children.Count > 0 && logPanel.Children[0] is StackPanel headerPanel)
        {
            if (headerPanel.Children[1] is TextBlock logTypeText)
            {
                string logTypeString = logTypeText.Text.Trim('[').Trim(' ').Trim(']');
                
                // // 将 全大全的字符串 转换为 首字母大写的字符串
                // TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                // if (Enum.TryParse<LogType>(textInfo.ToTitleCase(logTypeString.ToLower()), out var logType))
                // {
                //     return logType;
                // }

                return logTypeString.ToUpper() switch
                {
                    "TIP" => LogType.Tip,
                    "DEF" => LogType.Default,
                    "INF" => LogType.Info,
                    "SUC" => LogType.Success,
                    "WAR" => LogType.Warning,
                    "ERR" => LogType.Error,
                    "FAT" => LogType.Fatal,
                    _ => LogType.Tip
                };
            }
        }

        // 如果无法获取 LogType，则返回默认值
        return LogType.Tip;
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
            // default:
            //     break;
        }
    }

    private void BindForeground(LogType logType, TextBlock textBlock)
    {
        switch (logType)
        {
            case LogType.Tip:
                textBlock.Bind(TextBlock.ForegroundProperty, _dynamicResourceTextBlockTertiaryForeground);
                return;
            case LogType.Default:
                textBlock.Bind(TextBlock.ForegroundProperty, _dynamicResourceTextBlockDefaultForeground);
                return;
            case LogType.Info:
                textBlock.Bind(TextBlock.ForegroundProperty, _dynamicResourceTextBlockInfoForeground);
                return;
            case LogType.Success:
                textBlock.Bind(TextBlock.ForegroundProperty, _dynamicResourceTextBlockSuccessForeground);
                return;
            case LogType.Warning:
                textBlock.Bind(TextBlock.ForegroundProperty, _dynamicResourceTextBlockWarningForeground);
                return;
            case LogType.Error:
                textBlock.Bind(TextBlock.ForegroundProperty, _dynamicResourceTextBlockDangerForeground);
                return;
            case LogType.Fatal:
                textBlock.Bind(TextBlock.ForegroundProperty, _dynamicResourceTextBlockFatalForeground);
                return;
            default:
                textBlock.Bind(TextBlock.ForegroundProperty, _dynamicResourceTextBlockTertiaryForeground);
                return;
        }
    }
    
}