using BatchProcess3.Data;
using BatchProcess3.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatchProcess3.Data;

namespace BatchProcess3.Factories;

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
public class PageFactory(Func<Type, PageViewModel> factory)
{
    public PageViewModel GetPageViewModel<T>(Action<T>? afterCreation = null)
        where T : PageViewModel
    {
        var viewModel = factory(typeof(T));
        
        afterCreation?.Invoke((T)viewModel);
        
        return viewModel;
    }
}