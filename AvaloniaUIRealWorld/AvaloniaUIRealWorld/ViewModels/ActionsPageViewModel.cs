using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AvaloniaUIRealWorld.Data.EnumValues;
using AvaloniaUIRealWorld.ViewModels.Actions;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaUIRealWorld.ViewModels
{
    public partial class ActionsPageViewModel() : PageViewModel(ApplicationPageNames.Actions)
    {
        // 使用上面的方式替代以下方式构造函数
        // public ActionsPageViewModel() : base(ApplicationPageNames.Actions)
        // {
        //     PageName = ApplicationPageNames.Actions;
        // }

        [ObservableProperty] private string _Test = "Test Actions";
        [ObservableProperty] private ObservableCollection<ActionsPrintViewModel> _printList;

        [RelayCommand]
        public void RefreshActionsPage(ActionsPageName actionsPageName)
        {
            switch (actionsPageName)
            {
                case ActionsPageName.Print: FetchPrintList(); break;
            }
        }

        [RelayCommand]
        private void FetchPrintList()
        {
            // TODO: Fetch from a database/service provider
            PrintList =
            [
                new ActionsPrintViewModel { Id = "1", JobName = "Print Only Orawings" },
                new ActionsPrintViewModel { Id = "2", JobName = "Print All Drawings Scale To Fit" },
                new ActionsPrintViewModel { Id = "3", JobName = "Print 3D Models A3" },
            ];
        }

        [RelayCommand]
        public void DeletePrintItem(string id)
        {
            // TODO: Pass this logic to a service that handles the database/storage/fetching
            //       For now just do it direct in here

            if (PrintList.Count(x => x.Id == id) != 1)
                // TODO: Throw/Warn?
                return;

            // Remove item
            PrintList.Remove(PrintList.First(x => x.Id == id));
        }


        protected override void OnDesignTimeConstructor() => FetchPrintList();
    }
}