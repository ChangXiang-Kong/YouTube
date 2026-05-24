using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels.Actions;

public partial class ActionsPrintViewModel : ViewModelBase
{
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
    private string _saveState = "";
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _id = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _jobName = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _description = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _printDrawingRange = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _drawingExclusionList = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DrawingExclusionListTitle))]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _drawingExclusionIsWhiteList;

    public string DrawingExclusionListTitle => DrawingExclusionIsWhiteList ? "White List" : "Black List";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _printModels;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _printDrawings;

    [ObservableProperty]
    private bool _isNewItem;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _printerProfileId = "";

    [JsonIgnore]
    public bool HasChanged => IsNewItem || (_saveState != "" && _saveState != JsonSerializer.Serialize(this));

    public void SetSaveState()
    {
        _saveState = JsonSerializer.Serialize(this);
        OnPropertyChanged((nameof(HasChanged)));
    }

    public void RestoreSavedState()
    {
        var savedState = JsonSerializer.Deserialize<ActionsPrintViewModel>(_saveState);
        // 反射
        foreach (var propertyInfo in GetType().GetProperties())
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