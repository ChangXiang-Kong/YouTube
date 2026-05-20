using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AvaloniaApplication1.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.Models;

public partial class AppSettings : ViewModelBase
{
    #region 常规

    /// <summary>
    /// 语言选项
    /// </summary>
    public List<string> Langs
    {
        get => field.Order().ToList();
    } =
    [
        "简体中文",
        "English",
        "Anglish",
        "繁体中文"
    ];
    // 上 等于 下
    // private readonly List<string> _langsInternal =
    // [
    //     "简体中文",
    //     "English",
    //     "Anglish",
    //     "繁体中文"
    // ];
    // public List<string> Langs => _langsInternal.Order().ToList();

    /// <summary>
    /// 语言
    /// </summary>
    [Description("语言")] [ObservableProperty]
    private string _selectedLang = "简体中文";

    /// <summary>
    /// 在计算机开机时自动启动
    /// </summary>
    [Description("在计算机开机时自动启动")] [ObservableProperty]
    private bool _autoStart = true;

    /// <summary>
    /// 启动时自动检查更新
    /// </summary>
    [Description("启动时自动检查更新")] [ObservableProperty]
    private bool _autoCheckUpdates = true;

    #endregion 常规

    #region 账户

    #endregion 账户

    #region 通知

    #endregion 通知

    #region 界面

    /// <summary>
    /// 显示查看新闻按钮
    /// </summary>
    [Description("显示查看新闻按钮")] [ObservableProperty]
    private bool _displayViewNewsButton = true;

    /// <summary>
    /// 显示查看通知按钮
    /// </summary>
    [Description("显示查看通知按钮")] [ObservableProperty]
    private bool _displayViewNotificationsButton = true;

    /// <summary>
    /// 显示查看日志按钮
    /// </summary>
    [Description("显示查看日志按钮")] [ObservableProperty]
    private bool _displayViewLogButton = true;

    #endregion 界面



    /// <summary>
    /// 深拷贝（用于创建临时副本）
    /// </summary>
    /// <returns></returns>
    public AppSettings Clone()
    {
        return new AppSettings
        {
            DisplayViewNewsButton = DisplayViewNewsButton,
            DisplayViewNotificationsButton = DisplayViewNotificationsButton,
            DisplayViewLogButton = DisplayViewLogButton,
        };
    }
    
    /// <summary>
    /// 从本地加载
    /// </summary>
    /// <returns></returns>
    public AppSettings Load()
    {
        return null;
    }

    /// <summary>
    /// 保存到本地文件或数据库
    /// </summary>
    public void Save()
    {
    }
    
    
}