using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using BatchProcess3.EntityFramework.Entities;
using BatchProcess3.EntityFramework.Entities.Actions;
using BatchProcess3.EntityFramework.Entities.Process;
using BatchProcess3.Tools.Extensions;
using BatchProcess3.ViewModels.Actions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels.Process;

public partial class ProcessViewModel : ViewModelBase, ISelectableItemsListViewModel
{
    public ProcessViewModel()
    {
        ProcessActionsList = [];    // 2、可以在构造函数中设置一个值，这样就会调用 SetAndObserveEverything() 方法，
    }
    
    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _jobName = "";
    
    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _description = "";

    public ObservableCollection<ProcessActionViewModel> ProcessActionsList
    {
        get => field;
        set => this.SetAndObserveEverything(ref field, value, [nameof(HasChanged)]);
    // } = [];      // 1、不能使用默认值，因为设置默认值不会触发 SetAndObserveEverything() 方法，导致没有调用内部事件
    }
    
    [ObservableProperty] private bool _isNewItem;
    
    [JsonIgnore]
    public override bool HasChanged 
        => IsNewItem || (SavedState != "" && SavedState != JsonSerializer.Serialize(this, GetType(), JsonSerializerOptions));

    // 用于方便调试时直观看到该 ProcessViewModel 的内容
    // public override string ToString() => $"{JobName} ({Description})";
}

public static class ProcessViewModelBaseExtensions
{
    public static ProcessEntity ToEntity(this ProcessViewModel viewModel) => new()
    {
        Id = viewModel.Id,
        Description = viewModel.Description,
        JobName = viewModel.JobName,
        ProcessActions = viewModel.ProcessActionsList.Select(f => f.ToEntity()).ToList()
    };
    
    public static ProcessViewModel ToViewModel(this ProcessEntity entity) => new()
    {
        Id = entity.Id,
        JobName = entity.JobName,
        Description = entity.Description,
        ProcessActionsList = new(entity.ProcessActions
            .Select(x => x.ToViewModel())
            .OrderBy(x => x.SortOrder)),
    };
}