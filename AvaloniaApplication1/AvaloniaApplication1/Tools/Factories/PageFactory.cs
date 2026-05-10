using AvaloniaApplication1.Data;
using AvaloniaApplication1.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Tools.Factories;

// 方式一：基本写法
public class PageFactory0
{
    public PageFactory0(Func<ApplicationPageName, PageViewModel> factory)
    {
        _pageFactory = factory;
    }

    private readonly Func<ApplicationPageName, PageViewModel> _pageFactory;

    public PageViewModel GetPageViewModel(ApplicationPageName pageName) => _pageFactory.Invoke(pageName);
}

// 方式二：简写
public class PageFactory1(Func<ApplicationPageName, PageViewModel> factory)
{
    public PageViewModel GetPageViewModel(ApplicationPageName pageName) => factory.Invoke(pageName);
}

// 方式三：泛型 可传入参数，参考视频：https://www.youtube.com/watch?v=6bBowIjhk3s&list=PLrW43fNmjaQWwIdZxjZrx5FSXcNzaucOO&index=24
/// <summary>
/// 用于 ContentControl 导航的工厂类
/// </summary>
public class PageFactory(Func<Type, PageViewModel> factory)
{
    // 上 等价于 下
// public class PageFactory
// {
//     // 注入：从 DI 来的 ViewModelFactory
//     public PageFactory(Func<Type, PageViewModel> viewModelFactory)
//     {
//         _viewModelFactory = viewModelFactory;
//     }
//     private readonly Func<Type, PageViewModel> _viewModelFactory;

    public PageViewModel GetPageViewModel<TViewModel>(Action<TViewModel>? configureViewModel = null)
        where TViewModel : PageViewModel
    {
        // var viewModel = factory.Invoke(typeof(T));
        var viewModel = factory(typeof(TViewModel));

        configureViewModel?.Invoke((TViewModel)viewModel);

        return viewModel;
    }
}