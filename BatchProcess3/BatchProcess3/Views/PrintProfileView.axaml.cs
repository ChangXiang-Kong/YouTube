using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using BatchProcess3.ViewModels;

namespace BatchProcess3.Views;

public partial class PrintProfileView : UserControl
{
    public PrintProfileView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        KeyDown += OnKeyDown;
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        // TODO: 存在问题，第一次打开时能正常接收事件，但之后再打开无法接收事件
        if (e.Key == Key.Escape)
        {
            ((ConfirmDialogViewModel)DataContext).CancelCommand.Execute(null);
        }
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        // Ensure the focus is set after the XAML has been fully loaded
        Dispatcher.InvokeAsync(() =>
        {
            if (TextBox_Name != null)
            {
                // TODO: 存在问题，第一次打开时能正常获取焦点，但之后再打开无法获取焦点
                TextBox_Name.SelectAll();
                TextBox_Name.Focus();
                
                // 如果焦点仍然没有设置成功，可以尝试强制刷新布局
                this.UpdateLayout();
            }
        }, DispatcherPriority.Loaded);
    }
}