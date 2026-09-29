using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using BatchProcess3.EntityFramework.Entities.Actions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels.Actions;

public partial class ActionsTabSaveModelViewModel : ViewModelBase
{
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _id = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _jobName = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _description = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _fileName = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private ObservableCollection<string> _exportFormats = [];

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _saveAllConfigurations;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _saveLocation = "";

    [ObservableProperty] private bool _isNewItem;

    [JsonIgnore]
    public new bool HasChanged =>
        IsNewItem || (SavedState != "" && SavedState != JsonSerializer.Serialize(this, JsonSerializerOptions));

}

public static class ActionsTabSaveModelViewModelExtensions
{
    public static ActionsTabSaveModelEntity ToEntity(this ActionsTabSaveModelViewModel viewModel) => new()
    {
        Id = viewModel.Id,
        Description = viewModel.Description,
        JobName = viewModel.JobName,
        SaveLocation = viewModel.SaveLocation,
        ExportFormats = viewModel.ExportFormats.ToList(),
        FileName = viewModel.FileName,
        SaveAllConfigurations = viewModel.SaveAllConfigurations
    };

    public static ActionsTabSaveModelViewModel ToViewModel(this ActionsTabSaveModelEntity entity) => new()
    {
        Id = entity.Id,
        Description = entity.Description,
        JobName = entity.JobName,
        SaveLocation = entity.SaveLocation,
        ExportFormats = new(entity.ExportFormats),
        FileName = entity.FileName,
        SaveAllConfigurations = entity.SaveAllConfigurations
    };
}