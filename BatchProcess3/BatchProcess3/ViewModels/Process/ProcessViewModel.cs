using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using BatchProcess3.EntityFramework.Entities;
using BatchProcess3.EntityFramework.Entities.Actions;
using BatchProcess3.Tools.Extensions;
using BatchProcess3.ViewModels.Actions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels.Process;

public partial class ProcessViewModel : ViewModelBase, ISelectableItemsListViewModel
{
    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _jobName = "";
    
    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _description = "";

    public ObservableCollection<ActionViewModel> Actions
    {
        get => field;
        set => this.SetAndObserveEverything(ref field, value, [nameof(HasChanged)]);
    } = [];
    
    [ObservableProperty] private bool _isNewItem;
    
    [JsonIgnore]
    public override bool HasChanged 
        => IsNewItem || (SavedState != "" && SavedState != JsonSerializer.Serialize(this, GetType(), JsonSerializerOptions));
    
}

public static class ProcessViewModelBaseExtensions
{
    public static ProcessEntity ToEntity(this ProcessViewModel viewModel) => new()
    {
        Id = viewModel.Id,
        Description = viewModel.Description,
        JobName = viewModel.JobName,
        Actions = viewModel.Actions.Select(f => f.ToEntity()).ToList()
    };
    
    public static ProcessViewModel ToViewModel(this ProcessEntity entity) => new()
    {
        Id = entity.Id,
        JobName = entity.JobName,
        Description = entity.Description,
        Actions = new(entity.Actions.Select(f => f.ToViewModel())),
    };
}