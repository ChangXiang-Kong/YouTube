using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BatchProcess3.Tools.Converters;
using BatchProcess3.ViewModels.Actions;

namespace BatchProcess3.ViewModels;

public partial class ViewModelBase : ObservableObject
{
    public ViewModelBase()
    {
        // Detect design time 
        if (Avalonia.Controls.Design.IsDesignMode)
            OnDesignTimeConstructor();
    }

    protected virtual void OnDesignTimeConstructor() { }
    
    /// <summary>
    /// <code>
    /// 在 ViewModel 中重写：
    ///     public override void OnViewLoaded()
    ///     {
    ///         // some logic
    ///     }
    /// 在 View 中调用：
    ///     public TestPageView()
    ///     {
    ///         InitializeComponent();
    ///         Loaded += OnLoaded;
    ///     }
    ///     private void OnLoaded(object? sender, RoutedEventArgs e)
    ///     {
    ///         ((ViewModelBase)DataContext)?.OnViewLoaded();
    ///     }
    /// </code>
    /// </summary>
    public virtual void OnViewLoaded() { }
    
    // 参考视频：https://www.youtube.com/watch?v=xR5115U_RdI&list=PLrW43fNmjaQWwIdZxjZrx5FSXcNzaucOO&index=31
    // 可多看账几遍，视频中出现多次错误与解决思路，有助于了解 Json 的使用
    protected readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        // KeyValuePair 是 只读的，若想要进行序列化，这里不能为 true
        // public readonly struct KeyValuePair<TKey, TValue>(TKey key, TValue value)
        IgnoreReadOnlyFields = false,
        IgnoreReadOnlyProperties = false,
        NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,    // 处理 如 double.NaN 之类的无限数对象
        // Converters = { new StreamGeometryConverter() }, // 添加自定义转换器
    };
    
    // TODO: 使用 Guid 类型 还是 string 类型？
    [ObservableProperty] 
    public virtual partial string Id { get; set; } = Guid.CreateVersion7().ToString();

    /* [JsonIgnore] 与 [property: JsonIgnore] 的区别
        [JsonIgnore]            → 默认作用于 字段（Field）
        [property: JsonIgnore]  → 强制作用于 自动属性（Property）
        最清晰的总结
        ① 普通字段
            [JsonIgnore]
            public bool HasChanged = false;
            作用：忽略字段
        ② 字段但希望按属性规则忽略
            [property: JsonIgnore]
            public string SavedState = "";
            作用：把这个字段当成属性，并且忽略它
        ③ 真正的属性（最标准写法）
            [JsonIgnore]
            public string SavedState { get; set; }
     */
    [property: JsonIgnore]
    public string SavedState = "";

    [JsonIgnore]
    public virtual bool HasChanged => SavedState != "" && SavedState != JsonSerializer.Serialize(this, JsonSerializerOptions);
    
    public void SetSaveState()
    {
        SavedState = GetState();
        OnPropertyChanged((nameof(HasChanged)));
    }

    // public string GetState() => JsonSerializer.Serialize(this, GetType().DeclaringType ?? GetType(), _jsonSerializerOptions);
    public string GetState()
    {
        var state = JsonSerializer.Serialize(this, GetType().DeclaringType ?? GetType(), JsonSerializerOptions);
        // System.Diagnostics.Debug.WriteLine($"===== Serialized State: {state}");
        return state;
    }
    
    public void RestoreState(string? stateToRestore = null)
    {
        stateToRestore ??= SavedState;

        var type = GetType().DeclaringType ?? GetType();
        
        var savedState = JsonSerializer.Deserialize(stateToRestore, type, JsonSerializerOptions);
        // System.Diagnostics.Debug.WriteLine($"===== Serialized SavedState: {savedState}");
        
        // 反射
        foreach (var propertyInfo in type.GetProperties())
        {
            // Only set setters, not get only properties
            if (!propertyInfo.CanWrite)
                continue;
        
            // Ignore any properties that have a JsonIgnore attribute
            if (propertyInfo.GetCustomAttributes(typeof(JsonIgnoreAttribute), false).GetLength(0) > 0)
                continue;
        
            // Pull the saved value
            var originalValue = propertyInfo.GetValue(savedState);
            // Restore it to this calss
            propertyInfo.SetValue(this, originalValue);
        }
    }
}