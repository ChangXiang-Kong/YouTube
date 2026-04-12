using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatchProcess3.Data;
using BatchProcess3.ViewModels.Actions;
using CommunityToolkit.Mvvm.Input;

namespace BatchProcess3.ViewModels
{
    public partial class ActionsPageViewModel() : PageViewModel(ApplicationPageName.Actions)
    {
        // 使用上面的方式替代以下方式构造函数
        // public ActionsPageViewModel() : base(ApplicationPageName.Actions)
        // {
        //     PageName = ApplicationPageName.Actions;
        // }

        [ObservableProperty] 
        private string _test = "Test Actions";
        
        // TODO: Remove once we have database service
        private ActionsPrinterProfileViewModel _defaultPrinterProfile = new ActionsPrinterProfileViewModel
        {
            Id = "0",
            Name  = "(Default)",
            Description = "Use all default settings",
            Copies = 1,
            // TODO: Populate PrinterSettings
        };

        // 使用 [] 进行初始化以消除警告，当误写 PrintList = null; 时会提示 Cannot convert null literal to non-nullable reference type
        [ObservableProperty] 
        private ObservableCollection<ActionsPrintViewModel> _printList = [];

        [ObservableProperty] 
        private ActionsPrintViewModel? _selectedPrintListItem;
        
        [ObservableProperty]
        private ObservableCollection<ActionsPrinterProfileViewModel> _printerProfilesList = [];

        [RelayCommand]
        public void RefreshActionsPage(ActionsPageName actionsPageName)
        {
            switch (actionsPageName)
            {
                case ActionsPageName.Print: FetchPrintActionsData(); break;
            }
        }

        [RelayCommand]
        private void FetchPrintActionsData()
        {
            // TODO: Fetch from a database/service provider
            PrintList =
            [
                new ActionsPrintViewModel
                {
                    Id = "1",
                    JobName = "Print Only Orawings",
                    Description = "Prints only drawing files",
                    PrintDrawingRange = "0, 5, 7-8",
                    DrawingExclusionIsWhiteList = true,
                    PrintModels = true,
                    PrintDrawings = true,
                    DrawingExclusionList = $"Some item 1{Environment.NewLine}Some item 2{Environment.NewLine}Some item 3",
                    PrinterProfile = _defaultPrinterProfile,
                },
                new ActionsPrintViewModel
                {
                    Id = "2",
                    JobName = "Print All Drawings Scale To Fit",
                    Description = "Prints drawing scaled to fit the paper",
                    PrintModels = true,
                    PrintDrawings = false,
                    PrinterProfile = _defaultPrinterProfile,
                },
                new ActionsPrintViewModel
                {
                    Id = "3",
                    JobName = "Print 3D Models A3",
                    Description = "Prints models as 3D visuals",
                    PrintModels = false,
                    PrintDrawings = true,
                    PrinterProfile = _defaultPrinterProfile,
                },
            ];

            PrinterProfilesList =
            [
                _defaultPrinterProfile,
                new ActionsPrinterProfileViewModel
                {
                  Name  = "Print Landscape",
                  Description = "Print all files in landscape mode",
                  Copies = 1,
                  // TODO: Populate PrinterSettings
                },
                new ActionsPrinterProfileViewModel
                {
                  Name  = "Print Portrait",
                  Description = "Print all files in portrait mode",
                  Copies = 3,
                  // TODO: Populate PrinterSettings
                },
                new ActionsPrinterProfileViewModel
                {
                  Name  = "A3 Black & White",
                  Description = "Make all A3 prints black and white",
                  Copies = 5,
                  // TODO: Populate PrinterSettings
                },
            ];
        }

        protected override void OnDesignTimeConstructor() => FetchPrintActionsData();

        [RelayCommand]
        private void DeletePrintItem(string id)
        {
            // TODO: Pass this logic to a service that handles the database/storage/fetching
            //       For now just do it direct in here

            if (PrintList.Count(x => x.Id == id) != 1)
                // TODO: Throw/Warn?
                return;

            // Remove item
            PrintList.Remove(PrintList.First(x => x.Id == id));
        }

        [RelayCommand]
        private void AddNewPrintItem()
        {
            // Crate a new item
            var newItem = new ActionsPrintViewModel
            {
                JobName = "New Print Item",
                IsSelected = true,
                IsNewItem = true,
                PrinterProfile = _defaultPrinterProfile,
            };
            
            // Add to the print list
            PrintList.Add(newItem);
        }
    }
}