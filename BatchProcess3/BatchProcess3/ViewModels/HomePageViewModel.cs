using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatchProcess3.Data;

namespace BatchProcess3.ViewModels
{
    public partial class HomePageViewModel() : PageViewModel(ApplicationPageName.Home)
    {
        // 使用上面的方式替代以下方式构造函数
        // public HomePageViewModel()
        // {
        //     PageName = ApplicationPageName.Home;
        // }
        public string? Test { get; set; } = "Test Home";







    }
}
