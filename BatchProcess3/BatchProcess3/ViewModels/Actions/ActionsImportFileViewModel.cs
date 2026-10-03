using System.Text.Json;
using System.Text.Json.Serialization;
using BatchProcess3.EntityFramework.Entities.Actions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels.Actions;

public partial class ActionsImportFileViewModel : ActionViewModel
{
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _fileName = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _saveLocation = "";
    
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _addToProject;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _importArguments = "";
    
}

public static class ActionsTabImportFileViewModelExtensions
{
    public static ActionsImportFileEntity ToEntity(this ActionsImportFileViewModel viewModel) => new()
    {
        Id = viewModel.Id,
        Description = viewModel.Description,
        JobName = viewModel.JobName,
        FileName = viewModel.FileName,
        SaveLocation = viewModel.SaveLocation,
        ImportArguments = viewModel.ImportArguments,
        AddToProject = viewModel.AddToProject
    };

    public static ActionsImportFileViewModel ToViewModel(this ActionsImportFileEntity importFileEntity) => new()
    {
        Id = importFileEntity.Id,
        Description = importFileEntity.Description,
        JobName = importFileEntity.JobName,
        FileName = importFileEntity.FileName,
        SaveLocation = importFileEntity.SaveLocation,
        ImportArguments = importFileEntity.ImportArguments,
        AddToProject = importFileEntity.AddToProject
    };
}