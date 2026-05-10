using System;
using System.Linq;
using Avalonia.Controls;
using AvaloniaApplication1.ViewModels;

namespace AvaloniaApplication1.Tools.Factories;

// 方式一：从依赖注入中获取(Func<Type, PageViewModel> factory)，并使用 factory 获取对应的 ViewModel 实例
/// <summary>
/// 用于 NavigationPage 导航的工厂类
/// </summary>
public class PageNavigationFactory(Func<Type, PageViewModel> factory)
{
    // 上 等价于 下
// public class PageNavigationFactory
// {
//     // 注入：从 DI 来的 ViewModelFactory
//     public PageNavigationFactory(Func<Type, PageViewModel> viewModelFactory)
//     {
//         _viewModelFactory = viewModelFactory;
//     }
//     private readonly Func<Type, PageViewModel> _viewModelFactory;

    /// <summary>
    /// 创建页面并绑定ViewModel + 执行传参委托
    /// </summary>
    public Page GetPageWithViewModel<TPage, TViewModel>(Action<TViewModel>? configureViewModel = null)
        where TPage : Page, new()
        where TViewModel : PageViewModel
    {
        // 1.创建VM（从DI获取）
        var viewModel = factory(typeof(TViewModel)) as TViewModel
                        ?? throw new InvalidCastException($"无法获取或不存在 ViewModel: {typeof(TViewModel)}");

        // 2.执行委托传参
        configureViewModel?.Invoke(viewModel);

        // 3.创建页面
        var page = new TPage
        {
            // 4.绑定VM
            DataContext = viewModel
        };

        return page;
    }
}

// 方式二：不从依赖注入中获取(Func<Type, PageViewModel> factory)，而是通过 App.ViewModelMappings 获取对应的 ViewModel 实例
public class PageNavigationFactory1
{
    /// <summary>
    /// 创建页面并绑定ViewModel + 执行传参委托
    /// </summary>
    public Page GetPage<TPage, TViewModel>(Action<TViewModel>? configureViewModel = null)
        where TPage : Page, new()
        where TViewModel : PageViewModel
    {
        // 1.创建VM（从DI获取）
        var viewModelType = App.ViewModelMappings.FirstOrDefault(x => x.Value == typeof(TPage)).Key
                            ?? throw new InvalidCastException($"无法获取或不存在 ViewModel: {typeof(TViewModel)}");
        var viewModel = (TViewModel)Activator.CreateInstance(viewModelType);

        // 2.执行委托传参
        configureViewModel?.Invoke(viewModel);

        // 3.创建页面
        var page = new TPage
        {
            // 4.绑定VM
            DataContext = viewModel
        };

        return page;
    }
}