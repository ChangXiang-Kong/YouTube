using System.Text.Json;
using System.Text.Json.Serialization;
using BatchProcess3.Data;
using BatchProcess3.EntityFramework.Entities.Actions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels.Actions;

public partial class ActionsDrawingTemplateViewModel : ActionViewModel
{
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

}

public static class ActionsTabDrawingTemplateViewModelExtensions
{
    public static ActionsDrawingTemplateEntity ToEntity(this ActionsDrawingTemplateViewModel viewModel) => new()
    {
        Id = viewModel.Id,
        Description = viewModel.Description,
        JobName = viewModel.JobName,
        Operation = viewModel.Operation,
        CurrentTemplatePath = viewModel.CurrentTemplatePath,
        NewTemplatePath = viewModel.NewTemplatePath
    };

    public static ActionsDrawingTemplateViewModel ToViewModel(this ActionsDrawingTemplateEntity drawingTemplateEntity) => new()
    {
        Id = drawingTemplateEntity.Id,
        Description = drawingTemplateEntity.Description,
        JobName = drawingTemplateEntity.JobName,
        Operation = drawingTemplateEntity.Operation,
        CurrentTemplatePath = drawingTemplateEntity.CurrentTemplatePath,
        NewTemplatePath = drawingTemplateEntity.NewTemplatePath
    };
}