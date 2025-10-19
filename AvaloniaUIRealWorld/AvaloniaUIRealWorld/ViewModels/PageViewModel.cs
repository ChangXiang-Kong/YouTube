using AvaloniaUIRealWorld.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AvaloniaUIRealWorld.Data.EnumValues;

namespace AvaloniaUIRealWorld.ViewModels
{
    // 方式一：原始写法
    // public partial class PageViewModel : ViewModelBase
    // {
    //     public PageViewModel(ApplicationPageNames pageName)
    //     {
    //         _pageName = pageName;
    //     }
    //     
    //     [ObservableProperty]
    //     private ApplicationPageNames _pageName;
    // }
    
    // 方式二：简化写法
    public partial class PageViewModel(ApplicationPageNames pageName) : ViewModelBase
    {
        [ObservableProperty]
        private ApplicationPageNames _pageName = pageName;
    }
}
