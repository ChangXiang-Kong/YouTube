using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using BatchProcess3.ViewModels.MainMenus;

namespace BatchProcess3.Views.MainMenus;

public partial class HomePageView : UserControl
{
    public HomePageView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }
    
    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        ((HomePageViewModel)DataContext).InitializeCommand.Execute(null);
    }

    private void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        
    }
    
    
}