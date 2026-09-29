using System.Text.Json;
using System.Text.Json.Serialization;
using BatchProcess3.Data;
using BatchProcess3.EntityFramework.Entities.Actions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels.Actions;

public partial class ActionsTabDrawingTemplatesViewModel : ViewModelBase
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
    private bool _isNewItem;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private DrawingTemplateOperation _operation;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _currentTemplatePath = "";
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _newTemplatePath = "";
    
    [JsonIgnore]
    public new bool HasChanged => IsNewItem || (SavedState != "" && SavedState != JsonSerializer.Serialize(this, JsonSerializerOptions));

}

public static class ActionsTabDrawingTemplateViewModelExtensions
{
    public static ActionsTabDrawingTemplateEntity ToEntity(this ActionsTabDrawingTemplatesViewModel viewModel) => new()
    {
        Id = viewModel.Id,
        Description = viewModel.Description,
        JobName = viewModel.JobName,
        Operation = viewModel.Operation,
        CurrentTemplatePath = viewModel.CurrentTemplatePath,
        NewTemplatePath = viewModel.NewTemplatePath
    };

    public static ActionsTabDrawingTemplatesViewModel ToViewModel(this ActionsTabDrawingTemplateEntity entity) => new()
    {
        Id = entity.Id,
        Description = entity.Description,
        JobName = entity.JobName,
        Operation = entity.Operation,
        CurrentTemplatePath = entity.CurrentTemplatePath,
        NewTemplatePath = entity.NewTemplatePath
    };
}