using Avalonia.Controls;
using Avalonia.Interactivity;
using BatchProcess3.ViewModels;

namespace BatchProcess3.Views.MainMenus;

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
}