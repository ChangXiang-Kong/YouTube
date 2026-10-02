using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using BatchProcess3.EntityFramework.Entities.Actions;
using BatchProcess3.Tools.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels.Actions;

public partial class ActionsTabSaveDrawingViewModel : ViewModelBase
{
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
    private string _fileName = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _saveLocation = "";
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _sheetsFilter = "";

    /// <summary>
    /// 参考视频：https://www.youtube.com/watch?v=33De56aQ4DU&list=PLrW43fNmjaQWwIdZxjZrx5FSXcNzaucOO&index=48    10:25
    /// </summary>
    public ObservableCollection<ObservableKeyValuePair<string, bool>> ExportFormats
    {
        get => field ?? [];
        /*set
        {
            if (field == value)
                return;
            
            field = value;
        
            PropertyChangedEventHandler propertyChangedEventHandler = (s, e) =>
            {
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasChanged));
            };
        
            void NotifyCollectionChangedEventHandler(object? s, NotifyCollectionChangedEventArgs e)
            {
                // 当集合的 内部元素 调用 PropertyChanged 时，调用 ChangedDelegate() 
                foreach (var property in field.OfType<ViewModelBase>())
                {
                    // 防止重复事件
                    property.PropertyChanged -= propertyChangedEventHandler;
                    property.PropertyChanged += propertyChangedEventHandler;
                }
        
                propertyChangedEventHandler(this, new PropertyChangedEventArgs(nameof(ExportFormats)));
            }
        
            // 当集合的 内部元素 调用 PropertyChanged 时，调用 ChangedDelegate() 
            foreach (var property in field.OfType<ViewModelBase>())
                property.PropertyChanged += propertyChangedEventHandler;
        
            // 当集合变更时，调用 ChangedDelegate() 
            field.CollectionChanged += NotifyCollectionChangedEventHandler;
        }*/
        // 上 等于 下
        set => this.SetAndObserveEverything(ref field, value, [nameof(HasChanged)]);
    }
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _singlePdf;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _singleEDrawing;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _singleDwgDxf;
    
    [ObservableProperty]
    private bool _isNewItem;
    
    [JsonIgnore]
    public new bool HasChanged => IsNewItem || (SavedState != "" && SavedState != JsonSerializer.Serialize(this, JsonSerializerOptions));
    
}

public static class ActionsTabSaveDrawingViewModelExtensions
{
    public static ActionsTabSaveDrawingEntity ToEntity(this ActionsTabSaveDrawingViewModel viewModel) => new()
    {
        Id = viewModel.Id,
        Description = viewModel.Description,
        JobName = viewModel.JobName,
        FileName = viewModel.FileName,
        SaveLocation = viewModel.SaveLocation,
        ExportFormats = viewModel.ExportFormats.Where(x => x.Value).Select(x => x.Key).ToList(),
        SheetsFilter = viewModel.SheetsFilter,
        SingleDwgDxf = viewModel.SingleDwgDxf,
        SingleEDrawing = viewModel.SingleEDrawing,
        SinglePdf = viewModel.SinglePdf
    };

    public static ActionsTabSaveDrawingViewModel ToViewModel(this ActionsTabSaveDrawingEntity entity, ObservableCollection<string> exportFormats) => new()
    {
        Id = entity.Id,
        Description = entity.Description,
        JobName = entity.JobName,
        FileName = entity.FileName,
        SaveLocation = entity.SaveLocation,
        ExportFormats = new(exportFormats.Select(x => new ObservableKeyValuePair<string, bool>(x, entity.ExportFormats.Any(f => f == x)))),
        SheetsFilter = entity.SheetsFilter,
        SingleDwgDxf = entity.SingleDwgDxf,
        SingleEDrawing = entity.SingleEDrawing,
        SinglePdf = entity.SinglePdf
    };
}