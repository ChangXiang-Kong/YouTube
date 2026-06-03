using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaApplication1.Data;
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

    private void MenuItem_SelectProject_OnClick(object? sender, RoutedEventArgs e)
    {
        WeakReferenceMessenger.Default.Send(MessageToken.ShowSplitView_SelectProject, MessageToken.ShowSplitView_SelectProject);
    }

    private void MenuItem_SelectSystem_OnClick(object? sender, RoutedEventArgs e)
    {
        WeakReferenceMessenger.Default.Send(MessageToken.ShowSplitView_SelectSystem, MessageToken.ShowSplitView_SelectSystem);
    }
}