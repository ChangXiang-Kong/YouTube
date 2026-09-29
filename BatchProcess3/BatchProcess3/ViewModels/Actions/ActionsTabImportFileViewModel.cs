using System.Text.Json;
using System.Text.Json.Serialization;
using BatchProcess3.EntityFramework.Entities.Actions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels.Actions;

public partial class ActionsTabImportFileViewModel : ViewModelBase
{
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _id = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _fileName = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _description = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _jobName = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _saveLocation = "";
    
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _addToProject;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _importArguments = "";

    [ObservableProperty] private bool _isNewItem;

    [JsonIgnore]
    public new bool HasChanged => IsNewItem || (SavedState != "" && SavedState != JsonSerializer.Serialize(this, JsonSerializerOptions));
    
}

public static class ActionsTabImportFileViewModelExtensions
{
    public static ActionsTabImportFileEntity ToEntity(this ActionsTabImportFileViewModel viewModel) => new()
    {
        Id = viewModel.Id,
        Description = viewModel.Description,
        JobName = viewModel.JobName,
        FileName = viewModel.FileName,
        SaveLocation = viewModel.SaveLocation,
        ImportArguments = viewModel.ImportArguments,
        AddToProject = viewModel.AddToProject
    };

    public static ActionsTabImportFileViewModel ToViewModel(this ActionsTabImportFileEntity entity) => new()
    {
        Id = entity.Id,
        Description = entity.Description,
        JobName = entity.JobName,
        FileName = entity.FileName,
        SaveLocation = entity.SaveLocation,
        ImportArguments = entity.ImportArguments,
        AddToProject = entity.AddToProject
    };
}