using System.Text.Json;
using System.Text.Json.Serialization;
using BatchProcess3.EntityFramework.Entities.Actions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels.Actions;

public partial class ActionViewModel : ViewModelBase
{
    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _jobName = "";

    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _description = "";
    
    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private int _sortOrder;
    
    [ObservableProperty] 
    private bool _isNewItem;

    [JsonIgnore]
    public override bool HasChanged 
        => IsNewItem || (SavedState != "" && SavedState != JsonSerializer.Serialize(this, GetType(), JsonSerializerOptions));
    
}

public static class ActionViewModelExtensions
{
    public static ActionEntityBase ToEntity(this ActionViewModel viewModel) => new()
    {
        Id = viewModel.Id,
        Description = viewModel.Description,
        JobName = viewModel.JobName,
        SortOrder = viewModel.SortOrder,
    };
    
    public static ActionViewModel ToViewModel(this ActionEntityBase entity) => new()
    {
        Id = entity.Id,
        JobName = entity.JobName,
        Description = entity.Description,
        SortOrder = entity.SortOrder,
    };
}