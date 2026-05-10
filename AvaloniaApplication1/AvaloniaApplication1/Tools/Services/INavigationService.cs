using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using AvaloniaApplication1.ViewModels;

namespace AvaloniaApplication1.Tools.Services;

public interface INavigationService
{
    void Initialize(NavigationPage navigationPage);
    
    Task PushAsync<TPage, TViewModel>(Action<TViewModel>? configureViewModel = null)
        where TPage : Page, new()
        where TViewModel : PageViewModel;
    
    Task ReplaceAsync<TPage, TViewModel>(Action<TViewModel>? configureViewModel = null)
        where TPage : Page, new()
        where TViewModel : PageViewModel;
    
    Task PopAsync();
    
    Task PopToRootAsync();
}