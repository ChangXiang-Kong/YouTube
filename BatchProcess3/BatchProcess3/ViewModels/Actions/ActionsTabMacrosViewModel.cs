using System.Text.Json;
using System.Text.Json.Serialization;
using BatchProcess3.EntityFramework.Entities.Actions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels.Actions;

public partial class ActionsTabMacrosViewModel : ViewModelBase
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
    private string _macroPath = "";
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _moduleName = "";
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _excludeParts;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _excludeDrawings;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _excludeAssemblies;
    
    [ObservableProperty]
    private bool _isNewItem;
    
    [JsonIgnore]
    public new bool HasChanged => IsNewItem || (SavedState != "" && SavedState != JsonSerializer.Serialize(this, JsonSerializerOptions));

}

public static class ActionsTabMacrosViewModelExtensions
{
    public static ActionsTabMacrosEntity ToEntity(this ActionsTabMacrosViewModel viewModel) => new()
    {
        Id = viewModel.Id,
        Description = viewModel.Description,
        JobName = viewModel.JobName,
        MacroPath = viewModel.MacroPath,
        ModuleName = viewModel.ModuleName,
        ExcludeParts = viewModel.ExcludeParts,
        ExcludeDrawings = viewModel.ExcludeDrawings,
        ExcludeAssemblies = viewModel.ExcludeAssemblies
    };

    public static ActionsTabMacrosViewModel ToViewModel(this ActionsTabMacrosEntity entity) => new()
    {
        Id = entity.Id,
        Description = entity.Description,
        JobName = entity.JobName,
        MacroPath = entity.MacroPath,
        ModuleName = entity.ModuleName,
        ExcludeParts = entity.ExcludeParts,
        ExcludeDrawings = entity.ExcludeDrawings,
        ExcludeAssemblies = entity.ExcludeAssemblies
    };
}