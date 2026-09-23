using BatchProcess3.Data;

namespace BatchProcess3.ViewModels.MainMenus
{
    public partial class ProcessPageViewModel() : PageViewModel(ApplicationPageName.Process)
    {
        // 使用上面的方式替代以下方式构造函数
        // public ProcessPageViewModel() : base(ApplicationPageName.Process)
        // {
        //     // Some logic
        // }

        public string? Test { get; set; } = "Test Process";





    }
}
