using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using AvaloniaApplication1.Tools.Services;
using AvaloniaApplication1.ViewModels;
using Ursa.Controls;

namespace AvaloniaApplication1.Views;

// public partial class MainWindow : Window
public partial class MainWindow : UrsaWindow
{
    public MainWindow()
    {
        InitializeComponent();
        // NotificationManager = new WindowNotificationManager(this) { MaxItems = 6 };
        KeyDown += FullScreenKeyDown;
        /*// NavDemo
        // 初始化导航服务
        App.NavigationService?.Initialize(MainNav);
        // 设置首页
        App.NavigationService?.PushAsync<HomePage_NavDemo, HomePageViewModel_NavDemo>();
        */
    }
    
    // public WindowNotificationManager? NotificationManager { get; set; }

    

    #region F11ToggleFullScreen

    private void FullScreenKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.F11)
        {
            ToggleFullScreen();
            e.Handled = true;
        }
    }
    private void ToggleFullScreen()
    {
        WindowState = WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }
    
    #endregion F11ToggleFullScreen

    

}