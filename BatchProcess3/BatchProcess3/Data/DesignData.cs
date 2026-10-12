using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Microsoft.Extensions.DependencyInjection;

namespace BatchProcess3.Data;

public static class DesignData
{
    // 目前不打算使用 设计时的 DI
    // 参考视频：Design Time DI in Avalonia UI - Real World Series #65
    //      https://www.youtube.com/watch?v=_fVipt-LuLs&list=PLrW43fNmjaQWwIdZxjZrx5FSXcNzaucOO&index=66


    
    #region Set Design Time Background and Foreground
    
    /*
        参考示例：
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
                public IImage? BackgroundImage
                {
                    get => GetValue(BackgroundImageProperty);
                    set => SetValue(BackgroundImageProperty, value);
                }

                public static readonly StyledProperty<string?> HeaderProperty = AvaloniaProperty.Register<HeaderBanner, string?>(nameof(Header));
                public string? Header
                {
                    get => GetValue(HeaderProperty);
                    set => SetValue(HeaderProperty, value);
                }

                public static readonly StyledProperty<string?> SubHeaderAProperty = AvaloniaProperty.Register<HeaderBanner, string?>(nameof(SubHeaderA));
                public string? SubHeaderA
                {
                    get => GetValue(SubHeaderAProperty);
                    set => SetValue(SubHeaderAProperty, value);
                }

                public static readonly StyledProperty<string?> SubHeaderBProperty = AvaloniaProperty.Register<HeaderBanner, string?>(nameof(SubHeaderB));
                public string? SubHeaderB
                {
                    get => GetValue(SubHeaderBProperty);
                    set => SetValue(SubHeaderBProperty, value);
                }
            }
     */
    
    public static bool SetDesignTimeBackground(this Window window, string resourceKey)
    {
        if (Application.Current != null 
            && Application.Current.TryFindResource(resourceKey, out var bg)
            && bg is IBrush brush)
        {
            window.Background = brush;
            return true;
        }

        return false;
    }
    public static bool SetDesignTimeForeground(this Window window, string resourceKey)
    {
        if (Application.Current != null 
            && Application.Current.TryFindResource(resourceKey, out var bg)
            && bg is IBrush brush)
        {
            window.Foreground = brush;
            return true;
        }

        return false;
    }

    public static bool SetDesignTimeBackground(this TemplatedControl control, string resourceKey)
    {
        if (Application.Current != null 
            && Application.Current.TryFindResource(resourceKey, out var bg)
            && bg is IBrush brush)
        {
            control.Background = brush;
            return true;
        }

        return false;
    }
    public static bool SetDesignTimeForeground(this TemplatedControl control, string resourceKey)
    {
        if (Application.Current != null 
            && Application.Current.TryFindResource(resourceKey, out var bg)
            && bg is IBrush brush)
        {
            control.Foreground = brush;
            return true;
        }

        return false;
    }
    
    #endregion Set Design Time Background and Foreground
    
    
}