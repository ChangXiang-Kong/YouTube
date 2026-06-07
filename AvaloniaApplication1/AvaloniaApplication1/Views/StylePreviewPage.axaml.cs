using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using AvaloniaApplication1.Controls;
using AvaloniaApplication1.Data;
using AvaloniaApplication1.Tools.ListBoxLog;
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

    /// <summary>
    /// 离开可视树时的逻辑
    /// </summary>
    /// <param name="e"></param>
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        WeakReferenceMessenger.Default.Register<string, string>(this, MessageToken.ListBoxLogger_StylePreviewPage, MessageHandler);
        
    }
    
    private void MessageHandler(object recipient, string message)
    {
        var strArray = message.Split("|");
        switch (strArray[0])
        {
            // 处理来自 ListBoxLogger.cs.LogListBox_DoubleTapped() 的 Message
            case MessageToken.ListBoxLogger_StylePreviewPage:
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

    private void Button_Test_OnClick(object? sender, RoutedEventArgs e)
    {
        // var a = ListBox_Log.Items;
        // var b = "Success".Substring(0, 3);  // "Suc"
        // var c = default(LogType);   // Total
        var d = ListBox_MainLogger.Classes;
        var ee = ListBox_MainLogger;
        ee.Margin = new Thickness(50, 0, 0, 0);
        
        return;
    }

    #region Event
    private void Button_ToggleSplitView1_OnClick(object? sender, RoutedEventArgs e)
    {
        SplitView_Demo1.IsPaneOpen = !SplitView_Demo1.IsPaneOpen;
    }
    private void Button_ToggleSplitView2_OnClick(object? sender, RoutedEventArgs e)
    {
        SplitView_Demo2.IsPaneOpen = !SplitView_Demo2.IsPaneOpen;
    }
    
    
    #region ListBoxLogger
    private void Button_RegisterLogListBox_OnClick(object? sender, RoutedEventArgs e)
    {
        ListBoxLoggerManager.RegisterListBoxLogger(MessageToken.ListBoxLogger_StylePreviewPage, ListBox_MainLogger, ListBox_MainLogger_bak); 
        ((StylePreviewPageViewModel)DataContext).ListBoxLogger = ListBoxLoggerManager.GetLoggerByName(MessageToken.ListBoxLogger_StylePreviewPage);
        
        App.WindowToastManager?.Show(
            new Toast($"注册成功 {MessageToken.ListBoxLogger_StylePreviewPage}"),
            type: NotificationType.Success,
            showIcon: true,
            showClose: true,
            onClose: OnToastClose,
            classes: ["Light"]);
    }
    #endregion ListBoxLogger

    #region SearchBar | TextBox
    private void SearchBar_OnSearchStarted(object? sender, FunctionEventArgs<string> e)
    {
        string keyword = e.Info;
        
        App.WindowToastManager?.Show(
            new Toast($"[Event SearchBar_OnSearchStarted] 参数：{e.Info}"),
            type: NotificationType.Information,
            showIcon: true,
            showClose: true,
            onClose: OnToastClose,
            classes: ["Light"]);
    }
    private void SearchBar_OnCleared(object? sender, RoutedEventArgs e)
    {
        App.WindowToastManager?.Show(
            new Toast($"[Event SearchBar_OnCleared] SearchBar content cleared"),
            type: NotificationType.Information,
            showIcon: true,
            showClose: true,
            onClose: OnToastClose,
            classes: ["Light"]);
    }
    #endregion SearchBar | TextBox


    #endregion Event
    
    



    
    #region Method
    private void OnToastClose(MessageCloseReason closeReason)
    {
        var reason = closeReason;
    }
    

    #endregion Method

}