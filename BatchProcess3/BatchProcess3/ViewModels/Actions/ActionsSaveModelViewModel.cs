using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using BatchProcess3.EntityFramework.Entities.Actions;
using BatchProcess3.Tools.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels.Actions;

public partial class ActionsSaveModelViewModel : ActionViewModel, ISelectableItemsListViewModel
{
    public ActionsSaveModelViewModel()
    {
        ExportFormats = [];     // 2、可以在构造函数中设置一个值，这样就会调用 SetAndObserveEverything() 方法，
    }
    
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _fileName = "";

    public ObservableCollection<ObservableKeyValuePair<string, bool>> ExportFormats
    {
        get => field;
        set => this.SetAndObserveEverything(ref field, value, [nameof(HasChanged)]);
    // } = [];      // 1、不能使用默认值，因为设置默认值不会触发 SetAndObserveEverything() 方法，导致没有调用内部事件
    }

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _saveAllConfigurations;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _saveLocation = "";

}

public static class ActionsSaveModelViewModelExtensions
{
    public static ActionsSaveModelEntity ToEntity(this ActionsSaveModelViewModel viewModel) => new()
    {
        Id = viewModel.Id,
        Description = viewModel.Description,
        JobName = viewModel.JobName,
        SaveLocation = viewModel.SaveLocation,
        ExportFormats = viewModel.ExportFormats.Where(x => x.Value).Select(x => x.Key).ToList(),
        FileName = viewModel.FileName,
        SaveAllConfigurations = viewModel.SaveAllConfigurations
    };

    public static ActionsSaveModelViewModel ToViewModel(this ActionsSaveModelEntity saveModelEntity, ObservableCollection<string> exportFormats) => new()
    {
        Id = saveModelEntity.Id,
        Description = saveModelEntity.Description,
        JobName = saveModelEntity.JobName,
        SaveLocation = saveModelEntity.SaveLocation,
        ExportFormats = new(exportFormats.Select(x => new ObservableKeyValuePair<string, bool>(x, saveModelEntity.ExportFormats.Any(f => f == x)))),
        FileName = saveModelEntity.FileName,
        SaveAllConfigurations = saveModelEntity.SaveAllConfigurations
    };
}