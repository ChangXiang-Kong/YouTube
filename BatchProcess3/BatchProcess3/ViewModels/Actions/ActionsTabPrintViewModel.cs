using System;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using BatchProcess3.EntityFramework.Entities.Actions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels.Actions;

public partial class ActionsTabPrintViewModel : ViewModelBase
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private new string _id = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _jobName = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _description = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _printDrawingRange = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _drawingExclusionList = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DrawingExclusionListTitle))]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _drawingExclusionIsWhiteList;

    public string DrawingExclusionListTitle => DrawingExclusionIsWhiteList ? "White List" : "Black List";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _printModels;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _printDrawings;

    [ObservableProperty]
    private bool _isNewItem;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string? _printSettingsId = "";

    [JsonIgnore]
    public new bool HasChanged => IsNewItem || (SavedState != "" && SavedState != JsonSerializer.Serialize(this, JsonSerializerOptions));

}

public static class ActionsTabPrintViewModelExtensions
{
    public static ActionsTabPrintEntity ToEntity(this ActionsTabPrintViewModel viewModel) => new()
    {
        // Id = Guid.Parse(Id),
        Id = viewModel.Id,
        JobName = viewModel.JobName,
        Description = viewModel.Description,
        PrintDrawingRange = viewModel.PrintDrawingRange,
        DrawingExclusionList = viewModel.DrawingExclusionList,
        DrawingExclusionIsWhiteList = viewModel.DrawingExclusionIsWhiteList,
        PrintModels = viewModel.PrintModels,
        PrintDrawings = viewModel.PrintDrawings,
        // PrintSettingsId = Guid.TryParse(PrintSettingsId, out Guid res) ? res : Guid.Empty,
        PrintSettingsId = viewModel.PrintSettingsId ?? "null",
    };

    public static ActionsTabPrintViewModel ToViewModel(this ActionsTabPrintEntity entity) => new()
    {
        Id = entity.Id,
        JobName = entity.JobName,
        Description = entity.Description,
        DrawingExclusionIsWhiteList = entity.DrawingExclusionIsWhiteList,
        DrawingExclusionList = entity.DrawingExclusionList,
        PrintDrawingRange = entity.PrintDrawingRange,
        PrintModels = entity.PrintModels,
        PrintDrawings = entity.PrintDrawings,
        PrintSettingsId = entity.PrintSettingsId,
    };
}