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

    private readonly DynamicResourceExtension _dynamicResourceExtensionSemiIconChevronLeft = new("SemiIconChevronLeft");
    private readonly DynamicResourceExtension _dynamicResourceExtensionSemiIconChevronRight = new("SemiIconChevronRight");
    private readonly DynamicResourceExtension _dynamicResourceExtensionSemiIconChevronUp = new("SemiIconChevronUp");
    private readonly DynamicResourceExtension _dynamicResourceExtensionSemiIconChevronDown = new("SemiIconChevronDown");
    private readonly Thickness _bottomBarToggleTrueMargin = new(20, 0, 0, -10);
    private readonly Thickness _bottomBarToggleFalseMargin = new(20, 0, 0, -41);
    private readonly StreamGeometry _geometryAsk = StreamGeometry.Parse("M512 0 30.11843 240.941297l0 542.117406 481.88157 240.941297 481.88157-240.941297L993.88157 240.941297 512 0zM575.776472 768.799969 460.188012 768.799969 460.188012 656.222073l115.588459 0L575.776472 768.799969zM623.335603 509.329685c-52.375829 36.723353-59.600363 55.988096-59.600363 84.885211l0 19.866447L468.616977 614.081343l0-26.489278c0-45.754021 13.846342-80.67124 61.406497-116.791866 46.957428-36.723353 57.79423-62.0082 57.79423-84.282484 0-25.284848-21.67258-54.181962-55.386393-54.181962-42.743457 0-70.436142 26.489278-82.477374 85.486914l-105.956088-21.67258c24.683144-111.976192 82.477374-157.127486 205.289345-157.127486 98.12985 0 157.72919 63.212631 157.72919 131.842639C707.017407 423.240044 688.956071 461.76953 623.335603 509.329685z");

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
                SplitView_SelectSystem.IsPaneOpen = !SplitView_SelectSystem.IsPaneOpen;
                if (SplitView_SelectProject.IsPaneOpen)
                    SplitView_SelectProject.IsPaneOpen = !SplitView_SelectProject.IsPaneOpen;
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
        DockPanel.SetDock(closeButton, Avalonia.Controls.Dock.Right);
        contentDockPanel.Children.Add(closeButton);

        // 创建显示通知内容的 StackPanel
        var contentStackPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(5, 0),
            Spacing = 5
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
            // Classes = { "Primary" },
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

    private void Button_TestButtonClick(object? sender, RoutedEventArgs e)
    {
        
    }

    private void Button_NewSystemNotificationClick(object? sender, RoutedEventArgs e)
    {
        AddNewSystemNotification(new SystemNotificationModel
            { Message = "New Test Notification", YesAction = () => { }, NoAction = () => { } });
    }

    private void Button_SelectProjectClick(object? sender, RoutedEventArgs e)
    {
        MessageHandler(null, "ShowSplitView_SelectProject");
    }

    private void Button_SelectSystemClick(object? sender, RoutedEventArgs e)
    {
        MessageHandler(null, "ShowSplitView_SelectSystem");
    }


    private void IconButton_LeftBarToggleClick(object? sender, RoutedEventArgs e)
    {
        if (StackPanel_LeftBar.IsVisible)
        {
            StackPanel_LeftBar.IsVisible = false;
            // IconButton_LeftBarToggle.Margin = new Thickness(-10,0,0,0);
            IconButton_LeftBarToggle.Bind(IconButton.IconProperty, _dynamicResourceExtensionSemiIconChevronRight);
        }
        else
        {
            StackPanel_LeftBar.IsVisible = true;
            // IconButton_LeftBarToggle.Margin = new Thickness(-10,0,0,0);
            IconButton_LeftBarToggle.Bind(IconButton.IconProperty, _dynamicResourceExtensionSemiIconChevronLeft);
        }
    }

    private void IconButton_RightBarToggleClick(object? sender, RoutedEventArgs e)
    {
        if (StackPanel_RightBar.IsVisible)
        {
            StackPanel_RightBar.IsVisible = false;
            // IconButton_RightBarToggle.Margin = new Thickness(0,0,-10,0);
            IconButton_RightBarToggle.Bind(IconButton.IconProperty, _dynamicResourceExtensionSemiIconChevronLeft);
        }
        else
        {
            StackPanel_RightBar.IsVisible = true;
            // IconButton_RightBarToggle.Margin = new Thickness(0,0,-10,0);
            IconButton_RightBarToggle.Bind(IconButton.IconProperty, _dynamicResourceExtensionSemiIconChevronRight);
        }
    }

    private void IconButton_BottomBarToggleClick(object? sender, RoutedEventArgs e)
    {
        if (Grid_BottomBar.IsVisible)
        {
            Grid_BottomBar.IsVisible = false;
            IconButton_BottomBarToggle.Margin = _bottomBarToggleFalseMargin;
            IconButton_BottomBarToggle.Bind(IconButton.IconProperty, _dynamicResourceExtensionSemiIconChevronUp);
        }
        else
        {
            Grid_BottomBar.IsVisible = true;
            IconButton_BottomBarToggle.Margin = _bottomBarToggleTrueMargin;
            IconButton_BottomBarToggle.Bind(IconButton.IconProperty, _dynamicResourceExtensionSemiIconChevronDown);
        }
    }

    #endregion Button Click
}

public class SystemNotificationModel
{
    public string? Message { get; init; }
    public Action? YesAction { get; init; }
    public Action? NoAction { get; init; }
}