using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaApplication1.Views;

public partial class TitleBarLeftContent : UserControl
{
    public TitleBarLeftContent()
    {
        InitializeComponent();
    }

    private async void OpenRepository(object? sender, RoutedEventArgs e)
    {
        var top = TopLevel.GetTopLevel(this);
        if (top is null) return;
        var launcher = top.Launcher;
        await launcher.LaunchUriAsync(new Uri("https://github.com/irihitech/Ursa.Avalonia"));
    }

    private void MenuItem_SelectProjectClick(object? sender, RoutedEventArgs e)
    {
        WeakReferenceMessenger.Default.Send("ShowSplitView_SelectProject", "ShowSplitView_SelectProject");
    }

    private void MenuItem_SelectSystemClick(object? sender, RoutedEventArgs e)
    {
        WeakReferenceMessenger.Default.Send("ShowSplitView_SelectSystem", "ShowSplitView_SelectSystem");
    }
}