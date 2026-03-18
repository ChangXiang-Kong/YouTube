using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace BatchProcess3.Controls;

public class IconButton : Button
{
    // 提示：输入 styleproperty 后按下 Tab 键会自动生成以下内容 
    
    #region IconText

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

    #endregion IconText

}