using System.Text.Json;
using System.Text.Json.Serialization;
using BatchProcess3.EntityFramework.Entities.Actions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels.Actions;

public partial class ActionsFileInfoViewModel : ActionViewModel
{
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _keywords = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _subject = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _title = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _author = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _comments = "";

}

public static class ActionsFileInfoViewModelExtensions
{
    public static ActionsFileInfoEntity ToEntity(this ActionsFileInfoViewModel viewModel) => new()
    {
        Id = viewModel.Id,
        Description = viewModel.Description,
        JobName = viewModel.JobName,
        Author = viewModel.Author,
        Comments = viewModel.Comments,
        Keywords = viewModel.Keywords,
        Subject = viewModel.Subject,
        Title = viewModel.Title
    };

    public static ActionsFileInfoViewModel ToViewModel(this ActionsFileInfoEntity fileInfoEntity) => new()
    {
        Id = fileInfoEntity.Id,
        Description = fileInfoEntity.Description,
        JobName = fileInfoEntity.JobName,
        Author = fileInfoEntity.Author,
        Comments = fileInfoEntity.Comments,
        Keywords = fileInfoEntity.Keywords,
        Subject = fileInfoEntity.Subject,
        Title = fileInfoEntity.Title
    };
}