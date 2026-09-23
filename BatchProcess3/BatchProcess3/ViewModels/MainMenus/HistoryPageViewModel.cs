using BatchProcess3.Data;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels.MainMenus
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
