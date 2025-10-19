using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AvaloniaUIRealWorld.Data.EnumValues;

namespace AvaloniaUIRealWorld.ViewModels
{
    public partial class HistoryPageViewModel() : PageViewModel(ApplicationPageNames.History)
    {
        // 使用上面的方式替代以下方式构造函数
        // public HistoryPageViewModel()
        // {
        //     PageName = ApplicationPageNames.History;
        // }

        [ObservableProperty]
        private string _Test = "Test History";






    }
}
