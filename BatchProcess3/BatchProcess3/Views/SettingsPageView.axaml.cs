using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using BatchProcess3.ViewModels;

namespace BatchProcess3.Views;

public partial class SettingsPageView : UserControl
{
    public SettingsPageView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        ((ViewModelBase)DataContext)?.OnViewLoaded();
    }

    private void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        
    }
}