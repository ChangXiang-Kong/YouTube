using System.Threading.Tasks;
using AvaloniaApplication1.Data;
using AvaloniaApplication1.Tools.Services;
using AvaloniaApplication1.Views;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication1.ViewModels;

public partial class HomePageViewModel_NavDemo() : PageViewModel(ApplicationPageName.Home)
{
    [RelayCommand]
    private async Task GoToDetail()
        => await App.NavigationService?.PushAsync<DetailPage_NavDemo, DetailPageViewModel_NavDemo>();

    [RelayCommand]
    private async Task GoToSettings()
    {
        await App.NavigationService?.PushAsync<SettingsPage_NavDemo, SettingsPageViewModel_NavDemo>(vm =>
        {
            vm.Message = "来自首页的参数消息：MVVM 导航成功！";
        })!;
    }
}