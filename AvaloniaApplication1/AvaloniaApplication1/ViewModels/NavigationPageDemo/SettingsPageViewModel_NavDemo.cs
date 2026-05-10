using System.Threading.Tasks;
using AvaloniaApplication1.Data;
using AvaloniaApplication1.Tools.Services;
using AvaloniaApplication1.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication1.ViewModels;

public partial class SettingsPageViewModel_NavDemo() : PageViewModel(ApplicationPageName.Settings)
{
    [ObservableProperty] 
    private string _message;

    [RelayCommand]
    private async Task GoBack()
        => await App.NavigationService?.PopAsync();

    [RelayCommand]
    private async Task ReplacePage()
        => await App.NavigationService?.ReplaceAsync<DetailPage_NavDemo, DetailPageViewModel_NavDemo>();
}