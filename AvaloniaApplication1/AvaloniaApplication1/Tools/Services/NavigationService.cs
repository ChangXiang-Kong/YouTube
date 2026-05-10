using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using AvaloniaApplication1.Tools.Factories;
using AvaloniaApplication1.ViewModels;

namespace AvaloniaApplication1.Tools.Services;

public class NavigationService(PageNavigationFactory pageNavigationFactory) : INavigationService
{
    // 上 等价于 下
// public class NavigationService : INavigationService
// {
//     // 注入：从 DI 来的 PageNavigateFactory
//     public NavigationService(PageNavigationFactory pageNavigationFactory)
//     {
//         _pageNavigationFactory = pageNavigationFactory;
//     }
//     private readonly PageNavigationFactory _pageNavigationFactory;

    private NavigationPage? _navigationPage;

    public void Initialize(NavigationPage navigationPage)
    {
        _navigationPage = navigationPage;
    }

    public async Task PushAsync<TPage, TViewModel>(Action<TViewModel>? configureViewModel = null)
        where TPage : Page, new()
        where TViewModel : PageViewModel
    {
        ArgumentNullException.ThrowIfNull(_navigationPage);
        var page = pageNavigationFactory.GetPageWithViewModel<TPage, TViewModel>(configureViewModel);
        await _navigationPage!.PushAsync(page);
    }

    public async Task ReplaceAsync<TPage, TViewModel>(Action<TViewModel>? configureViewModel = null)
        where TPage : Page, new()
        where TViewModel : PageViewModel
    {
        ArgumentNullException.ThrowIfNull(_navigationPage);
        var page = pageNavigationFactory.GetPageWithViewModel<TPage, TViewModel>(configureViewModel);
        await _navigationPage!.ReplaceAsync(page);
    }

    public async Task PopAsync()
    {
        ArgumentNullException.ThrowIfNull(_navigationPage);
        await _navigationPage!.PopAsync();
    }

    public async Task PopToRootAsync()
    {
        ArgumentNullException.ThrowIfNull(_navigationPage);
        await _navigationPage!.PopToRootAsync();
    }
}