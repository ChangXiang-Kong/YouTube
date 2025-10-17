using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaUIRealWorld.ViewModels;

namespace AvaloniaUIRealWorld.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }
    
    private void Image_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        if (e.ClickCount != 2)
            return;

        (DataContext as MainViewModel)?.SideMenuResizeCommand?.Execute(null);
    }
}