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

public partial class ActionsSaveDrawingViewModel : ActionViewModel
{
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
        get => field ??= [];
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
    
}

public static class ActionsTabSaveDrawingViewModelExtensions
{
    public static ActionsSaveDrawingEntity ToEntity(this ActionsSaveDrawingViewModel viewModel) => new()
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

    public static ActionsSaveDrawingViewModel ToViewModel(this ActionsSaveDrawingEntity saveDrawingEntity, ObservableCollection<string> exportFormats) => new()
    {
        Id = saveDrawingEntity.Id,
        Description = saveDrawingEntity.Description,
        JobName = saveDrawingEntity.JobName,
        FileName = saveDrawingEntity.FileName,
        SaveLocation = saveDrawingEntity.SaveLocation,
        ExportFormats = new(exportFormats.Select(x => new ObservableKeyValuePair<string, bool>(x, saveDrawingEntity.ExportFormats.Any(f => f == x)))),
        SheetsFilter = saveDrawingEntity.SheetsFilter,
        SingleDwgDxf = saveDrawingEntity.SingleDwgDxf,
        SingleEDrawing = saveDrawingEntity.SingleEDrawing,
        SinglePdf = saveDrawingEntity.SinglePdf
    };
}