using System;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using BatchProcess3.Data;
using BatchProcess3.Tools.Extensions;
using BatchProcess3.Tools.Helper;
using BatchProcess3.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BatchProcess3.ViewModels;

// 参考视频：https://www.youtube.com/watch?v=suipJSELnrk&list=PLrW43fNmjaQWwIdZxjZrx5FSXcNzaucOO&index=29
// ttf图标来源：https://phosphoricons.com/
public partial class ConfirmDialogViewModel : DialogViewModel
{
    public ConfirmDialogViewModel()
    {
        if (Avalonia.Controls.Design.IsDesignMode)
        {
            // Design-time only
            MinWidth = _desktopMinWidth;
            MinHeight = _desktopMinHeight;
            MaxWidth = _desktopMaxWidth;
            MaxHeight = _desktopMaxHeight;
        }
        else
        {
            switch (Application.Current?.ApplicationLifetime)
            {
                case IClassicDesktopStyleApplicationLifetime desktop:
                    MinWidth = _desktopMinWidth;
                    MinHeight = _desktopMinHeight;
                    MaxWidth = _desktopMaxWidth;
                    MaxHeight = _desktopMaxHeight;
                    break;
                case ISingleViewApplicationLifetime singleViewPlatform:
                    MinWidth = _mobileMinWidth;
                    MaxWidth = _mobileMaxWidth;
                    MinHeight = _mobileMinHeight;
                    MaxHeight = _mobileMaxHeight;
                    break;
            }
        }
        _iconGeometry = _geometryAsk;   // 默认图标
    }

    private readonly StreamGeometry _geometryAsk = StreamGeometry.Parse("M512 0 30.11843 240.941297l0 542.117406 481.88157 240.941297 481.88157-240.941297L993.88157 240.941297 512 0zM575.776472 768.799969 460.188012 768.799969 460.188012 656.222073l115.588459 0L575.776472 768.799969zM623.335603 509.329685c-52.375829 36.723353-59.600363 55.988096-59.600363 84.885211l0 19.866447L468.616977 614.081343l0-26.489278c0-45.754021 13.846342-80.67124 61.406497-116.791866 46.957428-36.723353 57.79423-62.0082 57.79423-84.282484 0-25.284848-21.67258-54.181962-55.386393-54.181962-42.743457 0-70.436142 26.489278-82.477374 85.486914l-105.956088-21.67258c24.683144-111.976192 82.477374-157.127486 205.289345-157.127486 98.12985 0 157.72919 63.212631 157.72919 131.842639C707.017407 423.240044 688.956071 461.76953 623.335603 509.329685z");
    private readonly StreamGeometry _geometryInfo = StreamGeometry.Parse("M505.6512 39.0144c-261.2224 3.4816-470.1184 218.112-466.6368 479.4368 3.4816 261.12 218.112 470.1184 479.3344 466.6368 261.2224-3.4816 470.1184-218.112 466.7392-479.3344C981.504 244.4288 766.8736 35.5328 505.6512 39.0144zM558.08 196.608c48.128 0 62.2592 27.9552 62.2592 59.8016 0 39.8336-31.9488 76.6976-86.3232 76.6976-45.568 0-67.1744-22.9376-65.9456-60.8256C468.0704 240.4352 494.7968 196.608 558.08 196.608zM434.7904 807.6288c-32.8704 0-56.9344-19.968-33.8944-107.6224l37.6832-155.5456c6.5536-24.8832 7.68-34.9184 0-34.9184-9.8304 0-52.5312 17.2032-77.7216 34.2016l-16.384-26.9312c79.9744-66.7648 171.8272-105.8816 211.2512-105.8816 32.8704 0 38.2976 38.912 21.9136 98.6112l-43.2128 163.5328c-7.68 28.8768-4.4032 38.912 3.2768 38.912 9.9328 0 42.1888-11.9808 73.9328-36.9664l18.6368 24.8832C552.5504 777.728 467.6608 807.6288 434.7904 807.6288z");
    private readonly StreamGeometry _geometrySuccess = StreamGeometry.Parse("M512.66048 64.64c-247.424 0-448 200.57728-448 448s200.576 448 448 448 448-200.57728 448-448c0-247.424-200.57728-448-448-448z m250.71232 334.86336L480.98176 681.89312c-15.49568 15.49696-40.61952 15.49696-56.11648 0l-162.9184-162.9184c-15.49568-15.49568-15.49568-40.61824 0-56.1152s40.61952-15.49568 56.11648 0l134.85952 134.85952L707.25504 343.3856c15.49568-15.49568 40.61952-15.49568 56.11648 0s15.49696 40.6208 0.00128 56.11776z");
    private readonly StreamGeometry _geometryWarning = StreamGeometry.Parse("M943.644188 827.215696l-351.176649-608.204749c-42.945473-74.36249-113.147387-74.36249-156.092861 0l-351.176649 608.204749c-42.946498 74.431167-7.811716 135.14955 78.012605 135.14955l702.420949 0C951.455904 962.36422 986.555836 901.645838 943.644188 827.215696zM466.187532 391.579035c12.621133-13.644108 28.66175-20.466675 48.233578-20.466675 19.580028 0 35.612444 6.75389 48.241778 20.194018 12.544256 13.473954 18.820484 30.325365 18.820484 50.587035 0 17.430551-26.19759 145.621205-34.929778 238.882082l-63.105666 0c-7.666162-93.259852-36.090106-221.450507-36.090106-238.882082C447.358847 421.938226 453.643275 405.155491 466.187532 391.579035zM561.76804 835.026386c-13.268949 12.928641-29.062535 19.375023-47.345906 19.375023-18.275171 0-34.076957-6.447407-47.346931-19.375023-13.235123-12.89379-19.818859-28.517221-19.818859-46.869269 0-18.249546 6.583736-34.043131 19.818859-47.278254 13.268949-13.235123 29.07176-19.852685 47.346931-19.852685 18.283371 0 34.076957 6.617562 47.345906 19.852685 13.235123 13.235123 19.827059 29.028709 19.827059 47.278254C581.595099 806.51019 575.003163 822.132597 561.76804 835.026386z");
    private readonly StreamGeometry _geometryError = StreamGeometry.Parse("M495.469714 0C224.621714 0 0 224.621714 0 495.469714c0 270.884571 224.621714 495.506286 495.469714 495.506286 270.884571 0 495.506286-224.621714 495.506286-495.506286C990.976 224.621714 766.354286 0 495.469714 0z m211.419429 634.221714c19.821714 19.821714 19.821714 46.226286 0 66.048s-46.226286 19.821714-66.048 0l-138.752-138.715428-145.334857 145.334857a51.858286 51.858286 0 0 1-72.667429 0 51.858286 51.858286 0 0 1 0-72.667429l145.334857-145.334857-138.752-138.752c-19.821714-19.821714-19.821714-46.226286 0-66.048s46.262857-19.821714 66.084572 0l138.715428 138.715429 145.371429-145.334857a51.858286 51.858286 0 0 1 72.667428 0 51.858286 51.858286 0 0 1 0 72.667428l-145.371428 145.334857 138.752 138.752z");
    private readonly StreamGeometry _geometryFatal = StreamGeometry.Parse("M716.8 375.466667l34.133333 34.133333c17.066667 17.066667 42.666667 17.066667 59.733334 0 17.066667-17.066667 17.066667-42.666667 0-59.733333l-34.133334-34.133334 34.133334-34.133333c17.066667-17.066667 17.066667-42.666667 0-59.733333-17.066667-17.066667-42.666667-17.066667-59.733334 0l-34.133333 34.133333-34.133333-34.133333c-17.066667-17.066667-42.666667-17.066667-59.733334 0-17.066667 17.066667-17.066667 42.666667 0 59.733333l34.133334 34.133333-34.133334 34.133334c-17.066667 17.066667-17.066667 42.666667 0 59.733333 17.066667 17.066667 42.666667 17.066667 59.733334 0l34.133333-34.133333z m-426.666667 0l34.133334 34.133333c17.066667 17.066667 42.666667 17.066667 59.733333 0 17.066667-17.066667 17.066667-42.666667 0-59.733333l-34.133333-34.133334 34.133333-34.133333c17.066667-17.066667 17.066667-42.666667 0-59.733333-17.066667-17.066667-42.666667-17.066667-59.733333 0l-34.133334 34.133333-34.133333-34.133333c-17.066667-17.066667-42.666667-17.066667-59.733333 0-17.066667 17.066667-17.066667 42.666667 0 59.733333l34.133333 34.133333-34.133333 34.133334c-17.066667 17.066667-17.066667 42.666667 0 59.733333 17.066667 17.066667 42.666667 17.066667 59.733333 0l34.133333-34.133333zM0 85.333333c0-51.2 42.666667-85.333333 85.333333-85.333333h853.333334c51.2 0 85.333333 42.666667 85.333333 85.333333v853.333334c0 51.2-42.666667 85.333333-85.333333 85.333333H85.333333c-51.2 0-85.333333-42.666667-85.333333-85.333333V85.333333z m512 469.333334c-136.533333 0-230.4 68.266667-290.133333 196.266666-8.533333 17.066667 0 42.666667 17.066666 59.733334s42.666667 0 59.733334-17.066667c51.2-93.866667 110.933333-145.066667 221.866666-145.066667 102.4 0 170.666667 51.2 221.866667 145.066667 8.533333 17.066667 34.133333 25.6 59.733333 17.066667 17.066667-8.533333 25.6-34.133333 17.066667-59.733334-76.8-128-170.666667-196.266667-307.2-196.266666z");

    private readonly double _desktopMinWidth = 350;
    private readonly double _desktopMinWidth1 = double.NaN;
    private readonly double _desktopMinHeight = 220;
    private readonly double _desktopMaxWidth = 1200;
    private readonly double _desktopMaxHeight = 700;
    private readonly double _mobileMinWidth = double.NaN;
    private readonly double _mobileMinHeight = double.NaN;
    private readonly double _mobileMaxWidth = double.NaN;
    private readonly double _mobileMaxHeight = double.NaN;

    [property: JsonIgnore] [ObservableProperty] private double _minWidth;
    [property: JsonIgnore] [ObservableProperty] private double _minHeight;
    [property: JsonIgnore] [ObservableProperty] private double _maxWidth;
    [property: JsonIgnore] [ObservableProperty] private double _maxHeight;
    [property: JsonIgnore] [ObservableProperty] private double _dialogWidth = double.NaN;
    [property: JsonIgnore] [ObservableProperty] private double _dialogHeight = double.NaN;

    [property: JsonIgnore] [ObservableProperty] private double _iconWidth = 40;
    [property: JsonIgnore] [ObservableProperty] private double _iconHeight = 40;
    [property: JsonIgnore] [ObservableProperty] private string _iconText = "";         // 使用ttf字体图标
    [property: JsonIgnore] [ObservableProperty] private string _iconMessage = "";
    [property: JsonIgnore] [ObservableProperty] private string _iconForeground = "DodgerBlue";
    // 参考：也可以使用 IBrush 类型代替 string 类型，这样就可以使用在 App.axaml 中定义的颜色资源，如渐变色 <LinearGradientBrush x:Key="HoverGradient" .../>
    [property: JsonIgnore] [ObservableProperty] private IBrush _iconForeground1 = Brush.Parse("Pink");
    [property: JsonIgnore] [ObservableProperty] private IBrush _iconForeground2 = ResourceHelper.FindResource<IBrush>("HoverGradient", true) ?? Brush.Parse("Red");
    /* 序列化时忽略该 IconGeometry 属性，因为无法序列化 StreamGeometry 对象。
     说明：
        在 Avalonia 12 中：StreamGeometry 无法反向获取原始路径数据
        因为StreamGeometry：
            不保存原始 SVG Path 字符串
            不公开 Figures/Segments
            内部已编译成绘制指令
        所以：
            _geometrySuccess.ToString()
        得到的是类名：
            "Avalonia.Media.StreamGeometry"
        而不是路径数据：
            M512.66048 64.64...
        
        在 ViewModelBase.cs 中调用 GetState() 进行序列化后，对象的 iconGeometry 字符串为 "iconGeometry" : { "transform" : null }，
        因路径数据丢失，导致在反序列化时无法正常还原 Geometry
     */
    [property: JsonIgnore] [ObservableProperty] private StreamGeometry _iconGeometry;  // 使用Geometry
    [property: JsonIgnore] [ObservableProperty] private string _title = "Confirm";
    [property: JsonIgnore] [ObservableProperty] private string _message = "Are you sure?";
    [property: JsonIgnore] [ObservableProperty] private string _statusText = "";
    [property: JsonIgnore] [ObservableProperty] private string _progressText = "";
    [property: JsonIgnore] [ObservableProperty] private string _confirmText = "Yes";
    [property: JsonIgnore] [ObservableProperty] private string _cancelText = "No";
    [property: JsonIgnore] [ObservableProperty] private string _applyText = "Apply";
    
    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(CancelCommand))] private bool _isBusy = false;
    [ObservableProperty] private double _progressValue = 0;
    [ObservableProperty] private bool _isConfirmed;

    public bool NotBusy => !IsBusy;
    [JsonIgnore] public Func<ConfirmDialogViewModel, Task<bool>> OnConfirm { get; set; } = (_) => Task.FromResult(true);
    /// <summary>
    /// 修改 GeometryIcon 后，自动根据该枚举值的特性 GeometryIconAttribute 设置 IconMessage、IconForeground、IconGeometry
    /// </summary>
    [JsonIgnore]
    public GeometryIcon GeometryIcon
    {
        get => field;
        set
        {
            field = value;
            var res = field.ParseGeometryIconAttribute();
            IconMessage = res.Message;
            IconForeground = res.ColorStr;
            IconGeometry = StreamGeometry.Parse(res.GeometryPath);
            // 参考使用 IconText
            // switch (field)
            // {
            //     case GeometryIcon.Ask: IconText = "\xe3e8"; IconForeground = "DodgerBlue"; IconGeometry = _geometryAsk; break;
            //     case GeometryIcon.Info: IconText = "\xe2ce"; IconForeground = "#2cb8c5"; IconGeometry = _geometryInfo; break;
            //     case GeometryIcon.Success: IconText = "\xe184"; IconForeground = "#3bb346"; IconGeometry = _geometrySuccess; break;
            //     case GeometryIcon.Warning: IconText = "\xe4e0"; IconForeground = "#fc8800"; IconGeometry = _geometryWarning; break;
            //     case GeometryIcon.Error: IconText = "\xe4f8"; IconForeground = "#f93920"; IconGeometry = _geometryError; break;
            //     case GeometryIcon.Fatal: IconText = "\xea96"; IconForeground = "#c738ff"; IconGeometry = _geometryFatal; break;
            // }
        }
    }

    [RelayCommand]
    private async Task ConfirmAsync()
    {
        if (IsBusy) 
            return;
        
        IsBusy = true;
        
        // Clear status text
        StatusText = "";
        // Set initial progress text
        ProgressText = "Processing...";

        var result = await OnConfirm(this);
        
        IsBusy = false;
        ProgressValue = 0;
        
        if (!result)
            return;
        
        IsConfirmed = true;
        Close();
    }

    [RelayCommand(CanExecute = nameof(NotBusy))]
    private async void Cancel()
    {
        IsConfirmed = false;
        Close();
    }

    [RelayCommand]
    private void Apply()
    {
        IsConfirmed = true;
    }
}