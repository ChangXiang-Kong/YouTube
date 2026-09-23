using BatchProcess3.Data;

namespace BatchProcess3.ViewModels.MainMenus
{
    public partial class HomePageViewModel() : PageViewModel(ApplicationPageName.Home)
    {
        // 使用上面的方式替代以下方式构造函数
        // public HomePageViewModel() : base(ApplicationPageName.Home)
        // {
        //     // Some logic
        // }
        public string? Test { get; set; } = "Test Home";







    }
}
