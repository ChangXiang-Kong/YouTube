using System.Collections.ObjectModel;
using Avalonia.Controls.ApplicationLifetimes;
using AvaloniaApplication1.Controls.Models;
using AvaloniaApplication1.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication1.ViewModels;

public partial class SettingsWindowViewModel : PageViewModel
{
    /// <summary>
    /// Disign-time only constructor
    /// </summary>
    public SettingsWindowViewModel() : base(ApplicationPageName.Settings)
    {
    }
    // 使用上面的方式替代以下方式构造函数
    // public SettingsPageViewModel()
    // {
    //     PageName = ApplicationPageName.Settings;
    //     LoadSettings();
    // }

    public SettingsWindowViewModel(AppSettings appSettings) : base(ApplicationPageName.Settings)
    {
        _appSettings = appSettings;
        TempSettings = appSettings.Clone();     // 打开页面时克隆副本
    }

    private readonly AppSettings _appSettings;  // 真实全局配置
    public AppSettings TempSettings { get; }    // 临时编辑副本（UI绑定这个）

    [ObservableProperty] private string _test = "Test Settings";


    /// <summary>
    /// 确定 = 应用 + 关闭
    /// </summary>
    [RelayCommand]
    void Ok()
    {
        Apply();
        Close();
    }

    /// <summary>
    /// 取消 = 直接关闭（不覆盖）
    /// </summary>
    [RelayCommand]
    void Cancel()
    {
        Close();
    }

    /// <summary>
    /// 应用：保存副本 → 真实配置
    /// </summary>
    [RelayCommand]
    void Apply()
    {
        _appSettings.AutoStart = TempSettings.AutoStart;
        _appSettings.DisplayViewNewsButton = TempSettings.DisplayViewNewsButton;
        _appSettings.DisplayViewNotificationsButton = TempSettings.DisplayViewNotificationsButton;
        _appSettings.DisplayViewLogButton = TempSettings.DisplayViewLogButton;

        // TODO: 保存到文件/数据库
    }

    /// <summary>
    /// 关闭窗口（实际项目可改用INavigationService）
    /// </summary>
    void Close()
    {
        // 你可以换成导航返回
        if (App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow!.Close();
        }
    }
}