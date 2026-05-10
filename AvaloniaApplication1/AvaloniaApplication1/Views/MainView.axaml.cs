using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.Messaging;
using Ursa.Controls;

namespace AvaloniaApplication1.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        WeakReferenceMessenger.Default.Register<string, string>(this, "JumpTo", MessageHandler);
        WeakReferenceMessenger.Default.Register<string, string>(this, "ShowSplitView_SelectProject", MessageHandler);
        WeakReferenceMessenger.Default.Register<string, string>(this, "ShowSplitView_SelectSystem", MessageHandler);
        WeakReferenceMessenger.Default.Register<SystemNotificationModel, string>(this, "NewSystemNotification",
            NewSystemNotificationHandler);
    }

    #region Button Click

    private void Button_TestButtonClick(object? sender, RoutedEventArgs e)
    {
        AddNewSystemNotification(new SystemNotificationModel
            { Message = "New Test Notification", YesAction = () => { }, NoAction = () => { } });
    }

    #endregion Button Click

    #region WeakReferenceMessenger Handler

    private void MessageHandler(object recipient, string message)
    {
        switch (message)
        {
            case "JumpTo":
                // foreach (var item in tab.ItemsView)
                // {
                //     if (item is TabItem tabItem && tabItem.Header is not null && tabItem.Header.Equals(message))
                //     {
                //         tab.SelectedItem = tabItem;
                //         break;
                //     }
                // }
                break;
            case "ShowSplitView_SelectProject":
                SplitView_SelectProject.IsPaneOpen = !SplitView_SelectProject.IsPaneOpen;
                if (SplitView_SelectSystem.IsPaneOpen)
                    SplitView_SelectSystem.IsPaneOpen = !SplitView_SelectSystem.IsPaneOpen;
                break;
            case "ShowSplitView_SelectSystem":
                if (SplitView_SelectProject.IsPaneOpen)
                    SplitView_SelectProject.IsPaneOpen = !SplitView_SelectProject.IsPaneOpen;
                SplitView_SelectSystem.IsPaneOpen = !SplitView_SelectSystem.IsPaneOpen;
                break;
        }
    }

    private void NewSystemNotificationHandler(object recipient, SystemNotificationModel model)
    {
        AddNewSystemNotification(model);
    }

    /// <summary>
    /// 添加新的系统通知
    /// </summary>
    /// <example>AddNewSystemNotification(new SystemNotificationModel{ Message = "New Test Notification", YesAction = () => { }, NoAction = () => { }});</example>
    private void AddNewSystemNotification(SystemNotificationModel model)
    {
        /* 获取Resource SemiGreen
        if (Application.Current is null)
            throw new InvalidOperationException("Application.Current cannot be null");
        // 获取Resource SemiGreen
        if (!Application.Current.TryGetResource("SemiGreen2", Application.Current.RequestedThemeVariant,
                out var resource)
            || resource is not SolidColorBrush colorBrush)
        {
            // 如果资源未找到或不是SolidColorBrush类型，使用默认颜色
            colorBrush = new SolidColorBrush(Colors.LightGray);
        }*/

        var contentDockPanel = new DockPanel();
        // ✅ 关键：绑定 DynamicResource，而不是赋值颜色。等于 XAML 里的：Background="{DynamicResource SemiGreen2}"
        contentDockPanel.Bind(BackgroundProperty, new DynamicResourceExtension("SemiGreen2"));
        // if (StackPanel_NotificationArea.Children.Count >= 1)
        contentDockPanel.Margin = new Thickness(0, 0, 0, 1);

        // 创建关闭按钮
        var closeButton = new IconButton
        {
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Stretch,
            // 方式一：直接赋值
            // Icon = StreamGeometry.Parse("M17.66 19.78a1.5 1.5 0 0 0 2.12-2.12L14.12 12l5.66-5.66a1.5 1.5 0 0 0-2.12-2.12L12 9.88 6.34 4.22a1.5 1.5 0 1 0-2.12 2.12L9.88 12l-5.66 5.66a1.5 1.5 0 0 0 2.12 2.12L12 14.12l5.66 5.66Z"),
            // Theme = Application.Current.FindResource(Application.Current.RequestedThemeVariant,"BorderlessIconButton") as ControlTheme
            Classes = { "Tertiary" }
        };
        // 方式二：动态绑定
        /* 说明：
            ThemeProperty 是 Avalonia 内置控件的属性，所以能直接找到；因为 ThemeProperty 来自 Control.ThemeProperty，Avalonia 全局可见。
            IconProperty 是 IconButton 自己的附加属性，必须写完整类型名称才能访问。因为 IconProperty 不属于 Control，而是属于 IconButton！
            终极规则（以后永远不会错）：
                第三方控件（IconButton / TabItem 等）的属性：控件类型.属性名
                    例如：IconButton.IconProperty
                Avalonia 内置控件（Control / Visual 等）的属性：
                    直接写 ThemeProperty / BackgroundProperty / WidthProperty
         */
        closeButton.Bind(IconButton.IconProperty, new DynamicResourceExtension("SemiIconClose"));
        closeButton.Bind(ThemeProperty, new DynamicResourceExtension("BorderlessIconButton"));
        // 为按钮添加点击事件处理程序
        closeButton.Click += (sender, args) =>
        {
            // TODO: 停止任务
            model.NoAction?.Invoke();
            StackPanel_NotificationArea.Children.Remove(contentDockPanel);
        };

        // 将关闭按钮添加到新的 DockPanel 中
        DockPanel.SetDock(closeButton, Dock.Right);
        contentDockPanel.Children.Add(closeButton);

        // 创建显示通知内容的 StackPanel
        var contentStackPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(5, 0),
            Spacing = 10
        };

        // 添加一个 TextBlock 来显示通知文本
        var notificationTextBlock = new TextBlock
        {
            Text = model.Message,
            VerticalAlignment = VerticalAlignment.Center,
            TextWrapping = TextWrapping.Wrap
        };

        var yesButton = new Button
        {
            Content = "Yes",
            MinHeight = 20,
            Height = 24,
            FontWeight = FontWeight.Normal,
            Classes = { "Primary" },
        };
        yesButton.Bind(ThemeProperty, new DynamicResourceExtension("SolidButton"));
        // 为按钮添加点击事件处理程序
        yesButton.Click += (sender, args) =>
        {
            // TODO: 启动任务
            model.YesAction?.Invoke();
            // 任务完成后
            StackPanel_NotificationArea.Children.Remove(contentDockPanel);
        };
        var noButton = new Button
        {
            Content = "No",
            MinHeight = 20,
            Height = 24,
            FontWeight = FontWeight.Normal,
            Classes = { "Warning" },
        };
        noButton.Bind(ThemeProperty, new DynamicResourceExtension("SolidButton"));
        // 为按钮添加点击事件处理程序
        noButton.Click += (sender, args) =>
        {
            // TODO: 停止任务
            model.NoAction?.Invoke();
            StackPanel_NotificationArea.Children.Remove(contentDockPanel);
        };

        // 将内容添加到主 DockPanel 中
        contentStackPanel.Children.Add(notificationTextBlock);
        contentStackPanel.Children.Add(yesButton);
        contentStackPanel.Children.Add(noButton);
        contentDockPanel.Children.Add(contentStackPanel);
        StackPanel_NotificationArea.Children.Add(contentDockPanel);
    }

    #endregion WeakReferenceMessenger Handler

    #region Button Click

    private void Button_SelectProjectClick(object? sender, RoutedEventArgs e)
    {
        MessageHandler(null, "ShowSplitView_SelectProject");
    }
    private void Button_SelectSystemClick(object? sender, RoutedEventArgs e)
    {
        MessageHandler(null, "ShowSplitView_SelectSystem");
    }
    
    

    #endregion Button Click

    private void IconButton_LeftBarToggleClick(object? sender, RoutedEventArgs e)
    {
        if (StackPanel_LeftBar.IsVisible)
        {
            StackPanel_LeftBar.IsVisible = false;
            IconButton_LeftBarToggle.Margin = new Thickness(-13, -50, 0, 0);
            IconButton_LeftBarToggle.Bind(IconButton.IconProperty, new DynamicResourceExtension("SemiIconChevronRight"));
        }
        else
        {
            StackPanel_LeftBar.IsVisible = true;
            IconButton_LeftBarToggle.Margin = new Thickness(18, -50, 0, 0);
            IconButton_LeftBarToggle.Bind(IconButton.IconProperty, new DynamicResourceExtension("SemiIconChevronLeft"));
        }
    }
    private void IconButton_RightBarToggleClick(object? sender, RoutedEventArgs e)
    {
        if (StackPanel_RightBar.IsVisible)
        {
            StackPanel_RightBar.IsVisible = false;
            IconButton_RightBarToggle.Margin = new Thickness(0,-50,-13,0);
            IconButton_RightBarToggle.Bind(IconButton.IconProperty, new DynamicResourceExtension("SemiIconChevronLeft"));
        }
        else
        {
            StackPanel_RightBar.IsVisible = true;
            IconButton_RightBarToggle.Margin = new Thickness(0,-50,18,0);
            IconButton_RightBarToggle.Bind(IconButton.IconProperty, new DynamicResourceExtension("SemiIconChevronRight"));
        }
    }
    private void IconButton_BottomBarToggleClick(object? sender, RoutedEventArgs e)
    {
        if (Grid_BottomBar.IsVisible)
        {
            Grid_BottomBar.IsVisible = false;
            IconButton_BottomBarToggle.Margin = new Thickness(55,18,0,0);
            IconButton_BottomBarToggle.Bind(IconButton.IconProperty, new DynamicResourceExtension("SemiIconChevronUp"));
        }
        else
        {
            Grid_BottomBar.IsVisible = true;
            IconButton_BottomBarToggle.Margin = new Thickness(55,-40,0,0);
            IconButton_BottomBarToggle.Bind(IconButton.IconProperty, new DynamicResourceExtension("SemiIconChevronDown"));
        }
    }
}

public class SystemNotificationModel
{
    public string? Message { get; init; }
    public Action? YesAction { get; init; }
    public Action? NoAction { get; init; }
}