using BatchProcess3.Data;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels.MainMenus
{
    public partial class MacrosPageViewModel() : PageViewModel(ApplicationPageName.Macros)
    {
        // 使用上面的方式替代以下方式构造函数
        // public MacrosPageViewModel() : base(ApplicationPageName.Macros)
        // {
        //     // Some logic
        // }

        [ObservableProperty]
        private string _test = "Test Macros";






    }
}
