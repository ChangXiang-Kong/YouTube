using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace AvaloniaApplication1.Views;

public partial class StylePreviewPage : UserControl
{
    public StylePreviewPage()
    {
        InitializeComponent();
    }


    private void Button_ToggleSplitView1Click(object? sender, RoutedEventArgs e)
    {
        SplitView_Demo1.IsPaneOpen = !SplitView_Demo1.IsPaneOpen;
    }
    private void Button_ToggleSplitView2Click(object? sender, RoutedEventArgs e)
    {
        SplitView_Demo2.IsPaneOpen = !SplitView_Demo2.IsPaneOpen;
    }
}