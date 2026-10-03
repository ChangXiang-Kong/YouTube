using System.Text.Json;
using System.Text.Json.Serialization;
using BatchProcess3.EntityFramework.Entities.Actions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels.Actions;

public partial class ActionsMacrosViewModel : ActionViewModel
{
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

}

public static class ActionsTabMacrosViewModelExtensions
{
    public static ActionsMacrosEntity ToEntity(this ActionsMacrosViewModel viewModel) => new()
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

    public static ActionsMacrosViewModel ToViewModel(this ActionsMacrosEntity macrosEntity) => new()
    {
        Id = macrosEntity.Id,
        Description = macrosEntity.Description,
        JobName = macrosEntity.JobName,
        MacroPath = macrosEntity.MacroPath,
        ModuleName = macrosEntity.ModuleName,
        ExcludeParts = macrosEntity.ExcludeParts,
        ExcludeDrawings = macrosEntity.ExcludeDrawings,
        ExcludeAssemblies = macrosEntity.ExcludeAssemblies
    };
}