using System;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using BatchProcess3.EntityFramework.Entities.Actions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels.Actions;

public partial class ActionsPrintViewModel : ActionViewModel
{
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
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string? _printSettingsId = "";

}

public static class ActionsTabPrintViewModelExtensions
{
    public static ActionsPrintEntity ToEntity(this ActionsPrintViewModel viewModel) => new()
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

    public static ActionsPrintViewModel ToViewModel(this ActionsPrintEntity printEntity) => new()
    {
        Id = printEntity.Id,
        JobName = printEntity.JobName,
        Description = printEntity.Description,
        DrawingExclusionIsWhiteList = printEntity.DrawingExclusionIsWhiteList,
        DrawingExclusionList = printEntity.DrawingExclusionList,
        PrintDrawingRange = printEntity.PrintDrawingRange,
        PrintModels = printEntity.PrintModels,
        PrintDrawings = printEntity.PrintDrawings,
        PrintSettingsId = printEntity.PrintSettingsId,
    };
}