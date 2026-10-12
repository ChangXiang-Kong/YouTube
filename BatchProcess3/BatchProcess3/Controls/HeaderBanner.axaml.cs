using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using BatchProcess3.Data;

namespace BatchProcess3.Controls;

public partial class HeaderBanner : UserControl
{
    public HeaderBanner()
    {
        InitializeComponent();
        
        // Detect design time
        if (Avalonia.Controls.Design.IsDesignMode)
        {
            this.SetDesignTimeBackground("PrimaryBackground");
            this.SetDesignTimeForeground("PrimaryForeground");
            
            Header = "This is Header";
            SubHeaderA = "Sub Header A";
            SubHeaderB = "Sub Header B";
        }
        
        DataContext = this;
    }

    public static readonly StyledProperty<IImage?> BackgroundImageProperty = AvaloniaProperty.Register<HeaderBanner, IImage?>(nameof(BackgroundImage));

    /// <summary>
    /// 
    /// </summary>
    public IImage? BackgroundImage
    {
        get => GetValue(BackgroundImageProperty);
        set => SetValue(BackgroundImageProperty, value);
    }

    public static readonly StyledProperty<string?> HeaderProperty = AvaloniaProperty.Register<HeaderBanner, string?>(nameof(Header));

    /// <summary>
    /// 
    /// </summary>
    public string? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public static readonly StyledProperty<string?> SubHeaderAProperty = AvaloniaProperty.Register<HeaderBanner, string?>(nameof(SubHeaderA));

    /// <summary>
    /// 
    /// </summary>
    public string? SubHeaderA
    {
        get => GetValue(SubHeaderAProperty);
        set => SetValue(SubHeaderAProperty, value);
    }

    public static readonly StyledProperty<string?> SubHeaderBProperty = AvaloniaProperty.Register<HeaderBanner, string?>(nameof(SubHeaderB));

    /// <summary>
    /// 
    /// </summary>
    public string? SubHeaderB
    {
        get => GetValue(SubHeaderBProperty);
        set => SetValue(SubHeaderBProperty, value);
    }
    
}