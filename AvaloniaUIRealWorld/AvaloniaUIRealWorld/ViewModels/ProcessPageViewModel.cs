using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AvaloniaUIRealWorld.Data.EnumValues;

namespace AvaloniaUIRealWorld.ViewModels
{
    public partial class ProcessPageViewModel : PageViewModel
    {
        public ProcessPageViewModel()
        {
            PageName = ApplicationPageNames.Process;
        }

        public string? Test { get; set; } = "Test Process";





    }
}
