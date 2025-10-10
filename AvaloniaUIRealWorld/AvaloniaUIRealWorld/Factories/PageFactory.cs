using AvaloniaUIRealWorld.Data;
using AvaloniaUIRealWorld.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AvaloniaUIRealWorld.Data.EnumValues;

namespace AvaloniaUIRealWorld.Factories
{
    // 方式一：基本写法
    public class PageFactory0
    {
        public PageFactory0(Func<ApplicationPageNames, PageViewModel> factory)
        {
            _pageFactory = factory;
        }

        private readonly Func<ApplicationPageNames, PageViewModel> _pageFactory;

        public PageViewModel GetPageViewModel(ApplicationPageNames pageName) => _pageFactory.Invoke(pageName);

    }

    // 方式二：简写
    public class PageFactory(Func<ApplicationPageNames, PageViewModel> factory)
    {
        public PageViewModel GetPageViewModel(ApplicationPageNames pageName) => factory.Invoke(pageName);
    }


}
