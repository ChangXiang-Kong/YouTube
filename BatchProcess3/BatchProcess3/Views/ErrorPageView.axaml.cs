using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace BatchProcess3.Views;

public partial class ErrorPageView : UserControl
{
    public ErrorPageView(string title = "", string message = "", IImage? image = null)
    {
        InitializeComponent();

        if (!string.IsNullOrWhiteSpace(title))
            TextBlock_Title.Text = title;
        if (!string.IsNullOrWhiteSpace(message))
            TextBlock_Title.Text = message;
        if (image is not null)
            Image_SomeImage.Source = image;
    }
}