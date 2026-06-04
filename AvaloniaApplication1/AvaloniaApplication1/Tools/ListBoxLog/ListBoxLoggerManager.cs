using System;
using System.Collections.Concurrent;
using Avalonia.Controls;

namespace AvaloniaApplication1.Tools.ListBoxLog;

public class ListBoxLoggerManager
{
    public ListBoxLoggerManager() { }
    private static readonly Lazy<ListBoxLoggerManager> _instance = new Lazy<ListBoxLoggerManager>(() => new ListBoxLoggerManager());
    public static ListBoxLoggerManager Instance => _instance.Value;

    private readonly ConcurrentDictionary<string, ListBoxLogger> _loggers = new();

    /// <summary>
    /// 绑定 ListBox
    /// </summary>
    /// <param name="loggerName"></param>
    /// <param name="primaryListBox">主ListBox（显示全部日志）</param>
    /// <param name="secondListBox">副ListBox（显示过滤后的日志）</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void RegisterListBoxLogger(string loggerName, ListBox primaryListBox, ListBox secondListBox)
    {
        if (string.IsNullOrWhiteSpace(loggerName))
            throw new ArgumentNullException(nameof(loggerName), "LoggerName can not be null");
        
        if (primaryListBox == null)
            throw new ArgumentNullException(nameof(primaryListBox), "Logger can not be null");
        if (secondListBox == null)
            throw new ArgumentNullException(nameof(secondListBox), "Logger can not be null");

        Instance._loggers.TryAdd(loggerName, new ListBoxLogger(loggerName, primaryListBox, secondListBox));
        Instance._loggers[loggerName].ClearAllLogs();
    }

    /// <summary>
    /// 取消绑定日志ListBox
    /// </summary>
    public static void UnregisterListBoxLogger(string loggerName)
    {
        GetLoggerByName(loggerName).UnregisterListBoxLogger();
        Instance._loggers.TryRemove(loggerName, out _);
    }
    
    public static ListBoxLogger GetLoggerByName(string loggerName)
    {
        return Instance._loggers.TryGetValue(loggerName, out var logger)
            ? logger
            : throw new ArgumentException($"Logger {loggerName} not found. Possible reasons: LogListBox not registered or input a wrong {nameof(loggerName)} param");
    }
    
    /// <summary>
    /// 清空所有日志
    /// </summary>
    /// <param name="loggerName"></param>
    /// <exception cref="ArgumentException"></exception>
    public static void ClearAllLogs(string loggerName)
    {
       if (!Instance._loggers.TryGetValue(loggerName, out var logger))
            throw new ArgumentException($"Logger {loggerName} not found. Possible reasons: LogListBox not registered or input a wrong {nameof(loggerName)} param");
       
       logger.ClearAllLogs();
    }

    /// <summary>
    /// 添加 Tip 日志
    /// </summary>
    /// <param name="loggerName"></param>
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
    public static void TipLog(string loggerName, 
        string title, string subTitle, string otherInfo, string message,
        bool boldTitleFont = false, bool boldSubTitleFont = false, bool boldOtherInfoFont = false, bool boldMessageFont = false, 
        bool showLogType = true, bool showDate = true, bool showTime = true, bool showMilliseconds = true, int millisecondsLength = 4)
    {
        GetLoggerByName(loggerName).TipLog(title, subTitle, otherInfo, message,
            boldTitleFont, boldSubTitleFont, boldOtherInfoFont, boldMessageFont,
            showLogType, showDate, showTime, showMilliseconds, millisecondsLength);
    }

    /// <summary>
    /// 添加 Default 日志
    /// </summary>
    /// <param name="loggerName"></param>
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
    public static void DefaultLog(string loggerName, 
        string title, string subTitle, string otherInfo, string message,
        bool boldTitleFont = false, bool boldSubTitleFont = false, bool boldOtherInfoFont = false, bool boldMessageFont = false, 
        bool showLogType = true, bool showDate = true, bool showTime = true, bool showMilliseconds = true, int millisecondsLength = 4)
    {
        GetLoggerByName(loggerName).DefaultLog(title, subTitle, otherInfo, message,
            boldTitleFont, boldSubTitleFont, boldOtherInfoFont, boldMessageFont,
            showLogType, showDate, showTime, showMilliseconds, millisecondsLength);
    }

    /// <summary>
    /// 添加 Info 日志
    /// </summary>
    /// <param name="loggerName"></param>
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
    public static void InfoLog(string loggerName, 
        string title, string subTitle, string otherInfo, string message,
        bool boldTitleFont = false, bool boldSubTitleFont = false, bool boldOtherInfoFont = false, bool boldMessageFont = false, 
        bool showLogType = true, bool showDate = true, bool showTime = true, bool showMilliseconds = true, int millisecondsLength = 4)
    {
        GetLoggerByName(loggerName).InfoLog(title, subTitle, otherInfo, message,
            boldTitleFont, boldSubTitleFont, boldOtherInfoFont, boldMessageFont,
            showLogType, showDate, showTime, showMilliseconds, millisecondsLength);
    }

    /// <summary>
    /// 添加 Success 日志
    /// </summary>
    /// <param name="loggerName"></param>
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
    public static void SuccessLog(string loggerName, 
        string title, string subTitle, string otherInfo, string message,
        bool boldTitleFont = false, bool boldSubTitleFont = false, bool boldOtherInfoFont = false, bool boldMessageFont = false, 
        bool showLogType = true, bool showDate = true, bool showTime = true, bool showMilliseconds = true, int millisecondsLength = 4)
    {
        GetLoggerByName(loggerName).SuccessLog(title, subTitle, otherInfo, message,
            boldTitleFont, boldSubTitleFont, boldOtherInfoFont, boldMessageFont,
            showLogType, showDate, showTime, showMilliseconds, millisecondsLength);
    }

    /// <summary>
    /// 添加 Warning 日志
    /// </summary>
    /// <param name="loggerName"></param>
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
    public static void WarningLog(string loggerName, 
        string title, string subTitle, string otherInfo, string message,
        bool boldTitleFont = false, bool boldSubTitleFont = false, bool boldOtherInfoFont = false, bool boldMessageFont = false, 
        bool showLogType = true, bool showDate = true, bool showTime = true, bool showMilliseconds = true, int millisecondsLength = 4)
    {
        GetLoggerByName(loggerName).WarningLog(title, subTitle, otherInfo, message,
            boldTitleFont, boldSubTitleFont, boldOtherInfoFont, boldMessageFont,
            showLogType, showDate, showTime, showMilliseconds, millisecondsLength);
    }

    /// <summary>
    /// 添加 Error 日志
    /// </summary>
    /// <param name="loggerName"></param>
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
    public static void ErrorLog(string loggerName, 
        string title, string subTitle, string otherInfo, string message,
        bool boldTitleFont = false, bool boldSubTitleFont = false, bool boldOtherInfoFont = false, bool boldMessageFont = false, 
        bool showLogType = true, bool showDate = true, bool showTime = true, bool showMilliseconds = true, int millisecondsLength = 4)
    {
        GetLoggerByName(loggerName).ErrorLog(title, subTitle, otherInfo, message,
            boldTitleFont, boldSubTitleFont, boldOtherInfoFont, boldMessageFont,
            showLogType, showDate, showTime, showMilliseconds, millisecondsLength);
    }

    /// <summary>
    /// 添加 Fatal 日志
    /// </summary>
    /// <param name="loggerName"></param>
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
    public static void FatalLog(string loggerName, 
        string title, string subTitle, string otherInfo, string message,
        bool boldTitleFont = false, bool boldSubTitleFont = false, bool boldOtherInfoFont = false, bool boldMessageFont = false, 
        bool showLogType = true, bool showDate = true, bool showTime = true, bool showMilliseconds = true, int millisecondsLength = 4)
    {
        GetLoggerByName(loggerName).FatalLog(title, subTitle, otherInfo, message,
            boldTitleFont, boldSubTitleFont, boldOtherInfoFont, boldMessageFont,
            showLogType, showDate, showTime, showMilliseconds, millisecondsLength);
    }
    
}