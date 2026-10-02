using System.Text.Json;
using System.Text.Json.Serialization;
using BatchProcess3.Data;
using BatchProcess3.EntityFramework.Entities.Actions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels.Actions;

public partial class ActionsTabDrawingTemplateViewModel : ViewModelBase
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
    [NotifyPropertyChangedFor(nameof(CurrentTemplatePathIsVisible))]
    [NotifyPropertyChangedFor(nameof(NewTemplatePathIsVisible))]
    private DrawingTemplateOperation _operation;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string? _currentTemplatePath;

    public bool CurrentTemplatePathIsVisible => Operation is DrawingTemplateOperation.Replace;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string? _newTemplatePath;
    
    public bool NewTemplatePathIsVisible => Operation is not DrawingTemplateOperation.Reload;
    
    [ObservableProperty]
    private bool _isNewItem;
    
    [JsonIgnore]
    public new bool HasChanged => IsNewItem || (SavedState != "" && SavedState != JsonSerializer.Serialize(this, JsonSerializerOptions));

}

public static class ActionsTabDrawingTemplateViewModelExtensions
{
    public static ActionsTabDrawingTemplateEntity ToEntity(this ActionsTabDrawingTemplateViewModel viewModel) => new()
    {
        Id = viewModel.Id,
        Description = viewModel.Description,
        JobName = viewModel.JobName,
        Operation = viewModel.Operation,
        CurrentTemplatePath = viewModel.CurrentTemplatePath,
        NewTemplatePath = viewModel.NewTemplatePath
    };

    public static ActionsTabDrawingTemplateViewModel ToViewModel(this ActionsTabDrawingTemplateEntity entity) => new()
    {
        Id = entity.Id,
        Description = entity.Description,
        JobName = entity.JobName,
        Operation = entity.Operation,
        CurrentTemplatePath = entity.CurrentTemplatePath,
        NewTemplatePath = entity.NewTemplatePath
    };
}