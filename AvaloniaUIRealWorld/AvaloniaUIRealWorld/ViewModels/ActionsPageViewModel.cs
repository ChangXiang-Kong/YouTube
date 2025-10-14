using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AvaloniaUIRealWorld.Data.EnumValues;

namespace AvaloniaUIRealWorld.ViewModels
{
    public partial class ActionsPageViewModel : PageViewModel
    {
        public ActionsPageViewModel()
        {
            PageName = ApplicationPageNames.Actions;
        }

        [ObservableProperty]
        private string _Test = "Test Actions";

        // Git Commit Test




    }
}
