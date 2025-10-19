using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AvaloniaUIRealWorld.Data.EnumValues;

namespace AvaloniaUIRealWorld.ViewModels
{
    public partial class HomePageViewModel() : PageViewModel(ApplicationPageNames.Home)
    {
        // 使用上面的方式替代以下方式构造函数
        // public HomePageViewModel()
        // {
        //     PageName = ApplicationPageNames.Home;
        // }
        public string? Test { get; set; } = "Test Home";







    }
}
