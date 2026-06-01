using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaApplication1.Tools.Helper;
using AvaloniaApplication1.Tools.ListBoxLogger;
using AvaloniaApplication1.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using Ursa.Controls;

namespace AvaloniaApplication1.Views;

public partial class StylePreviewPage : UserControl
{
    public StylePreviewPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        // 获取 LoggerName
        string loggerName = ((StylePreviewPageViewModel)DataContext).LoggerName;
        WeakReferenceMessenger.Default.Register<string, string>(this, loggerName, MessageHandler);
        
    }
    
    private void MessageHandler(object recipient, string message)
    {
        var strArray = message.Split("|");
        switch (strArray[0])
        {
            // 处理来自 ListBoxLogger.cs.LogListBox_DoubleTapped() 的 Message
            case "ListBoxLogger_StylePreviewPage":
                App.WindowToastManager?.Show(
                    new Toast(strArray[1]),
                    type: NotificationType.Information,
                    showIcon: true,
                    showClose: true,
                    onClose: OnToastClose,
                    classes: ["Light"]);
                break;
        }
    }

    #region Click

    private void Button_ToggleSplitView1_OnClick(object? sender, RoutedEventArgs e)
    {
        SplitView_Demo1.IsPaneOpen = !SplitView_Demo1.IsPaneOpen;
    }
    private void Button_ToggleSplitView2_OnClick(object? sender, RoutedEventArgs e)
    {
        SplitView_Demo2.IsPaneOpen = !SplitView_Demo2.IsPaneOpen;
    }


    #endregion Click


    #region Method
    
    private void OnToastClose(MessageCloseReason closeReason)
    {
        var reason = closeReason;
    }
    

    #endregion Method
    
}