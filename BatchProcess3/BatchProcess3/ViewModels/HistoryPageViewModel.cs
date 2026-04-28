using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatchProcess3.Data;

namespace BatchProcess3.ViewModels
{
    public partial class HistoryPageViewModel() : PageViewModel(ApplicationPageName.History)
    {
        // 使用上面的方式替代以下方式构造函数
        // public HistoryPageViewModel() : base(ApplicationPageName.History)
        // {
        //     // Some logic
        // }

        [ObservableProperty]
        private string _test = "Test History";






    }
}
