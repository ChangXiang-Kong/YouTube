using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AvaloniaUIRealWorld.Data.EnumValues;

namespace AvaloniaUIRealWorld.ViewModels
{
    public partial class ErrorPageViewModel : PageViewModel
    {
        public ErrorPageViewModel()
        {
            PageName = ApplicationPageNames.Error;
        }

        [ObservableProperty]
        string? _ErrorMsg = "404 Not Found";



    }
}
