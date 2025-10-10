using Avalonia.Controls;
using AvaloniaUIRealWorld.ViewModels;

namespace AvaloniaUIRealWorld.Views
{
    public partial class MainView : Window
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
}