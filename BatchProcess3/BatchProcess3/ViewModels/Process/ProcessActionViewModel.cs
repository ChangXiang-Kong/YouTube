using System.Text.Json;
using System.Text.Json.Serialization;
using BatchProcess3.EntityFramework.Entities;
using BatchProcess3.EntityFramework.Entities.Actions;
using BatchProcess3.EntityFramework.Entities.Process;
using BatchProcess3.ViewModels.Actions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels.Process;

public partial class ProcessActionViewModel : ActionViewModel
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _processId = "";
}

public static class ProcessActionViewModelExtensions
{
    public static ProcessActionEntity ToEntity(this ProcessActionViewModel viewModel) => new()
    {
        Id = viewModel.Id,
        JobName = viewModel.JobName,
        Description = viewModel.Description,
        SortOrder = viewModel.SortOrder,
        ProcessId = viewModel.ProcessId,
    };
    
    public static ProcessActionViewModel ToViewModel(this ProcessActionEntity entity) => new()
    {
        Id = entity.Id,
        JobName = entity.JobName,
        Description = entity.Description,
        SortOrder = entity.SortOrder,
        ProcessId = entity.ProcessId,
    };
    
    public static ProcessActionViewModel ToProcessActionViewModel(this ActionEntity entity) => new()
    {
        Id = entity.Id,
        JobName = entity.JobName,
        Description = entity.Description,
        SortOrder = entity.SortOrder,
        // ProcessId = entity.ProcessId,
    };
}