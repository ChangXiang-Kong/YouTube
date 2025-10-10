using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AvaloniaUIRealWorld.Controls;

public class IconButton : Button
{

    /// <summary>
    /// IconText StyledProperty definition
    /// </summary>
    public static readonly StyledProperty<string> IconTextProperty =
        AvaloniaProperty.Register<IconButton, string>(nameof(IconText));

    /// <summary>
    /// Gets or sets the IconText property. This StyledProperty
    /// indicates ....
    /// </summary>
    public string IconText
    {
        get => this.GetValue(IconTextProperty);
        set => SetValue(IconTextProperty, value);
    }


}