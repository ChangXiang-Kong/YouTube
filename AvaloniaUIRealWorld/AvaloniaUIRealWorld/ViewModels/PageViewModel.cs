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
    public partial class PageViewModel : ViewModelBase
    {
        // 参考视频：https://www.youtube.com/watch?v=qz6Xh7few9g&list=PLrW43fNmjaQWwIdZxjZrx5FSXcNzaucOO&index=17  3:55
        protected PageViewModel(ApplicationPageNames pageName)
        {
            _pageName = pageName;

            // Detect design time 
            if (Avalonia.Controls.Design.IsDesignMode)
                OnDesignTimeConstructor();
        }

        [ObservableProperty] private ApplicationPageNames _pageName;

        protected virtual void OnDesignTimeConstructor()
        {
        }
    }

    // 方式二：简化写法
    // public partial class PageViewModel(ApplicationPageNames pageName) : ViewModelBase
    // {
    //     [ObservableProperty]
    //     private ApplicationPageNames _pageName = pageName;
    // }
}