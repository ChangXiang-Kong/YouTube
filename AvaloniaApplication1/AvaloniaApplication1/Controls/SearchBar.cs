using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace AvaloniaApplication1.Controls;

/// <summary>
/// SearchBar 自定义控件，继承TextBox，支持Command、回车搜索、实时搜索、实时搜索防抖、手动 IsEnabled 优先
/// </summary>
public class SearchBar : TextBox
{
    #region RoutedEvent
    // RoutingStrategies.Bubble：冒泡路由事件，上层容器可以捕获搜索事件
    public static readonly RoutedEvent<FunctionEventArgs<string>> SearchStartedEvent =
        RoutedEvent.Register<SearchBar, FunctionEventArgs<string>>(nameof(SearchStarted), RoutingStrategies.Bubble);
    /// <summary>
    /// 路由事件：SearchStartedEvent 向外通知搜索触发（核心对外事件）
    /// XAML 或者后台代码可以两种方式接收搜索：
    ///     事件：        SearchStarted="OnSearchHandler"
    ///     Command：    Command="{Binding SearchCmd}"
    /// </summary>
    public event EventHandler<FunctionEventArgs<string>> SearchStarted
    {
        // AddHandler/RemoveHandler是 Avalonia 路由事件标准挂载写法
        add => AddHandler(SearchStartedEvent, value);
        remove => RemoveHandler(SearchStartedEvent, value);
    }

    public static readonly RoutedEvent<RoutedEventArgs> ClearedEvent =
        RoutedEvent.Register<SearchBar, RoutedEventArgs>(nameof(Cleared), RoutingStrategies.Bubble);
    public event EventHandler<RoutedEventArgs> Cleared
    {
        add => AddHandler(ClearedEvent, value);
        remove => RemoveHandler(ClearedEvent, value);
    }
    #endregion RoutedEvent

    
    
    #region StyledProperty
    public static readonly StyledProperty<bool> IsRealTimeProperty = AvaloniaProperty.Register<SearchBar, bool>(nameof(IsRealTime));
    /// <summary>
    /// 是否实时搜索<br/>
    /// 开启时，XAML 绑定的 CommandParameter 会被 Text 覆盖（实时搜索优先使用输入文本做参数）
    /// </summary>
    public bool IsRealTime
    {
        get => GetValue(IsRealTimeProperty);
        set => SetValue(IsRealTimeProperty, value);
    }

    public static readonly StyledProperty<int> DebounceMillisecondsProperty = AvaloniaProperty.Register<SearchBar, int>(nameof(DebounceMilliseconds), defaultValue: 300);
    /// <summary>
    /// 防抖间隔（默认300ms，最小100ms）
    /// </summary>
    public int DebounceMilliseconds
    {
        get => GetValue(DebounceMillisecondsProperty);
        set => SetValue(DebounceMillisecondsProperty, Math.Max(100, value));
    }

    public static readonly StyledProperty<int> MinSearchLengthProperty = AvaloniaProperty.Register<SearchBar, int>(nameof(MinSearchLength), defaultValue: 0);
    /// <summary>
    /// 最小搜索字符长度，小于该长度不触发搜索，（默认0，不限制）<br/>
    /// MinSearchLength=0 ：不限制字符，空文本也能搜索<br/>
    /// MinSearchLength=2 ：输入a/ab -> a不搜、ab才触发搜索<br/>
    /// 当 MinSearchLength=0 时，手动删文字变成空也触发搜索
    /// </summary>
    public int MinSearchLength
    {
        get => GetValue(MinSearchLengthProperty);
        set => SetValue(MinSearchLengthProperty, Math.Max(0, value));
    }

    public static readonly StyledProperty<ICommand?> CommandProperty = AvaloniaProperty.Register<SearchBar, ICommand?>(nameof(Command));
    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public static readonly StyledProperty<object?> CommandParameterProperty = AvaloniaProperty.Register<SearchBar, object?>(nameof(CommandParameter));
    /// <summary>
    /// 提示：XAML 不需要手动绑定 CommandParameter="{Binding Text,RelativeSource=Self}"，Search()方法已自动接收 Text 为命令参数
    /// </summary>
    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public static readonly StyledProperty<InputElement?> CommandTargetProperty = AvaloniaProperty.Register<SearchBar, InputElement?>(nameof(CommandTarget));
    /// <summary>
    /// 备用命令目标（Avalonia 很少用，兼容设计）
    /// </summary>
    public InputElement? CommandTarget
    {
        get => GetValue(CommandTargetProperty);
        set => SetValue(CommandTargetProperty, value);
    }

    public static readonly StyledProperty<ICommand?> ClearedCommandProperty = AvaloniaProperty.Register<SearchBar, ICommand?>(nameof(ClearedCommand));
    public ICommand? ClearedCommand
    {
        get => GetValue(ClearedCommandProperty);
        set => SetValue(ClearedCommandProperty, value);
    }
    
    public static readonly StyledProperty<IEnumerable<FilterOption>?> FilterOptionsProperty = AvaloniaProperty.Register<SearchBar, IEnumerable<FilterOption>?>(nameof(FilterOptions), defaultValue: null);
    /// <summary>
    /// 要显示的过滤项集合
    /// </summary>
    public IEnumerable<FilterOption>? FilterOptions
    {
        get => GetValue(FilterOptionsProperty);
        set => SetValue(FilterOptionsProperty, value);
    }

    public static readonly StyledProperty<ICommand?> FilterConfirmedCommandProperty = AvaloniaProperty.Register<SearchBar, ICommand?>(nameof(FilterConfirmedCommand));
    public ICommand? FilterConfirmedCommand
    {
        get => GetValue(FilterConfirmedCommandProperty);
        set => SetValue(FilterConfirmedCommandProperty, value);
    }
    
    #endregion StyledProperty

    
    
    /// <summary>
    /// 类静态构造，只执行一次
    ///     全局监听Command属性变更，新旧命令切换时：解绑旧CanExecuteChanged、绑定新CanExecuteChanged
    ///     AddClassHandler：Avalonia 静态属性变更全局注册，所有 SearchBar 实例共用监听
    /// </summary>
    static SearchBar()
    {
        CommandProperty.Changed.AddClassHandler<SearchBar>((x, e) =>
        {
            x.OnCommandChanged(e.OldValue as ICommand, e.NewValue as ICommand);
        });
        /*
        多个 ICommand 属性（ClearedCommand、FilterConfirmedCommand……）要不要全部监听？
        结论：
            只有需要 CanExecute 驱动 IsEnabled 的 Command 才需要监听逻辑。
        区分两种 Command，规则不一样：
            ① 需要管控「控件 IsEnabled」的主 Command -> 必须加 Changed 监听（你现在的 Command 属于此类）
                绑定后需要：CanExecute->自动禁用/启用SearchBar整体
                必须：XxxCommandProperty.Changed + OnXxxCommandChanged + 挂CanExecuteChanged
            ② 普通回调 Command（ClearedCommand / FilterConfirmedCommand）-> 不需要监听 Changed
                这类 Command 特征：
                不参与控制控件 IsEnabled；
                只在点击 / 触发时 Command?.Execute(...)；
                即便没解绑 CanExecuteChanged，也不影响控件可用性；
            只用的时候判空调用 Execute，不用订阅 CanExecuteChanged -> 不用注册 Property.Changed
        举例：
            // 1. 主搜索Command：控制IsEnabled -> 必须监听Changed
            public static readonly StyledProperty<ICommand?> CommandProperty = ...
            // 2. 清空回调Command：只触发执行、不控IsEnabled -> 不用监听Changed
            public static readonly StyledProperty<ICommand?> ClearedCommandProperty = ...
            // 3. 筛选弹窗确认Command：只触发执行、不控IsEnabled -> 不用监听Changed
            public static readonly StyledProperty<ICommand?> FilterConfirmedCommandProperty = ...
         */
    }

    public SearchBar()
    {
        _debounceTimer = new DispatcherTimer();
        _debounceTimer.Tick += DebounceTimerOnTick;
    }

    // 标记：只要 XAML 设置IsEnabled="False"，后续 Command 无论 CanExecute 返回 true/false 都无法启用控件
    private bool _explicitlyDisabled;
    // 防抖计时器
    private readonly DispatcherTimer _debounceTimer;

    
    
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        
        
    }

    /// <summary>
    /// 离开可视树时的逻辑
    /// </summary>
    /// <param name="e"></param>
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        
        ////////// 搜索
        // 离开可视树停止防抖计时器
        // 定时器是实例私有字段，随 SearchBar 生命周期销毁。控件彻底销毁时，_debounceTimer随实例 GC 自动释放，无内存泄漏。
        _debounceTimer.Stop();
        
        
    }

    /// <summary>
    /// 监听属性变化
    /// </summary>
    /// <param name="change"></param>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        // 监听手动修改IsEnabled，记录用户禁用状态
        if (change.Property == IsEnabledProperty)
        {
            _explicitlyDisabled = !IsEnabled;
        }

        // Text 属性变化 + IsRealTime=true -> 实时搜索，执行防抖逻辑
        if (change.Property == TextProperty)
        {
            // 首次初始化赋值直接跳过实时防抖
            if (IsRealTime)
            {
                RestartDebounceTimer();
            }
        }

        // Command 属性赋值变更，手动触发命令切换逻辑（补充静态监听兜底）
        if (change.Property == CommandProperty)
        {
            OnCommandChanged(change.OldValue as ICommand, change.NewValue as ICommand);
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        // 回车触发搜索
        if (e.Key == Key.Enter)
        {
            // 按下回车立即搜索，终止防抖
            _debounceTimer.Stop();
            Search();
            e.Handled = true;
        }
    }

    /// <summary>
    /// 命令切换解绑 / 绑定监听
    /// 核心目的：
    ///     更换 Command 时取消旧命令订阅，防止内存泄漏
    ///     新命令订阅CanExecuteChanged，命令可用性变化自动回调
    ///     立刻刷新 IsEnabled 状态
    /// </summary>
    /// <param name="oldCommand"></param>
    /// <param name="newCommand"></param>
    private void OnCommandChanged(ICommand? oldCommand, ICommand? newCommand)
    {
        if (oldCommand != null) oldCommand.CanExecuteChanged -= CanExecuteChanged;
        if (newCommand != null) newCommand.CanExecuteChanged += CanExecuteChanged;

        UpdateCanExecute();
    }
    /// <summary>
    /// 命令可用性变化，修改IsEnabled
    /// VM 里ICommand.CanExecute(false) -> 控件自动IsEnabled=false禁用
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CanExecuteChanged(object? sender, EventArgs e) => UpdateCanExecute();
    /// <summary>
    /// 命令可用性变化，修改IsEnabled
    /// 优先级：用户手动禁用 > Command可用性
    /// VM 里ICommand.CanExecute(false) -> 控件自动IsEnabled=false禁用
    /// </summary>
    private void UpdateCanExecute()
    {
        // 用户手动设了IsEnabled=False，Command无权改回启用
        if (_explicitlyDisabled)
        {
            IsEnabled = false;
            return;
        }
        
        IsEnabled = Command == null || Command.CanExecute(CommandParameter);
        // 上 等于 下
        // if (Command == null) { IsEnabled = true; return; }
        // IsEnabled = Command.CanExecute(CommandParameter);
    }

    
    
    #region Search
    /// <summary>
    /// 统一搜索入口，触发搜索事件 + 执行Command
    /// 整个控件最关键方法，统一搜索入口
    ///     向外冒泡SearchStarted事件，事件模式拿到 Text (Info)
    ///     自动覆盖 CommandParameter = 当前输入文本（XAML 不用手动绑定CommandParameter="{Binding Text,RelativeSource=Self}"）
    ///     校验命令可用性，执行 ICommand
    ///     重点：不用 XAML 绑定 CommandParameter，代码内部自动赋值 Text，你之前写的绑定可以删掉
    /// </summary>
    public virtual void Search()
    {
        // 开头判断 IsEnabled：禁用状态直接拦截实时输入、回车、按钮点击所有搜索逻辑
        if (!IsEnabled) 
            return;
        
        string txt = Text ?? string.Empty;
        
        // 文本为空 -> 触发清空事件与命令，不再搜索
        if (string.IsNullOrEmpty(txt))
        {
            // 抛出事件 + Command
            RaiseEvent(new RoutedEventArgs(ClearedEvent, this));
            ClearedCommand?.Execute(null);
            return;
        }
        
        // 字符长度不足最小限制，不搜索
        if (txt.Length < MinSearchLength)
            return;
        
        // 执行搜索，停止防抖计时器
        _debounceTimer.Stop();
        
        // 抛出路由事件
        RaiseEvent(new FunctionEventArgs<string>(SearchStartedEvent, this, Text));

        // TODO: 不需要 CommandParameter，执行命令时传入 Text 即可，这里留着作为参考示例
        // 更新CommandParameter为最新的Text值，否则在执行命令时，Text是新值，但CommandParameter是旧值
        // CommandParameter = Text; 会修改绑定源，如果 XAML 绑定了 CommandParameter 会被代码覆盖（设计如此：优先使用输入文本做参数）
        // 自动覆盖 CommandParameter = "当前输入文本"（XAML 不用手动绑定CommandParameter="{Binding Text,RelativeSource=Self}"）
        CommandParameter = Text;

        // 满足CanExecute则执行命令
        if (Command?.CanExecute(CommandParameter) == true)
        {
            Command.Execute(CommandParameter);
        }
    }

    #region Debounce
    private void DebounceTimerOnTick(object? sender, EventArgs e)
    {
        _debounceTimer.Stop();
        Search();
    }
    /// <summary>
    /// 重置防抖倒计时
    /// </summary>
    private void RestartDebounceTimer()
    {
        _debounceTimer.Stop();
        if (_debounceTimer.Interval != TimeSpan.FromMilliseconds(DebounceMilliseconds))
            _debounceTimer.Interval = TimeSpan.FromMilliseconds(DebounceMilliseconds);
        _debounceTimer.Start();
    }
    #endregion Debounce
    #endregion Search
    
    
    
}

/// <summary>
/// 搜索事件参数
/// 继承 AvaloniaRoutedEventArgs，自定义携带附加数据 Info，搜索时把Text放到Info，外部订阅SearchStarted事件可以拿到搜索文本
/// 替代 WPF 的FunctionEventArgs，Avalonia 没有内置该参数类
/// </summary>
/// <typeparam name="T"></typeparam>
public class FunctionEventArgs<T>(RoutedEvent routedEvent, object source, T? info) : RoutedEventArgs(routedEvent, source)
{
    public T? Info { get; init; } = info;
}
// 上 等于 下
// public class FunctionEventArgs<T> : RoutedEventArgs
// {
//     public FunctionEventArgs(RoutedEvent routedEvent, object source, T? info) : base(routedEvent, source)
//     {
//         Info = info;
//     }
//
//     public T? Info { get; init; }
// }

public class FilterOption : INotifyPropertyChanged
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    private int _count;
    public int Count { get => _count; set => SetField(ref _count, value); }
    private bool _isChecked;
    public bool IsChecked { get => _isChecked; set => SetField(ref _isChecked, value); }
    
    
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}