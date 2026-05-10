using System.Threading.Tasks;
using AvaloniaApplication1.Data;
using AvaloniaApplication1.Tools.Services;
using AvaloniaApplication1.Views;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication1.ViewModels;

public partial class DetailPageViewModel_NavDemo() : PageViewModel(ApplicationPageName.Detail)
{
    [RelayCommand]
    private async Task GoToSettings()
    {
        await App.NavigationService?.PushAsync<SettingsPage_NavDemo, SettingsPageViewModel_NavDemo>(vm =>
        {
            vm.Message = "来自详情页的参数消息：MVVM 导航成功！";
        })!;
    }

    [RelayCommand]
    private async Task PopToRoot()
    {
        await App.NavigationService?.PopToRootAsync();
    }
}