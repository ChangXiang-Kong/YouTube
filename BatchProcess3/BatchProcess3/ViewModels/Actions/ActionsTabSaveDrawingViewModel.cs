using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using BatchProcess3.EntityFramework.Entities.Actions;
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
    private ObservableCollection<string> _sheetsFilter = [];
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private ObservableCollection<string> _exportFormats = [];
    
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
        ExportFormats = viewModel.ExportFormats.ToList(),
        SheetsFilter = viewModel.SheetsFilter.ToList(),
        SingleDwgDxf = viewModel.SingleDwgDxf,
        SingleEDrawing = viewModel.SingleEDrawing,
        SinglePdf = viewModel.SinglePdf
    };

    public static ActionsTabSaveDrawingViewModel ToViewModel(this ActionsTabSaveDrawingEntity entity) => new()
    {
        Id = entity.Id,
        Description = entity.Description,
        JobName = entity.JobName,
        FileName = entity.FileName,
        SaveLocation = entity.SaveLocation,
        ExportFormats = new(entity.ExportFormats),
        SheetsFilter = new(entity.SheetsFilter),
        SingleDwgDxf = entity.SingleDwgDxf,
        SingleEDrawing = entity.SingleEDrawing,
        SinglePdf = entity.SinglePdf
    };
}